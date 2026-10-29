# Elements

All visual content lives in one of the section's `ElementList`s — `Header.ElementList`, `Pane.ElementList`, or `Footer.ElementList`. Every element supports the standard positioning properties (`Left`, `Top`, `Right`, `Bottom`, `Width`, `Height`) as `UnitValue` strings, plus `IsBackground` to control draw order.

## Text

Single-line text. Doesn't wrap — anything that exceeds the bounds is clipped.

```csharp
section.Pane.ElementList.Add(new Text
{
    Value = "Invoice {InvoiceNumber}",
    FontSize = "14pt",
    TextAlignment = TextBase.Alignment.Right
});
```

`Value` supports the `{Key}` placeholder syntax — see [Data and rendering](data-and-rendering.md). The `HideValue` property suppresses output when a referenced data key is empty — useful for optional headers.

## TextBox

Multi-line text that wraps and can span multiple pages.

```csharp
section.Pane.ElementList.Add(new TextBox
{
    Value = "Long descriptive copy that needs to wrap across many lines.",
    Width = "100%",
    Top   = "5cm"
});
```

Use a `TextBox` whenever the content length is unbounded. Combine with `ReferencePoint` (below) to stack content that grows.

Text wraps between words. A single word wider than the box is drawn on a line of its own and runs past the edge.

> **Changed in 3.1:** a word wider than the box used to be drawn twice.

## Image

Embed an image from a URL, file path, or stream.

```csharp
section.Header.ElementList.Add(new Image
{
    Source = "https://example.com/logo.png",
    Left = "0", Top = "0",
    Width = "5cm"
});
```

`Source` accepts an HTTP URL, a local file path, or a byte array serialised with `Image.BytesToLongString`. PNG and JPEG are decoded by PDFsharp itself, so images work on every platform.

An `http://` or `https://` source is downloaded and cached in `Tharga.Reporter/Images` under the system temp folder; later renders reuse the cached file until it is older than `Image.CacheDuration` (one hour by default, `TimeSpan.Zero` downloads on every render). If a new download fails, the cached copy is used. A download only appears in the cache once it is complete, and renders that need the same URL at the same time share one download. A `file://` URL reads the local file directly. Other schemes, such as `ftp://`, are not downloaded. If the image cannot be loaded (the file is missing, the server answers with an error, or the download times out after 100 seconds), the element draws a red cross with the message *Image '…' is missing.* instead of failing the render.

> **Changed in 3.1:** downloads used to be cached directly in the system temp folder and never
> refreshed, and a render could read a file another render was still writing.

## BarCode

A Code 39 barcode. The bars are drawn as vector rectangles rather than an embedded raster image, so the
barcode stays sharp at any print size and renders identically on every platform.

```csharp
section.Pane.ElementList.Add(new BarCode
{
    Code   = "1234567890",
    Left   = "8mm",
    Right  = "8mm",
    Top    = "18mm",
    Bottom = "18mm"
});
```

The barcode fills the box you give it; size the box to the print area the scanner expects. A short code in a
very wide box produces bars too wide to scan, so keep the box roughly proportional to the length of the code.

Code 39 encodes digits, upper-case letters, space and `- . $ / + %`. Lower case is upper-cased, so `"abc123"`
encodes as `ABC123`. Anything the symbology cannot represent raises an `InvalidOperationException` naming the
offending character, rather than being silently dropped from the barcode.

## Line

Straight line. Without explicit position, draws across the full width.

```csharp
section.Pane.ElementList.Add(new Line
{
    Thickness = "1mm",
    Color = Color.MidnightBlue
});

section.Pane.ElementList.Add(new Line
{
    Top       = "10cm",
    Thickness = "0.5mm",
    Color     = Color.Black
});
```

Without a `Thickness` the line is one point wide.

> **Changed in 3.1:** `Color` and `Thickness` used to be ignored; every line was drawn black and one point wide.

## Rectangle

Outlined or filled rectangle.

```csharp
section.Pane.ElementList.Add(new Rectangle
{
    Left   = "25%",  Top    = "25%",
    Width  = "50%",  Height = "50%",
    BorderColor      = Color.Green,
    BackgroundColor  = Color.LightYellow
});
```

Set only `BorderColor` for outline, only `BackgroundColor` for solid fill, or both.

> **Changed in 3.1:** the border used to be drawn black whatever `BorderColor` said, and any
> `BackgroundColor` filled the rectangle solid black.

## Table

Renders tabular data from a `DocumentDataTable`. Tables grow across pages and support grouping rows.

```csharp
var table = new Table
{
    Width = "100%",
    DataTableName = "InvoiceLines",
    Columns =
    {
        new TableColumn { Title = "Item",  ColumnName = "Item",  Width = "60%" },
        new TableColumn { Title = "Qty",   ColumnName = "Qty",   Width = "20%", HeaderAlignment = Table.Alignment.Right, ContentAlignment = Table.Alignment.Right },
        new TableColumn { Title = "Price", ColumnName = "Price", Width = "20%", HeaderAlignment = Table.Alignment.Right, ContentAlignment = Table.Alignment.Right },
    }
};

section.Pane.ElementList.Add(table);
```

The matching table data goes onto the `DocumentData` — see [Data and rendering](data-and-rendering.md).

Header, content and group rows are drawn in the colours of `HeaderFont`, `ContentFont` and `GroupFont`. The header
area, the content area and each group row can each have a border colour (`HeaderBorderColor`, `ContentBorderColor`,
`GroupBorderColor`) and a background colour (`HeaderBackgroundColor`, `ContentBackgroundColor`,
`GroupBackgroundColor`); an area with only a background colour is filled without a border.

> **Changed in 3.1:** table colours used to be ignored; text and borders were black and any background colour
> filled its area solid black.

## ReferencePoint

A logical container that holds child elements, with optional vertical stacking. Use it when you want a group of elements to flow as one block and grow downward.

```csharp
var rp = new ReferencePoint
{
    Stack = ReferencePoint.StackMethod.Vertical,
    Top   = "3cm"
};
rp.ElementList.Add(new Text { Value = "First line" });
rp.ElementList.Add(new TextBox { Value = "Body paragraph that wraps." });
rp.ElementList.Add(new Text { Value = "Final line" });

section.Pane.ElementList.Add(rp);
```

With `Stack = Vertical`, each child is anchored directly below the previous one, so a growing `TextBox` shifts the elements after it instead of overlapping them.

A `ReferencePoint` takes up the area its children use, so a reference point nested in a vertical stack pushes the
elements after it down.

> **Changed in 3.1:** a reference point used to report an empty area, so in a vertical stack the element after a
> nested reference point was drawn on top of its content.

## Z-order and `IsBackground`

Elements draw in the order they're added to the `ElementList`. To put something behind the rest, set `IsBackground = true` on the element — backgrounds render in a pre-pass so foreground content overlays them.

## Next

- [Data and rendering](data-and-rendering.md) — `DocumentData`, table data, the full `Renderer` surface, and choosing a `PageFormat`.
