# Getting started

## Install

For a typical .NET 8 / 9 / 10 application:

```
dotnet add package Tharga.Reporter
```

Tharga.Reporter targets `net8.0`, `net9.0`, and `net10.0`. At runtime it uses `System.Drawing.Common`, `Aspose.BarCode`, and `PdfSharp.MigraDoc`, so it expects a Windows host.

## Namespaces

The core types span three namespaces — import them all at the top of the file that builds your document:

```csharp
using Tharga.Reporter;            // Renderer, PageFormat
using Tharga.Reporter.Entity;     // Template, Section, DocumentData, DocumentProperties
using Tharga.Reporter.Entity.Element;  // Text, TextBox, Image, BarCode, Line, Rectangle, Table
```

- `Tharga.Reporter` — `Renderer` and `PageFormat`, the rendering entry points.
- `Tharga.Reporter.Entity` — the document model: `Template`, `Section`, units, document data, font.
- `Tharga.Reporter.Entity.Element` — the visual elements you stack inside a section's pane, header, or footer.

## Minimal example

The shortest useful program is: build a `Section`, add an element, wrap it in a `Template`, hand it to `Renderer.GetPdfBinary`:

```csharp
using PdfSharp;
using Tharga.Reporter;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;

var section = new Section();
section.Pane.ElementList.Add(new Text { Value = "Hello, world!" });

var template = new Template(section);
var renderer = new Renderer(template);
var pdfBytes = renderer.GetPdfBinary(PageSize.A4);

File.WriteAllBytes("hello.pdf", pdfBytes);
```

`GetPdfBinary` returns the PDF as a `byte[]`, so you can stream it to a HTTP response, save it to disk, or hand it to a print pipeline.

## Adding margins, a header, and a footer

`Section` exposes a `Margin`, a `Header`, a `Pane`, and a `Footer`. Each of the three areas has an `ElementList`:

```csharp
using System.Drawing;
using PdfSharp;
using Tharga.Reporter;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;

var section = new Section
{
    Margin = new UnitRectangle { Left = "1cm", Top = "2cm", Right = "1cm", Bottom = "2cm" }
};
section.Header.Height = "2cm";
section.Header.ElementList.Add(new Text { Value = "Invoice", FontSize = "18pt" });

section.Pane.ElementList.Add(new Text { Value = "Body content here." });

section.Footer.Height = "1cm";
section.Footer.ElementList.Add(new Text { Value = "Page {Page} of {TotalPages}" });

var pdf = new Renderer(new Template(section)).GetPdfBinary(PageSize.A4);
```

Units accept `cm`, `mm`, `pt`, `in`, and `%` (relative to the parent area). The `{Page}` and `{TotalPages}` tokens are built-in placeholders filled by the renderer.

## Next steps

- [Templates](templates.md) — how `Template`, `Section`, `Header`, `Pane`, and `Footer` fit together; multi-section documents.
- [Elements](elements.md) — the catalog of element types and their key properties.
- [Data and rendering](data-and-rendering.md) — `DocumentData` for placeholder substitution and table data, plus the full `Renderer` and `PageFormat` surface.
