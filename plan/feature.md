# Feature: Render on Linux — remove the GDI+ dependency

## Goal

Make `Tharga.Reporter` render templates on Linux. Today any template containing a
`BarCode` element throws `DllNotFoundException` at `GdiplusStartup`, because
`BarCode.Render` post-processes the barcode through `System.Drawing`, which has been
Windows-only since .NET 8.

Source: [Tharga/Reporter#17](https://github.com/Tharga/Reporter/issues/17), filed
2026-09-04 against a consumer whose member-card printing stopped entirely after a move
from a Windows to a Linux App Service.

## Scope

`System.Drawing` is used in eleven files, but only two of them touch GDI+:

| File | GDI+ usage | In scope |
|---|---|---|
| `Entity/Element/BarCode.cs` | `Image.FromStream`, `Bitmap`, `Graphics.FromImage`, `Save` | Yes |
| `Entity/Element/Image.cs` | `Image.FromFile`, `Image.FromStream`, `Bitmap`, `Graphics`, `Pen`, `Font`, `SolidBrush` | Yes |
| `Font.cs`, `Line.cs`, `Rectangle.cs`, `Table.cs`, `ElementExtensions.cs`, `FontExtensions.cs` | `System.Drawing.Color` only | No |

`Color` lives in `System.Drawing.Primitives`, part of the shared framework, and works on
every platform. `Font.Color` is also **public API**, so re-typing it would be a breaking
change for no benefit. Those six files stay untouched.

`Image.cs` is in scope even though the issue names only `BarCode`: it fails on Linux for
exactly the same reason, so a BarCode-only fix would leave the reporter's next template
failing identically.

## Approach

Swap the two files from `System.Drawing` to **`Aspose.Drawing`**.

`Aspose.Drawing.Common` is already a transitive dependency — `Aspose.BarCode` 26.7.0 on
`net10.0` depends on it and *not* on `System.Drawing.Common`. It is a fully managed,
zero-dependency GDI+ reimplementation, and every member the two files use exists there
with an identical signature (verified against the package's XML documentation):
`Bitmap(Image, Size)`, `Graphics.FromImage`, `DrawImage(Image, PointF[], RectangleF,
GraphicsUnit)`, `DrawImage(Image, int, int, int, int)`, `Image.FromFile`,
`Image.FromStream`, `Save(Stream, ImageFormat)`, `Pen(Color)`, `Font(string, float)`,
`SolidBrush`, `DrawLine`, `DrawString`, `Color.Red`, `Color.DarkRed`, `ImageFormat.Png`.

The `System.Drawing.Common` package reference is then removed outright.

## Acceptance criteria

- [ ] Neither `BarCode.cs` nor `Image.cs` references a `System.Drawing` GDI+ type
- [ ] `Tharga.Reporter.csproj` has no `System.Drawing.Common` package reference
- [ ] The packed `.nuspec` does not list `System.Drawing.Common` as a dependency
- [ ] An architecture test fails before the change and passes after it
- [ ] Rendering a template containing a `BarCode` produces a non-empty PDF
- [ ] Rendering a template containing an `Image` produces a non-empty PDF
- [ ] No public API changes — `Font.Color` stays `System.Drawing.Color`
- [ ] Build is clean and the full suite passes (baseline: 50 tests, 6 run, 44 skipped, 0 failed)
- [ ] `README.md`, `docs/index.md` and `docs/articles/getting-started.md` no longer claim the library is Windows-only, and no longer claim stale `net8.0`/`net9.0` targets

## Done condition

Issue #17 is closed with evidence, the central request is marked Done, and a released
version exists that the reporter can consume.

## Out of scope — noted, not fixed

- **Aspose runs unlicensed.** There is no `.lic` file or `SetLicense` call in the repo, so
  `Aspose.BarCode` renders in evaluation mode with a watermark banner. The `y=10` scanline
  slice in `BarCode.Render` and the commented-out "Paint over the license info" block are
  both working around it. This is pre-existing and unchanged by this fix, but it is a real
  licensing exposure and deserves its own decision.
- **`WebClient` / SYSLIB0014** at `Image.cs:154` — backlog item 1, needs an async decision.
- **44 of 50 tests are skipped** — backlog item 2. This fix adds coverage rather than
  restoring it.
