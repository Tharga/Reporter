# Upgrading to 3.0

Version 3.0 removes every dependency on a platform graphics library, so the same code renders on
Windows, Linux and macOS. Templates, elements and the `Renderer` API are unchanged — but the
underlying PDF library changed, and a few behaviours are deliberately different.

## Why

Version 2.x rendered through `System.Drawing`, which has been Windows-only since .NET 8. Any
template containing a `BarCode` or an `Image` threw `DllNotFoundException` at `GdiplusStartup`
on Linux, and there was no configuration-level workaround.

## What changed

| | 2.4 | 3.0 |
|---|---|---|
| PDF engine | PdfSharp 1.51 + MigraDoc | PDFsharp 6.2 |
| Imaging | `System.Drawing.Common` | none |
| Barcodes | Aspose.BarCode (commercial) | ZXing.Net (Apache-2.0) |
| Runs on | Windows only | Windows, Linux, macOS |

## If you reference PdfSharp types

`PageFormat`, `RenderData` and `DebugData` expose PdfSharp types, so code touching them needs to
compile against the new package.

1. Replace `PdfSharp.MigraDoc.Standard` with `PDFsharp` 6.2.4 in your own project.
2. Rename `XFontStyle` to `XFontStyleEx`.

If you only build `Template`s and call `Renderer.GetPdfBinary`, there is nothing to change.

MigraDoc is no longer a dependency. If your application used it through Tharga.Reporter, reference
`PDFsharp-MigraDoc` directly.

## Fonts

PDFsharp 6 does not read fonts from the operating system by itself. Tharga.Reporter installs a
resolver that checks fonts you register, then fonts installed on the host, then an embedded
fallback — so nothing needs configuring for text to render.

The fallback is not the font you asked for, though. A template naming `Verdana` gets real Verdana
on Windows and the fallback on a Linux host without it, which changes glyph widths and therefore
line breaks. If exact output matters, install the font on the host or register it:

```csharp
ReporterFontResolver.Register("Verdana", false, false, File.ReadAllBytes("fonts/verdana.ttf"));
```

See [Fonts](fonts.md) for the full picture.

## Barcodes

Bars are now drawn as vector rectangles instead of an embedded raster image, so they stay sharp at
any print size. Nothing changes in how you declare a `BarCode`.

**One behaviour is deliberately different, and it is a bug fix.** 2.x silently *dropped* characters
Code 39 cannot encode — `"abc123"` produced a barcode that scanned as `123`, and `"a-b.c"` scanned
as `-.`, with no error. 3.0 upper-cases instead, so the characters survive:

| Code | 2.4 scanned as | 3.0 scans as |
|---|---|---|
| `abc123` | `123` | `ABC123` |
| `ABC123` | `ABC123` | `ABC123` |

Anything Code 39 genuinely cannot represent now raises an `InvalidOperationException` naming the
character rather than quietly producing a barcode with different content.

**If any of your codes contain lower-case letters, the barcode they produce changes.** Check
whether anything downstream — a database lookup, a printed card already in circulation — depends
on the old, truncated value.
