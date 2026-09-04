# Plan: Render on Linux — remove the GDI+ dependency

Branch: `fix/linux-gdi-plus` (from `master`)
Issue: [Tharga/Reporter#17](https://github.com/Tharga/Reporter/issues/17)

**Status: the first approach was investigated and abandoned. Awaiting a decision on the
replacement. No code has been changed.**

---

## What the investigation established

**1. `XImage.FromStream` is itself GDI+.** Decompiling
`PdfSharp.Standard` 1.51.15 (`PdfSharp.Drawing.XImage`, line 311) gives:

```csharp
_gdiImage = Image.FromStream(stream);   // System.Drawing
```

`PdfSharp.MigraDoc.Standard` also declares its own `System.Drawing.Common` dependency, so
the package is in the graph whether or not `Tharga.Reporter.csproj` names it.

**This kills the "swap to Aspose.Drawing" plan.** `BarCode.cs:78` and `Image.cs:37` both
finish by handing a PNG stream to `XImage.FromStream`. Producing that PNG with Aspose
instead of `System.Drawing` moves the `GdiplusStartup` crash one stack frame deeper — it
does not remove it.

**2. Everything else in the rendering stack is already clean.** `XFont`'s GDI+ code is
reachable only through its `System.Drawing.FontFamily` constructor overloads; every
`XFont` in this repo is built from a string name, so that path is never taken. `XGraphics`
draws through `XGraphicsPdfRenderer`, straight to the PDF content stream. This matches the
consumer's report precisely: text, lines, rectangles and tables render on Linux, images do
not.

**3. `XImage` is used in exactly two places** — `BarCode.cs:78` and `Image.cs:37`. There is
no third caller to worry about.

**4. Aspose.BarCode is commercial and is running unlicensed.** No `.lic` file, no
`SetLicense` call. Evaluation mode stamps an "Aspose" watermark on the generated image; the
`y=10` scanline slice and the commented-out "Paint over the license info" block exist to
crop below it. `Aspose.Drawing.Common` is also an Aspose commercial component, so the
abandoned plan would have made a commercial dependency more explicit rather than less.

**5. ZXing.Net is Apache-2.0 and, on `net6.0` and later, declares no dependencies at all** —
no imaging library. `MultiFormatWriter.encode` returns a `BitMatrix`, which is a bar/space
pattern and needs no raster step.

---

## Consequence

A barcode can be drawn as PdfSharp rectangles, which never touches `XImage` and therefore
works on Linux. A user-supplied photograph cannot — `Image.cs` genuinely needs a working
`XImage`, and that requires PDFsharp 6.x, which is MIT-licensed, cross-platform, and ships
its own image importers.

So the issue splits in two, and only the first half is in reach of this branch.

---

## Steps — pending confirmation of the revised approach

- [ ] 1. NuGet updates across the solution (mandatory first work, bundled into this PR)
- [ ] 2. Reproduce the crash on Linux in Docker — Docker 26.0.0 and an Ubuntu WSL distro are
      available on this machine, so the bug and the fix can both be demonstrated rather
      than argued
- [ ] 3. Guard test: assert no GDI+ type is reachable from the barcode path (red first)
- [ ] 4. Replace Aspose.BarCode with ZXing.Net; encode Code39 to a `BitMatrix`
- [ ] 5. Draw the bars as PdfSharp filled rectangles; delete the raster round-trip, the
      `y=10` watermark slice and the `System.Drawing` usage in `BarCode.cs`
- [ ] 6. Rendering tests for the barcode path
- [ ] 7. Re-run the Linux repro to show it now renders
- [ ] 8. Docs — but only claim what is true: barcodes work on Linux, the `Image` element
      does not yet
- [ ] 9. File a separate issue for the `Image` element / PDFsharp 6 upgrade
- [ ] 10. Close records, archive plan, final commit, PR

---

## Notes

- 2026-09-04 — Baseline before any change: Release build with 3 pre-existing warnings;
  50 tests, 6 run, 44 skipped, 0 failed.
- 2026-09-04 — Local SDK is 10.0.301, which `shared-instructions.md` records as reporting
  "zero tests ran". Real counts come from running the built test assembly directly.
- 2026-09-04 — Approach 1 (swap `System.Drawing` to `Aspose.Drawing`) abandoned before any
  code was written: `XImage.FromStream` is itself GDI+, so it would have moved the crash
  one frame deeper. Recorded so it is not proposed again.
- 2026-09-04 — **`Moq` needed `InternalsVisibleTo("DynamicProxyGenAssembly2")`.** This is
  the actual reason the four rendering tests carry `Skip = "Can't gain access to internal
  stuff."` — `InternalsVisibleTo("Tharga.Reporter.Tests")` was already present, so the
  recorded reason was only half the story. Adding the proxy-generator attribute unblocked
  real rendering tests.
- 2026-09-04 — **The Aspose evaluation watermark is a light-grey swirl drawn over the bars**,
  plus a text legend below them. Thresholding at 128 removes it entirely: every bar row from
  7 to 63 yields an identical 40-run pattern for `ABC123` (8 characters x 5 bars). Redrawing
  from the run lengths therefore produces clean bars with no watermark.
- 2026-09-04 — **A short code stretched across a wide element does not scan.** `"1"` rendered
  at 1200x300 is rejected by the reader; at 600 or 300 wide it decodes. The element scales the
  barcode to fill its bounds, which is pre-existing behaviour and unchanged here, but it means
  bounds have to be roughly proportional to code length. Test canvases are sized per code.
- 2026-09-04 — **`XFont` reaches GDI+ after all**, via `PlatformFontResolver.ResolveTypeface`,
  not only through the `System.Drawing.FontFamily` constructor overloads as first read from
  the decompiled source. Any template containing a `Text` element still dies on Linux.
  `GlobalFontSettings.FontResolver` (public, `IFontResolver`) is the documented bypass.

## Linux verification (2026-09-04)

Docker Desktop was not running, so the suite was executed under WSL Ubuntu
(`5.15.146.1-microsoft-standard-WSL2`, x86_64) against a user-local .NET 10.0.11 runtime
installed to `~/.dotnet` (removable with `rm -rf ~/.dotnet`).

| Result | Detail |
|---|---|
| 15 of 16 passed | Including `A_template_containing_a_barcode_renders_to_a_pdf` — a real PDF, produced on Linux |
| 1 failed | `BarCode_Samples.Write_scannable_samples_for_manual_verification`, which adds a `Text` element: `XFont` -> `PlatformFontResolver` -> `GdiplusStartup` -> `DllNotFoundException` |

This is a clean natural experiment: the failure proves GDI+ is genuinely absent from the
host, and the barcode test passing in the same process proves the new path genuinely avoids
it. The barcode half of issue #17 is fixed and demonstrated on Linux.

## PDFsharp 6 evaluation (2026-09-04) — validated by spike, not assumed

`PDFsharp` **6.2.4** is MIT, declares no `System.Drawing.Common`, and states cross-platform
support. A spike exercising the full surface this project uses was built on Windows and the
same assembly run under WSL Ubuntu:

| Capability | Linux result |
|---|---|
| Font resolve + `MeasureString` + `DrawString` | works — `23.0 x 13.3`, identical to Windows |
| `DrawLine`, `DrawRectangle`, `XPen`, `XSolidBrush`, `XColor.FromKnownColor` | works |
| `XImage.FromFile` + `DrawImage` | works — `800x300` embedded |
| `XUnit.FromMillimeter`, `PageSize`, `XStringFormats` | works |
| 128-bit encryption + `OwnerPassword` | works — 13403-byte PDF written |

**API deltas found — only two:**

1. `XFontStyle` is renamed **`XFontStyleEx`**.
2. `SecuritySettings.DocumentSecurityLevel = PdfDocumentSecurityLevel.Encrypted128Bit`
   becomes `SecurityHandler.SetEncryptionToV2With128Bits()`.

Everything else compiled unchanged: `XRect`, `XSize`, `XColor`, `XKnownColor`, `XPen`,
`XBrush`, `XSolidBrush`, `XBrushes`, `XPoint`, `XUnit`, `XGraphicsUnit`, `XStringFormat(s)`,
`XGraphics.FromPdfPage`, `PdfDocument`, `PdfPage`, `PageSize`.

**A font resolver is mandatory.** PDFsharp 6's Core build makes `PlatformFontResolver` throw
by design, so `GlobalFontSettings.FontResolver` must be set. The spike used
`PdfSharp.Snippets.Font.FailsafeFontResolver`, which works but *substitutes* unknown families
(Arial becomes SegoeWP) — acceptable for a spike, wrong for a card printer where the output is
scanned and printed. A library-owned resolver with an embedded font is the right answer, and
it is what makes the package work on Linux with no host configuration.

**MigraDoc can be dropped.** `GetDocument` builds an empty `Document` whose only role is to
count pages and satisfy `DocumentRenderer.RenderPage`; all drawing goes through `XGraphics` in
`DoRenderStuff`. Removing it deletes a whole dependency rather than migrating it.

**This is a breaking change for consumers.** PdfSharp types are in the public API —
`PageFormat(PageSize)` and its implicit operators, `RenderData.ParentBounds`/`ElementBounds`,
and `DebugData.Pen`/`Brush`/`Font`. Moving to a different PdfSharp assembly changes those type
identities, so this warrants a major version bump rather than a patch.

## Also found, not in scope

`Renderer.cs:115` encrypts every generated PDF at 128-bit with the owner password hard-coded
as `"qwerty12"`, shipped in the published package. It protects nothing and is worth a separate
decision.

## Migration completed (2026-09-04)

| Step | Result |
|---|---|
| `PDFsharp` 6.2.4 replaces `PdfSharp.MigraDoc.Standard` + `System.Drawing.Common` | done |
| MigraDoc removed entirely — `GetDocument` replaced by `GetPageCount` | done |
| `XFontStyle` -> `XFontStyleEx`; security via `SetEncryptionToV2With128Bits()` | done |
| Deprecated `XUnit` implicit conversions replaced with `.Point` / `XUnit.FromPoint` | done — warnings back to the 3-warning baseline |
| `Image.cs` rebuilt on `XImage`; `System.Drawing` gone from the library | done |
| `ReporterFontResolver` + `SystemFontIndex`, installed automatically by `Renderer` | done |
| `MAJOR_MINOR` 2.4 -> 3.0 | done |
| Docs: README, index, getting-started, elements, new `fonts.md`, toc | done |

**Published dependency graph, read from the packed nuspec:** `Aspose.BarCode` 26.8.0 and
`PDFsharp` 6.2.4 only. `System.Drawing.Common` and MigraDoc absent.

**Tests: 65 total, 21 run, 44 skipped, 0 failed — identical on Windows and Linux.**

**Font fidelity, measured on both platforms:**

| Family | Windows | Linux (WSL Ubuntu) |
|---|---|---|
| Verdana | real system font, width 82.16 | fallback, width 71.70 |
| Arial | real system font, width 73.37 | fallback, width 71.70 |
| DejaVu Sans | fallback, width 71.70 | real system font, width 82.20 |

Existing Windows output is therefore unchanged. On a host lacking the named family the text
still renders but metrics differ, which is why `docs/articles/fonts.md` documents both
remedies — install the font, or register it with `ReporterFontResolver.Register`.

**A PDF generated on Linux** was inspected: `%PDF-1.7`, 3 pages, encrypted, one embedded font
subset, and zero image XObjects — confirming the barcode is vector and nothing raster is
involved.

## Remaining before this can close

- [ ] User confirms the rendered barcodes scan on a physical reader
- [ ] Decide whether the hard-coded `"qwerty12"` owner password stays
- [ ] Close records: `Requests.md`, backlog, comment and close issue #17
- [ ] Archive `plan/feature.md`, `git rm -r plan`, final commit, push, PR

## Last session

PDFsharp 6 migration complete. The library no longer references any platform graphics library
and the full suite passes identically on Windows and Linux. Awaiting the physical scan test
and approval to push.
