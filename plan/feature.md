# Feature: Migrate to GitHub Actions CI/CD

## Goal
Replace the Azure DevOps build pipeline (`azure-pipelines.yml` + `buildnumber.yml`) with a GitHub Actions workflow that builds, tests, packs, and publishes `Tharga.Reporter` to NuGet, in line with the canonical reference at `c:\dev\tharga\Toolkit\Crawler\.github\workflows\build.yml`.

## Source request
`Tharga.Reporter` entry under "GitHub Actions CI/CD" in `$DOC_ROOT/Tharga/Requests.md` (Priority: Medium, Status: Pending, dated 2026-04-06).

## Scope
- Add `.github/workflows/build.yml` adapted from Crawler.
- Configure for the Reporter codebase:
  - `MAJOR_MINOR=2.3` (latest tag is `2.3.3`; matches current AzDo `majorMinor`).
  - Pack only `Tharga.Reporter/Tharga.Reporter.csproj`. Console (`Exe`) and Tests are not packed.
  - Multi-target SDKs: net8, net9, net10.
  - Preserve test category filter `(Category!=Integration)&(Category!=TimeCritical)` from AzDo.
  - Use `windows-latest` runner — Reporter depends on `System.Drawing.Common` and `Aspose.BarCode`, both Windows-runtime sensitive. This is a deviation from the Crawler `ubuntu-latest` reference, justified by the runtime constraint.

## Out of scope (user-side follow-ups)
- Configuring `NUGET_API_KEY` secret in GitHub repo settings.
- Creating `release` and `prerelease` environments in GitHub.
- Disabling the Azure DevOps pipeline in the AzDo project UI.
- Deleting `azure-pipelines.yml` / `buildnumber.yml` (kept for now until GHA confirmed working).
- Switching branching strategy: feature branches → master, deleting `develop` after final merge from develop → master.

## Acceptance criteria
- `.github/workflows/build.yml` exists and is syntactically valid YAML.
- Local `dotnet restore` + `dotnet build -c Release` + `dotnet test -c Release` all succeed.
- Workflow targets `master` branch (push + PR), matching the post-migration branching strategy.
- Workflow packs `Tharga.Reporter.nupkg` with computed `MAJOR_MINOR.PATCH` versioning derived from git tags.
- After user-side follow-ups, the next PR `develop → master` produces a pre-release on NuGet, and the merge produces a `2.3.4` release.

## Done condition
- Workflow committed on `develop`.
- User confirms first GHA run succeeds on master branch (pre-release + release).
- AzDo pipeline disabled.
- `azure-pipelines.yml` and `buildnumber.yml` removed in a follow-up commit (after AzDo is disabled).
- Request marked Done in `Requests.md`.
