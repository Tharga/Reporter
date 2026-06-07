---
_layout: landing
---

# Tharga.Reporter

A .NET library for **building and rendering PDF documents** from declarative templates. Define a `Template` of `Section`s with text, images, barcodes, lines, rectangles, and tables, feed it data, and render to a PDF byte array.

Useful when you want to generate receipts, invoices, member cards, labels, reports, or any other structured PDF without hand-writing PdfSharp/MigraDoc code.

## Package

| Package | Target | What it does |
|---|---|---|
| [Tharga.Reporter](https://www.nuget.org/packages/Tharga.Reporter) | net8.0 / net9.0 / net10.0 | Template model (`Section`, `Pane`, `Header`, `Footer`), elements (`Text`, `TextBox`, `Image`, `BarCode`, `Line`, `Rectangle`, `Table`), and a `Renderer` that produces a PDF. |

> Tharga.Reporter is Windows-only at runtime — it uses `System.Drawing.Common` and `Aspose.BarCode` under the hood.

## Quick start

```
dotnet add package Tharga.Reporter
```

```csharp
using Tharga.Reporter;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using PdfSharp;

var section = new Section();
section.Pane.ElementList.Add(new Text { Value = "Hello, world!" });

var template = new Template(section);
var renderer = new Renderer(template);
var pdfBytes = renderer.GetPdfBinary(PageSize.A4);

File.WriteAllBytes("hello.pdf", pdfBytes);
```

See [Getting started](articles/getting-started.md) for the full setup.

## What's in the box

- **Template model** — `Template` wraps one or more `Section`s; each `Section` has a `Header`, `Pane`, `Footer`, and `Margin`. See [Templates](articles/templates.md).
- **Elements** — `Text`, `TextBox`, `Image`, `BarCode`, `Line`, `Rectangle`, `Table`, and `ReferencePoint` for stacking. See [Elements](articles/elements.md).
- **Unit system** — positions and sizes use a string-based unit grammar (`"1cm"`, `"10mm"`, `"50%"`) via `UnitValue` and `UnitRectangle`. Percentages are relative to the parent area.
- **Data substitution** — `DocumentData` lets templates contain `{Placeholder}` tokens that get filled at render time. Tables work the same way for repeating rows. See [Data and rendering](articles/data-and-rendering.md).
- **Page formats** — `PageFormat` wraps PdfSharp's `PageSize` and adds custom millimetre sizes plus a built-in `PlasticCard` (85x54mm).

## Repo

[github.com/Tharga/Reporter](https://github.com/Tharga/Reporter) — source, issues, releases.
