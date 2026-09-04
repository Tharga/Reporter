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

- 2026-09-04 — Baseline recorded before any change: Release build succeeds with 3
  pre-existing warnings; test assembly reports 50 total, 6 run, 44 skipped, 0 failed.
- 2026-09-04 — Local SDK is 10.0.301, which `shared-instructions.md` records as reporting
  "zero tests ran". Real counts come from running
  `Tharga.Reporter.Tests/bin/Release/net10.0/Tharga.Reporter.Tests.exe` directly.
- 2026-09-04 — Approach 1 (swap `System.Drawing` → `Aspose.Drawing`) abandoned before any
  code was written, on the evidence above. Recorded so it is not proposed again.

## Last session

Investigated the fix, found that PdfSharp's own `XImage` is the deeper blocker, and
abandoned the agreed approach before writing code. Revised approach proposed; awaiting a
decision.
