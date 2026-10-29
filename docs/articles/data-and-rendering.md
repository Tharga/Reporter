# Data and rendering

A template is just the layout. The actual values — invoice number, customer name, line items — come from a `DocumentData` instance passed to the `Renderer`. This page covers the data model, the `Renderer`, and the available page formats.

## DocumentData

Build a `DocumentData` and add key/value pairs:

```csharp
var data = new DocumentData();
data.Add("InvoiceNumber", "INV-00042");
data.Add("CustomerName",  "Bob Lablow");
```

Any text element whose `Value` contains `{Key}` tokens will have those tokens replaced at render time:

```csharp
section.Pane.ElementList.Add(new Text { Value = "Invoice {InvoiceNumber}" });
// renders as: "Invoice INV-00042"
```

Pass the data to the `Renderer`:

```csharp
var pdf = new Renderer(template, data).GetPdfBinary(PageSize.A4);
```

If a placeholder references an unknown key, it renders empty. Use `Text.HideValue` to suppress an entire element when a particular key is empty.

A `{` with no closing `}` after it is not a placeholder and is drawn as it is.

> **Changed in 3.1:** a `{` without a closing `}` used to make the render throw `ArgumentOutOfRangeException`.

## Built-in placeholders

The renderer fills these automatically — you don't need to add them to `DocumentData`:

| Token | Resolves to |
|---|---|
| `{Page}` | Current page number |
| `{TotalPages}` | Total pages in the document |
| `{Section}` | Current section name |

So `"Page {Page} of {TotalPages}"` produces `"Page 1 of 3"`.

## Table data

For `Table` elements, add a `DocumentDataTable` with the same name as the table's `DataTableName`:

```csharp
var lines = new DocumentDataTable("InvoiceLines");
lines.Rows.Add(new DocumentDataTableData(new Dictionary<string, string>
{
    { "Item", "Widget" }, { "Qty", "2" }, { "Price", "10.00" }
}));
lines.Rows.Add(new DocumentDataTableData(new Dictionary<string, string>
{
    { "Item", "Gadget" }, { "Qty", "1" }, { "Price", "25.00" }
}));

var data = new DocumentData();
data.Add(lines);
```

Each `DocumentDataTableData` becomes a row; the dictionary keys must match the `ColumnName` on each `TableColumn` you defined.

For visual grouping, use `DocumentDataTableGroup` between data rows — it renders as a styled separator with its `Content` as the label.

## Renderer

The `Renderer` takes a `Template` plus the optional knobs:

```csharp
var renderer = new Renderer(
    template,
    documentData: data,
    includeBackgroundObjects: true,
    documentProperties: properties,
    debug: false
);

byte[] pdf = renderer.GetPdfBinary(PageSize.A4);
```

- **`documentData`** — placeholder + table data. `null` for templates with no placeholders.
- **`includeBackgroundObjects`** — set `false` to omit elements marked `IsBackground = true`. Useful when the foreground is rendered onto pre-printed paper.
- **`documentProperties`** — fills the PDF metadata: `Author`, `Title`, `Subject`, `Creator`.
- **`debug`** — draws guides for element bounds, reference points, and the page grid. Off for production.
- **`Security`** — an init-only property that encrypts the PDF. See [Document security](#document-security).

A `Renderer` renders one document. Create a new one for each document; the `Template` can be shared.
Rendering never changes the template, so one template can serve many documents, also from several
threads at the same time.

> **Changed in 3.1:** rendering used to store pagination and measured table column widths on the
> template, so renders running at the same time mixed up each other's pages, and a table could keep
> the column widths of an earlier document.

## DocumentProperties

PDF metadata seen by the reader:

```csharp
var props = new DocumentProperties
{
    Title   = "Invoice INV-00042",
    Author  = "Acme Corp",
    Subject = "Customer invoice",
    Creator = "MyApp 1.2.0"
};
```

## Document security

A PDF is not encrypted unless you set `Security` on the renderer:

```csharp
var renderer = new Renderer(template, data)
{
    Security = new DocumentSecurity
    {
        OwnerPassword = ownerPassword,
        PermitModifyDocument = false
    }
};
```

- **`OwnerPassword`** gives full access, including changing the permissions.
- **`UserPassword`** is needed to open the document at all. Leave it empty to let anyone open and read it.
- **`Permit…`** — `PermitPrint`, `PermitFullQualityPrint`, `PermitExtractContent`, `PermitModifyDocument`,
  `PermitAnnotations`, `PermitFormsFill` and `PermitAssembleDocument`. All are allowed unless you turn them off.
  They are honoured by the PDF reader, so they only restrict anything while the owner password stays secret.
- **`Encryption`** — `Aes128` (the default, read by practically every reader), `Aes256` (stronger, PDF 2.0), or
  `Rc4With128Bits` (weak; only for readers that cannot open AES).

At least one of the two passwords is required.

> **Changed in 3.1:** every PDF used to be encrypted with RC4, fixed permissions and an owner password
> that was published in the source code. See [Upgrading to 3.1](upgrading-to-3.1.md).

## PageFormat

`PageFormat` accepts the PdfSharp `PageSize` enum directly (implicit conversion):

```csharp
renderer.GetPdfBinary(PageSize.A4);
renderer.GetPdfBinary(PageSize.Letter);
renderer.GetPdfBinary(PageSize.A5);
```

For a custom size, pass millimetres:

```csharp
renderer.GetPdfBinary(new PageFormat(customWidth: 100, customHeight: 150));
```

There's also a built-in `PlasticCard` (85x54mm):

```csharp
renderer.GetPdfBinary(PageFormat.PlasticCard);
```

## End-to-end example

A two-page invoice with a header, body table, footer page numbers, and metadata:

```csharp
using PdfSharp;
using Tharga.Reporter;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;

var section = new Section
{
    Margin = new UnitRectangle { Left = "1cm", Top = "2cm", Right = "1cm", Bottom = "2cm" }
};

section.Header.Height = "2cm";
section.Header.ElementList.Add(new Text { Value = "Invoice {InvoiceNumber}", FontSize = "18pt" });

var table = new Table
{
    Width = "100%",
    DataTableName = "Lines",
    Columns =
    {
        new TableColumn { Title = "Item",  ColumnName = "Item",  Width = "60%" },
        new TableColumn { Title = "Qty",   ColumnName = "Qty",   Width = "20%" },
        new TableColumn { Title = "Price", ColumnName = "Price", Width = "20%" },
    }
};
section.Pane.ElementList.Add(table);

section.Footer.Height = "1cm";
section.Footer.ElementList.Add(new Text { Value = "Page {Page} of {TotalPages}" });

var data = new DocumentData();
data.Add("InvoiceNumber", "INV-00042");

var lines = new DocumentDataTable("Lines");
lines.Rows.Add(new DocumentDataTableData(new Dictionary<string, string>
{
    { "Item", "Widget" }, { "Qty", "2" }, { "Price", "10.00" }
}));
data.Add(lines);

var props = new DocumentProperties
{
    Title  = "Invoice INV-00042",
    Author = "Acme Corp"
};

var pdf = new Renderer(new Template(section), data, documentProperties: props)
    .GetPdfBinary(PageSize.A4);

File.WriteAllBytes("invoice.pdf", pdf);
```
