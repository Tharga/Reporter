# Templates

A document is a `Template` containing one or more `Section`s. Each `Section` has three areas — `Header`, `Pane`, `Footer` — plus a `Margin`, optional `DefaultFont`, and an optional `Name`.

## Template

```csharp
var section = new Section();
section.Pane.ElementList.Add(new Text { Value = "Hello" });

var template = new Template(section);
```

For multi-section documents, build the `Template` with the first section and add more via `SectionList`:

```csharp
var first  = new Section();
var second = new Section();

var template = new Template(first);
template.SectionList.Add(second);
```

Each section in the list renders as one logical block — usually one page, but elements like `TextBox` and `Table` can grow across pages.

## Section areas

A `Section` is laid out top-to-bottom:

```
+--------------------------------+
|         Header                 |  Height controlled by section.Header.Height
+--------------------------------+
|                                |
|         Pane                   |  Fills the remaining space
|                                |
+--------------------------------+
|         Footer                 |  Height controlled by section.Footer.Height
+--------------------------------+
```

Each area is an `ElementList` you push visual elements onto:

```csharp
section.Header.Height = "2cm";
section.Header.ElementList.Add(new Text { Value = "Statement of Account" });

section.Pane.ElementList.Add(new TextBox
{
    Value = "Body text that wraps across lines and pages.",
    Width = "100%"
});

section.Footer.Height = "1cm";
section.Footer.ElementList.Add(new Text { Value = "Page {Page} of {TotalPages}" });
```

## Margins

`Section.Margin` is a `UnitRectangle`. The default is zero on all sides:

```csharp
section.Margin = new UnitRectangle
{
    Left   = "1cm",
    Top    = "2cm",
    Right  = "1cm",
    Bottom = "2cm",
};
```

Element coordinates (`Left`, `Top`, `Right`, `Bottom`, `Width`, `Height`) are measured **inside** the section's margins.

## Units

Positions and sizes are strings parsed by `UnitValue`:

| Suffix | Meaning |
|---|---|
| `cm` | centimetres |
| `mm` | millimetres |
| `pt` | points (1/72 inch) |
| `in` | inches |
| `%` | percentage of the parent area's width or height |

So `Left = "50%"` means halfway across the parent area, and `Width = "100%"` fills it.

## Default font

Set `Section.DefaultFont` to provide a fallback for all text elements in that section:

```csharp
section.DefaultFont = new Font
{
    Name = "Arial",
    Size = "12pt",
    Color = Color.Black
};
```

Individual elements can override by setting their own `Font` property.

## Naming sections

Set `Section.Name` if you want to reference a specific section in `DocumentData` table mappings or downstream tooling. The name is preserved in the XML serialization (`Template.ToXml()` / `Template.Load(xml)`).

## XML round-trip

Templates can be persisted as XML:

```csharp
XmlDocument xml = template.ToXml();
// ... persist xml ...

Template loaded = Template.Load(xml);
```

This is useful when the template is authored by a designer separately from the C# call site, or when templates ship as resources alongside the data that fills them.

## Next

- [Elements](elements.md) — the visual primitives you place inside `Header.ElementList`, `Pane.ElementList`, and `Footer.ElementList`.
- [Data and rendering](data-and-rendering.md) — feeding `DocumentData` into the renderer.
