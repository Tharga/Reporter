# Plan: Migrate to GitHub Actions CI/CD

## Steps

- [x] 1. Create `.github/workflows/build.yml` adapted from Crawler reference
  - `MAJOR_MINOR=2.3`
  - `runs-on: windows-latest` (System.Drawing.Common + Aspose.BarCode)
  - Add `shell: bash` to steps that use bash syntax (Windows runner default is PowerShell)
  - Test filter: `(Category!=Integration)&(Category!=TimeCritical)`
  - Pack: `Tharga.Reporter/Tharga.Reporter.csproj` only
  - Warning threshold bumped from 15 to 200 (existing CA1416 baseline ~150)
- [x] 2. Validate locally: `dotnet restore` + `dotnet build -c Release` + `dotnet test -c Release`
  - Build: 0 errors, 150 warnings (mostly CA1416 System.Drawing platform analyzer)
  - Tests: 6 passed, 44 skipped (legacy `[Skip]` attributes — unrelated to migration), 0 failed
- [x] 3. Commit workflow on develop branch
  - Commit `1b742fa` on `develop`

## User-side follow-ups (not done by Claude)

- [ ] Configure `NUGET_API_KEY` secret in GitHub
- [ ] Create `release` and `prerelease` environments in GitHub
- [ ] Push develop → open PR develop → master → verify pre-release builds
- [ ] Merge PR → verify `2.3.4` release publishes to NuGet
- [ ] Disable Azure DevOps pipeline
- [x] Remove `azure-pipelines.yml` and `buildnumber.yml`
- [ ] Delete `develop` branch after final merge
- [ ] Mark request as Done in `Requests.md`

## Decisions

- **Runner**: `windows-latest` — deviation from Crawler's `ubuntu-latest`. Reporter's PDF/barcode rendering (`System.Drawing.Common`, `Aspose.BarCode`, `PdfSharp.MigraDoc.Standard`) historically requires Windows or `libgdiplus` on Linux. Keeping Windows matches the existing AzDo configuration with no surprises.
- **MAJOR_MINOR=2.3** — current AzDo `majorMinor` is `2.3` and latest published tag is `2.3.3`. Next release will be `2.3.4` (continuous version stream, no collision risk).
- **Branch trigger**: workflow targets `master` only. While the repo is still on the `develop`→`master` flow (this commit), pushes to `develop` will not trigger the workflow. The first real run will be the PR `develop → master`.
- **Working on develop**: user explicitly instructed working on `develop` rather than creating a feature branch. Plan/feature files committed alongside the workflow.

## Last session

**2026-04-28** — `.github/workflows/build.yml` created and committed on `develop` (`1b742fa`). Build (0 errors / 150 warnings) and tests (6 pass / 44 skip / 0 fail) verified locally on net8/9/10 multi-target. Workflow is ready; the user-side follow-ups (NuGet secret, GitHub environments, PR develop → master, AzDo disable, file removal) are pending and tracked above. Next step is for the user to push `develop` and create the PR — first GHA run will be a pre-release on that PR, the merge to master will publish `2.3.4` to NuGet.
