# Plan: Migrate to GitHub Actions CI/CD

## Steps

- [~] 1. Create `.github/workflows/build.yml` adapted from Crawler reference
  - `MAJOR_MINOR=2.3`
  - `runs-on: windows-latest` (System.Drawing.Common + Aspose.BarCode)
  - Add `shell: bash` to steps that use bash syntax (Windows runner default is PowerShell)
  - Test filter: `(Category!=Integration)&(Category!=TimeCritical)`
  - Pack: `Tharga.Reporter/Tharga.Reporter.csproj` only
- [ ] 2. Validate locally: `dotnet restore` + `dotnet build -c Release` + `dotnet test -c Release`
- [ ] 3. Commit workflow on develop branch (user explicitly requested working on develop, not feature branch)

## User-side follow-ups (not done by Claude)

- [ ] Configure `NUGET_API_KEY` secret in GitHub
- [ ] Create `release` and `prerelease` environments in GitHub
- [ ] Push develop → open PR develop → master → verify pre-release builds
- [ ] Merge PR → verify `2.3.4` release publishes to NuGet
- [ ] Disable Azure DevOps pipeline
- [ ] Remove `azure-pipelines.yml` and `buildnumber.yml` after AzDo is confirmed disabled
- [ ] Delete `develop` branch after final merge
- [ ] Mark request as Done in `Requests.md`

## Decisions

- **Runner**: `windows-latest` — deviation from Crawler's `ubuntu-latest`. Reporter's PDF/barcode rendering (`System.Drawing.Common`, `Aspose.BarCode`, `PdfSharp.MigraDoc.Standard`) historically requires Windows or `libgdiplus` on Linux. Keeping Windows matches the existing AzDo configuration with no surprises.
- **MAJOR_MINOR=2.3** — current AzDo `majorMinor` is `2.3` and latest published tag is `2.3.3`. Next release will be `2.3.4` (continuous version stream, no collision risk).
- **Branch trigger**: workflow targets `master` only. While the repo is still on the `develop`→`master` flow (this commit), pushes to `develop` will not trigger the workflow. The first real run will be the PR `develop → master`.
- **Working on develop**: user explicitly instructed working on `develop` rather than creating a feature branch. Plan/feature files committed alongside the workflow.

## Last session
_(in progress — see todo list)_
