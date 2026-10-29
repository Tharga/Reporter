# Upgrading to 3.1

Version 3.1 is a bug-fix release, but several of the fixes change what a PDF looks like or how it is
protected. Nothing was removed and no code has to change to compile against it. Read the list below
and check the items that apply to your templates.

## PDFs are no longer encrypted by default

Up to 3.0 every PDF was encrypted with RC4, with fixed permissions (no changes, annotations, form
filling or page assembly) and an owner password that was written in the source code of this public
repository. Anyone could lift the restrictions with that password, so they protected nothing, and
there was no way to produce an unencrypted PDF.

From 3.1 a PDF is not encrypted unless you ask for it. To encrypt, set `Security` on the renderer
with passwords of your own:

```csharp
var renderer = new Renderer(template, data)
{
    Security = new DocumentSecurity
    {
        OwnerPassword = ownerPassword,
        PermitModifyDocument = false,
        PermitAnnotations = false,
        PermitFormsFill = false,
        PermitAssembleDocument = false
    }
};
```

The default algorithm is AES with a 128-bit key. `DocumentEncryption.Aes256` is stronger, and
`DocumentEncryption.Rc4With128Bits` is the algorithm 3.0 used, for readers that cannot open AES. See
[Data and rendering](data-and-rendering.md#document-security).

**If anything downstream relies on the restriction flags of the PDFs you produce**, set `Security`
as above, or those PDFs are produced without restrictions.

## Colours and line thickness are drawn

These properties were stored and serialized but ignored when rendering, so everything was drawn in
black, and any background colour filled its area solid black:

| Element | Properties now applied |
|---|---|
| `Line` | `Color`, `Thickness` |
| `Rectangle` | `BorderColor`, `BackgroundColor` |
| `Text`, `TextBox` | `Font.Color`, or the colour of the section's `DefaultFont` |
| `Table` | `HeaderFont`, `ContentFont` and `GroupFont` colours; `HeaderBorderColor`, `HeaderBackgroundColor`, `ContentBorderColor`, `ContentBackgroundColor`, `GroupBorderColor`, `GroupBackgroundColor` |

A table area with only a background colour is now filled without a border; set its border colour
as well to get one. Table borders are still drawn 0.1 points wide.

A `Line` without a `Thickness` is drawn one point wide, as it always was. Its `Thickness` property
now reports `1px` for that default instead of `0.1px`, which was never drawn.

**Templates that set any of these properties render differently.** Most will look the way they
were meant to; check any that set a colour without seeing its effect.

## Font classes are applied

`Text.FontClass`, `TextBox.FontClass`, `Table.HeaderFontClass` and `Table.ContentFontClass` now
select a font, defined per section in `Section.FontClasses`. See
[Templates](templates.md#font-classes). A class that is not defined falls back to the section's
`DefaultFont`, which is what every font class did before.

## A comma in a unit value is a decimal separator everywhere

`UnitValue.Parse("2,5mm")` used to read the comma with the culture of the machine: 2.5 mm where the
decimal separator is a comma, 25 mm where it is a point. A comma and a point now both mean a decimal
separator on every machine.

**If a template written with commas was rendered on a machine whose decimal separator is a point**,
its sizes and positions shrink back to what was written.

## A nested reference point makes room in a vertical stack

A `ReferencePoint` reported an empty area, so in a `ReferencePoint` with
`Stack = StackMethod.Vertical`, the element after a nested reference point was drawn on top of its
content. It now reports the area its children used, and the elements after it move down.

## Downloaded images expire

An image downloaded from an `http://` or `https://` source used to be cached in the system temp
folder forever, so a changed image behind the same URL was never seen again on that machine. From
3.1:

- The cache is in a folder of its own, `Tharga.Reporter/Images` under the system temp folder.
  Images cached by earlier versions directly in the temp folder are no longer used and can be
  deleted.
- An image is downloaded again once it is older than `Image.CacheDuration`, one hour by default. If
  that download fails, the cached copy is used.
- A download is written under a temporary name and moved into place when it is complete, and
  renders that need the same URL at the same time share one download, so a render never reads a
  half-written file.
- A cached file that cannot be read as an image is downloaded again; if that fails too, the image
  is drawn as missing instead of failing the render.

## Smaller fixes

- **A template can be rendered by several renderers at the same time.** Pagination and stacking
  are kept per render, and rendering no longer changes the template, so a template built once can
  serve many documents, also in parallel. A table no longer keeps the column widths it measured
  for one document as the configuration for the next. `Section.GetPageOffset` and
  `Section.SetPageOffset` are obsolete: the renderer no longer uses them.
- **A `{` without a closing `}`** in a text, table cell, barcode code or image source is kept as
  text, instead of making the render throw `ArgumentOutOfRangeException`.
- **A word wider than its `TextBox`** is drawn once. It used to be drawn twice, which could push
  the rest of the text onto an extra page.
