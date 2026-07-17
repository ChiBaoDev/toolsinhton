# Task 11 Report

## Result
DONE_WITH_CONCERNS

## Scope
- Expanded the finite Task 11 C# scanner to include direct `MessageBox.Show(...)` and arbitrarily qualified aliases such as `Dialogs.MessageBox.Show(...)`, while preserving the legacy Task 10 scanner contract.
- Regenerated and individually classified the scanner-authoritative inventory across the required Tools, SpellCheck, OCR, Video, Translate, Options, ASSA, SSA, Controls, and Logic roots; the absent `src/ui/Features/Plugins` root remains consistently skipped.
- Localized first-party titles, prompts, validation messages, and product-owned errors through existing typed `Language*` classes, including the reviewer hotspots in Batch Convert, Fix Common Errors, Join Subtitles, and SSA Styles.
- Added exact candidate identity for Task 11 rows using source, line, column, scanner kind, literal/source expression; legacy Task 10 literal rows retain their prior no-kind matching behavior.
- Added an independent finite-range `Se.Language.*` source scan with bidirectional source/inventory symmetry. It scans 401 source ranges persisted separately in `tests/UI/TestData/Task11LocalizedSourceRanges.json` and does not seed discovery from localized inventory rows or sweep unrelated pre-existing uses.
- Synchronized constructor defaults, `English.json`, reviewed Vietnamese shards, and generated `Vietnamese.json`. Shared `General.Error`/`General.Warning` entries are reused, and the repeated speech-to-text `Download {0}?` prompt is consolidated as `DownloadXPrompt`.

## RED evidence
- Expanded scanner before inventory regeneration: focused Task 11 test reported direct/qualified `MessageBox.Show` candidates without inventory rows; evidence was captured in `.superpowers/sdd/task11-red.txt` during the migration.
- Exact-column regression before matching fix: `CandidateMatchesRequiresExactColumnForLiteralRows` failed with `Expected: False; Actual: True` for a duplicate same-line literal at a different column.
- Independent source verification before finite ranges were recorded: independently discovered localized expressions had no matching inventory rows.
- Cleanup verification caught two real synchronization errors before GREEN: missing `$.video.audioToText.downloadXPrompt` in generated `Vietnamese.json`, then reviewed-batch counts that still included removed duplicate catalog leaves.
- Task 11 required-root regression RED: after adding `Task11RejectsMissingRequiredRootOtherThanPlugins` and the production call, the focused test build failed with `CS0103` because `AssertTask11RootsExist` did not exist. This demonstrated the required-root behavioral gate was absent before implementation.

## Inventory
- Total: 586
- `localized`: 425
- `technical-exception`: 101
- `external-runtime`: 2
- `non-ui`: 58
- Finite localized source ranges: 401 (independent manifest)
- Non-localized rows without a specific reason: 0
- The two external-runtime rows are Google Lens redirect/parser diagnostics written to the error log, not application UI.

## GREEN verification

### Focused Task 11 inventory tests
```text
dotnet test D:/toolsinhton/.claude/worktrees/vietnamese-localization/tests/UI/UITests.csproj --no-restore --filter "FullyQualifiedName~FirstPartyUiLiteralInventoryTests" --logger "console;verbosity=minimal"
Passed! - Failed: 0, Passed: 19, Skipped: 0, Total: 19, Duration: 8 s
```

The new focused regression test also passed independently: `Task11RejectsMissingRequiredRootOtherThanPlugins` — Failed: 0, Passed: 1, Total: 1. The Task 11 gate now explicitly requires every configured root except `src/ui/Features/Plugins`; Plugins remains optional and is still naturally scanned whenever present because it remains in `RequiredTask11Roots`. Task 10 root validation and scanning were unchanged.

### Complete localization suite
```text
dotnet test D:/toolsinhton/.claude/worktrees/vietnamese-localization/tests/UI/UITests.csproj --no-restore --filter "FullyQualifiedName~UITests.Logic.Localization" --logger "console;verbosity=minimal"
Passed! - Failed: 0, Passed: 107, Skipped: 0, Total: 107, Duration: 34 s
```

Reviewed Vietnamese leaf ownership after duplicate consolidation:
- B1: 1,076
- B2: 171
- B3: 203
- B4: 1,257
- B5: 864
- B6: 30
- Total: 3,601

### Deterministic Vietnamese merge
```text
C:/Windows/System32/WindowsPowerShell/v1.0/powershell.exe -NoProfile -ExecutionPolicy Bypass -File D:/toolsinhton/.claude/worktrees/vietnamese-localization/tools/localization/Merge-VietnameseLanguage.ps1 -RepoRoot D:/toolsinhton/.claude/worktrees/vietnamese-localization
```
Repeated SHA-256 values:
```text
before=3811b024c83f32ae93279ebf5bd36fc74b8bc1b6046d2acda53622ec99130658
first=3811b024c83f32ae93279ebf5bd36fc74b8bc1b6046d2acda53622ec99130658
second=3811b024c83f32ae93279ebf5bd36fc74b8bc1b6046d2acda53622ec99130658
```

### Debug solution build
```text
dotnet build D:/toolsinhton/.claude/worktrees/vietnamese-localization/SubtitleEdit.sln --no-restore --configuration Debug --verbosity minimal
Build succeeded.
1 Warning(s)
0 Error(s)
Time Elapsed 00:00:02.51
```

The warning is pre-existing `CS8600` in `tests/libse/SubtitleFormats/EbuTtDTest.cs:103`.

### JSON and diff validation
```text
Python JSON parse of English.json, Vietnamese.json, Task11LiteralInventory.json, Task11LocalizedSourceRanges.json, VietnameseUntranslatedAllowlist.json, and the four modified reviewed shards
validated-json 8

git -C D:/toolsinhton/.claude/worktrees/vietnamese-localization diff --check
PASS (no whitespace errors; Git emitted only working-tree LF-to-CRLF conversion notices)
```

## Self-review
- Confirmed all 586 inventory rows have exactly one classification and every non-localized row has a concrete reason.
- Restored the two Google Lens diagnostics to `external-runtime` instead of hiding them under blanket technical exceptions.
- Corrected the `NVIDIA's` encoding artifact and verified no replacement characters remain in inventory reasons.
- Verified no stale numbered `DownloadX2`-`DownloadX6` references or duplicate domain `Error`/`Warning` references remain.
- Verified generated/runtime build outputs are not included in the source diff or intended staging set.

## Concerns
- `src/ui/Features/Plugins` does not exist in this checkout; plugin language configuration under the scanned Logic root is still covered.
- Some intentionally invariant English values remain exact only where documented in `VietnameseUntranslatedAllowlist.json` (product/engine names and a language-independent placeholder pattern).
