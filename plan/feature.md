# Feature: Documentation site + PackageIconUrl migration

## Goal
Set up a DocFX documentation site at `reporter.tharga.net` and migrate `<PackageIconUrl>` to `https://thargelion.net/assets/component-reporter.png`. Both items shipped together because they share a release cycle and the icon ends up referenced from both the NuGet listing and the docs site navbar.

## Source requests
- `Move PackageIconUrl to thargelion.net/assets` → Tharga.Reporter ([Requests.md:1670](file:///c:/Users/danie/SynologyDrive/Documents/Notes/Tharga/Requests.md#L1670))
- `Documentation sites under tharga.net` → Tharga.Reporter ([Requests.md:1790](file:///c:/Users/danie/SynologyDrive/Documents/Notes/Tharga/Requests.md#L1790))

## Reference
**Tharga.Console** ([PR #35](https://github.com/Tharga/Console/pull/35)) — pattern to mirror for both items. Console already pairs the icon migration with the docs site in a single feature branch.

## Scope

### PackageIconUrl
Replace `http://thargelion.se/wp-content/uploads/2019/11/Thargelion-White-Icon-150.png` in [Tharga.Reporter.csproj](Tharga.Reporter/Tharga.Reporter.csproj#L10) with `https://thargelion.net/assets/component-reporter.png`. Verify URL resolves before committing.

### Docs site
- `docs/` source tree mirroring Console's layout: `docfx.json`, `CNAME`, `index.md`, `toc.yml`, `articles/` (4 articles + index + toc), `templates/thg/` overlay.
- Use the **absolute-URL + template overlay** approach (option b in the trap warning) — matches Console exactly, lets the logo URL live on `thargelion.net` rather than needing a local copy.
- 4 articles: `getting-started.md`, `templates.md`, `elements.md`, `data-and-rendering.md`. Topics reflect the public API surface (Renderer, Template, Section, Pane, elements, DocumentData, PageFormat).
- CNAME → `reporter.tharga.net` (DNS is pre-req meta-repo work and assumed to be in place).

### README.md
Add docs link line above the existing badges/description, mirroring Console's format.

### Build pipeline
Add to [.github/workflows/build.yml](.github/workflows/build.yml):
- `pages: write` and `id-token: write` to the workflow-level `permissions`.
- `docs` job: `needs: release`, installs DocFX, builds `docs/docfx.json`, uploads pages artifact.
- `docs-deploy` job: `needs: docs`, deploys to `github-pages` environment.

Both jobs gated on `github.ref == 'refs/heads/master' && github.event_name == 'push'` so docs only publish after a successful release (per "CI: Publish docs after release succeeds" pattern at [Requests.md:1854](file:///c:/Users/danie/SynologyDrive/Documents/Notes/Tharga/Requests.md#L1854)).

## Out of scope (user-side)
- DNS configuration for `reporter.tharga.net` → `tharga.github.io` (assumed already done; otherwise pages will deploy under the github.io URL until DNS is set).
- Uploading `component-reporter.png` to `thargelion.net/assets/` (meta-repo pre-req).
- Enabling GitHub Pages in repo settings (one-time setup).

## Acceptance criteria
- `<PackageIconUrl>` references `https://thargelion.net/assets/component-reporter.png` and the URL resolves (HTTP 200).
- `docfx docs/docfx.json` builds the site locally with no errors.
- `dotnet build -c Release` and `dotnet test -c Release` still pass.
- Workflow YAML is syntactically valid; `docs` + `docs-deploy` chained after `release`.
- README links to `https://reporter.tharga.net`.
- On merge to master, a patch release publishes to NuGet (with new icon URL) **and** docs deploy to `reporter.tharga.net`.

## Done condition
- All acceptance criteria met.
- PR merged, release published, docs site live and verified via `curl https://reporter.tharga.net/articles/` returning the absolute-URL logo unchanged (no `../https://...` corruption).
- Both Reporter requests marked Done in `Requests.md`.
