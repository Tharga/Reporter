# Plan: Documentation site + PackageIconUrl migration

## Steps

- [x] 1. Create feature branch `feature/docs-and-icon` from master
- [x] 2. Create `plan/feature.md` and `plan/plan.md`
- [ ] 3. Update `<PackageIconUrl>` in [Tharga.Reporter.csproj](Tharga.Reporter/Tharga.Reporter.csproj) → `https://thargelion.net/assets/component-reporter.png`. Verify URL via curl.
- [ ] 4. Create docs root files:
  - `docs/docfx.json` (api metadata from Tharga.Reporter.csproj; modern + thg template; absolute logo URL)
  - `docs/CNAME` = `reporter.tharga.net`
  - `docs/index.md` (landing — overview, packages, quick start, what's in the box)
  - `docs/toc.yml`
- [ ] 5. Write 4 articles + index + toc:
  - `getting-started.md` — install, namespaces, minimal example
  - `templates.md` — Template / Section / Header / Pane / Footer hierarchy, multi-section
  - `elements.md` — Text, TextBox, Image, BarCode, Line, Rectangle, Table, ReferencePoint
  - `data-and-rendering.md` — DocumentData substitution, Renderer, PageFormat, PdfDocumentProperties
  - `articles/index.md` + `articles/toc.yml`
- [ ] 6. Add template overlay (Console option b — absolute-URL trap fix):
  - `docs/templates/thg/layout/_master.tmpl` (copied from Console)
  - `docs/templates/thg/public/main.css` (logo navbar sizing)
- [ ] 7. Update [README.md](README.md) — add `**Docs:** [reporter.tharga.net](https://reporter.tharga.net)` line.
- [ ] 8. Update [.github/workflows/build.yml](.github/workflows/build.yml):
  - Add `pages: write` and `id-token: write` to top-level permissions
  - Append `docs` job (needs: release, install docfx, build site, upload-pages-artifact)
  - Append `docs-deploy` job (needs: docs, deploy-pages, environment github-pages)
- [ ] 9. Validate locally:
  - `curl -I https://thargelion.net/assets/component-reporter.png` returns 200
  - `dotnet tool install -g docfx` if missing
  - `docfx docs/docfx.json` builds cleanly
  - `dotnet build -c Release` passes
  - `dotnet test -c Release` passes
- [ ] 10. Commit + push feature branch, create PR `feature/docs-and-icon → master`

## User-side follow-ups (not done by Claude)
- [ ] Verify DNS for `reporter.tharga.net` is configured to point at `tharga.github.io`
- [ ] Enable GitHub Pages in repo settings (Source: GitHub Actions)
- [ ] Confirm `component-reporter.png` is uploaded to `thargelion.net/assets/`
- [ ] Merge PR — release publishes with new icon, docs deploy to subdomain
- [ ] Mark both requests Done in `Requests.md`

## Decisions

- **Pair both requests in one feature** — user instruction; matches Console's PR #35 pattern.
- **Reference project**: Tharga.Console (user-specified). Inherit absolute-URL logo + template-overlay approach (option b).
- **Article count**: 4. Mirrors Console/Mcp/Runtime; covers the four logical surfaces of Reporter — install/setup, structure, elements, render output.
- **Docs CI integration**: jobs live inside `build.yml`, `needs: release`. No separate `docs.yml`. Per the "CI: Publish docs after release succeeds" pattern, this is the canonical layout.

## Last session
_(in progress)_
