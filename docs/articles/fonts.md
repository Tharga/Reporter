# Fonts

Tharga.Reporter renders text through PDFsharp, which does not read fonts from the operating
system by itself. Tharga.Reporter installs a font resolver for you the first time a `Renderer`
is constructed, so in the common case there is nothing to configure.

## How a font family is resolved

A font named on a `Text`, `TextBox` or `Table` element is looked up in this order:

1. **Fonts you registered** with `ReporterFontResolver.Register`.
2. **Fonts installed on the host** — the resolver scans the usual font directories on Windows,
   Linux and macOS and matches on family name plus bold/italic.
3. **An embedded fallback face**, so a host with no fonts installed still renders text rather
   than throwing.

## Why the same template can look different on Linux

The fallback is what makes a bare container work, but it is not the font you asked for. A
template that names `Verdana` gets real Verdana on a Windows host and the fallback face on a
Linux host that has no Verdana installed. The text still renders and the layout still flows,
but glyph widths differ, so line breaks and centring can land differently.

This matters most when the output is printed or scanned — a membership card, a label, a form
that has to line up with pre-printed stationery.

There are two ways to get identical output everywhere.

**Install the font on the host.** On a Debian-based image:

```dockerfile
RUN apt-get update && apt-get install -y --no-install-recommends fonts-liberation \
    && rm -rf /var/lib/apt/lists/*
```

**Or ship the font with your application** and register it at startup, which does not depend on
the host at all:

```csharp
using Tharga.Reporter.Fonts;

ReporterFontResolver.Register("Verdana", false, false, File.ReadAllBytes("fonts/verdana.ttf"));
ReporterFontResolver.Register("Verdana", true, false, File.ReadAllBytes("fonts/verdanab.ttf"));
```

Register the exact faces your templates use. A family registered as regular-only is still used
for bold and italic text, with the bold or italic appearance simulated.

> Check the licence of any font you embed. Redistributing a font inside an application is not
> always permitted, and the fonts shipped with Windows generally are not.

## Supplying your own resolver

If your application already has a font strategy, assign it before rendering and Tharga.Reporter
will leave it alone:

```csharp
PdfSharp.Fonts.GlobalFontSettings.FontResolver = new MyFontResolver();
```

`GlobalFontSettings.FontResolver` is global and is only set once, so assign it during startup
rather than per request.

## Barcodes do not use fonts

The `BarCode` element draws its bars as vector rectangles and renders no text, so it is
unaffected by font resolution and looks identical on every platform.
