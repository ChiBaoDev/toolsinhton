# Task 10 Report

## Result
DONE

## Scope
- Completed the fixed Main, Files, Edit, Sync, and Shared scans and classified targeted candidates in `docs/localization/vi/ui-string-inventory.md`; technical exceptions, external-runtime metadata, and non-UI schema identifiers remain explicitly justified.
- Localized the targeted first-party UI literals through `Se.Language`, adding matching model defaults and English/Vietnamese catalog leaves.
- Added focused source-policy and Vietnamese runtime coverage in `tests/UI/Logic/Localization/FirstPartyUiLiteralInventoryTests.cs`.
- Added the new leaves to their existing approved shards only: B1 (`file`), B3 (`main`), and B4 (`tools.applyDurationLimits`); ownership remains exact under longest-prefix resolution.
- Regenerated `src/ui/Assets/Languages/Vietnamese.json` exclusively with the deterministic merge script. The generated file is LF-only.

## Fix evidence and commands
- Initial focused localization suite: 87 passed / 3 failed, identifying missing B1/B3/B4 shard paths and CRLF output.
- Windows PowerShell fallback used because `pwsh` was unavailable: `C:\Windows\System32\WindowsPowerShell1.0\powershell.exe -NoProfile -ExecutionPolicy Bypass -File D:	oolsinhton\.claude\worktreesietnamese-localization	ools\localization\Merge-VietnameseLanguage.ps1` — PASS (no output).
- `dotnet test tests/UI/UITests.csproj -c Debug --filter FullyQualifiedName~UITests.Logic.Localization --no-restore --verbosity minimal` — PASS, 90/90.
- `dotnet test tests/UI/UITests.csproj -c Debug --filter FullyQualifiedName~FirstPartyUiLiteralInventoryTests --no-restore --verbosity minimal` — PASS, 2/2.
- `git diff --check` — PASS.
- `dotnet build SubtitleEdit.sln -c Debug --no-restore --verbosity minimal` — PASS, 0 errors, 1 pre-existing nullable warning in `tests/libse/SubtitleFormats/EbuTtDTest.cs:103`.
- `dotnet build SubtitleEdit.sln -c Release --no-restore --verbosity minimal` — PASS, 0 errors, 1 pre-existing nullable warning in `tests/libse/SubtitleFormats/EbuTtDTest.cs:103`.
- `Vietnamese.json` line-ending check — 0 CRLF, 3411 LF.
- No runtime-generated files staged; no push performed.

## Commit
- `eecbe26fee6814a32efe80dbc011d93dc27808b4` (`refactor: localize core Vietnamese UI literals`)
