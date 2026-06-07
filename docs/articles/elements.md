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

`Source` accepts an HTTP URL, a local file path, or anything `System.Drawing.Image.FromStream`/`FromFile` understands.

## BarCode

A Code 39 barcode rendered via Aspose.BarCode.

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

The barcode fills the box you give it; size the box to the print area the scanner expects.

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

## Z-order and `IsBackground`

Elements draw in the order they're added to the `ElementList`. To put something behind the rest, set `IsBackground = true` on the element — backgrounds render in a pre-pass so foreground content overlays them.

## Next

- [Data and rendering](data-and-rendering.md) — `DocumentData`, table data, the full `Renderer` surface, and choosing a `PageFormat`.
