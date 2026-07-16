# Task 6 Report - Vietnamese B3 main, sync, and waveform

## Status
Completed. B3 is reviewed by `ChiBaoDev` with the required note, and its shard contains every and only B3-owned English string leaf in source order.

## Red evidence
The supplied TDD red run reported **3 failed / 6 passed / 9 total**, including the missing `VietnameseDraft/03-main-sync-waveform.json` shard.

## Files
- `tests/UI/TestData/VietnameseDraft/03-main-sync-waveform.json` - 188 manually translated B3 leaves.
- `tests/UI/TestData/VietnameseTranslationBatches.json` - B3 review metadata.
- `tests/UI/Logic/Localization/VietnameseTranslationBatchTests.cs` - stable B3 expectation and deterministic counts.
- `tests/UI/TestData/VietnameseUntranslatedAllowlist.json` - five exact bidirectional color-map/style invariants.
- `docs/localization/vi/glossary.md` - waveform/sync/timing terminology.
- `docs/localization/vi/ui-string-inventory.md` - reviewed B3 scope and count.

## Counts and invariants
- `$.main` excluding B2 nested roots: **125**
- `$.waveform`: **31**
- `$.sync`: **32**
- B3 total: **188**
- Reviewed B1+B2+B3 total: **1,412**
- English-identical B3 values: **5** (`Viridis`, `Plasma`, `Inferno`, `Turbo`, `Neon`), each with an exact allowlist entry and no stale/extra B3 entry.

## Verification
Command: `dotnet test tests/UI/UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests|FullyQualifiedName~CompositeFormatPlaceholderParserTests" --verbosity normal`

Result: **17 passed / 0 failed**, build succeeded with **0 warnings / 0 errors**. `git diff --check` completed with no output. Structural review confirmed exact source order, 188 leaves, and exact bidirectional allowlist equality.

## Translation review
Reviewed all values for placeholder and format-specifier parity; status versus command wording; waveform versus spectrogram; graphical line versus subtitle line; milliseconds, seconds, frames, fps, playback speed, timing speed factor, and offset semantics; and point/visual synchronization terminology. Follow-up review removed a literal source `X` from the merged-lines status, made playback-speed wording explicit in both affected labels, and standardized silence navigation as `Tua đến đoạn im lặng`.

## Self-review
Only task files are included. No runtime-generated files are staged.

## Concerns
None.

## Review fix

Applied every Important and Minor correction from `task-6-review-findings.md`: clarified original/translation switch and merge operations; made all waveform playback-position actions and subtitle-time seeking explicit; expressed silence volume as a threshold; replaced ambiguous bare `dịch` with `di chuyển` for cell/text movement while retaining timing-shift shortcut wording; and refined matching-original import, whitespace trimming, timing-gap sorting, two-second playback return, and percentage-speed labels.

Systematically re-reviewed all **188** B3 values against English source and referenced runtime contexts for status versus command wording, original/translation direction, cell/text movement versus timing shifts, waveform playback positions, silence detection, subtitle-time navigation, gaps, frames/fps, playback speed, timing speed factors, offsets, and placeholder semantics. The re-review found and corrected four additional concrete context issues: character-weighted timing distribution, subtitle timing-change terminology, waveform-centering targets, and shot-change toggling at the current playback position. Structure, source order, 188-leaf count, and the five exact bidirectional allowlist invariants remain unchanged. `xLinesMerged` intentionally has no `{0}` placeholder because the English source has none.

Required combined suite:
`dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests|FullyQualifiedName~CompositeFormatPlaceholderParserTests" --verbosity normal`

Exact result: **17 passed / 0 failed / 17 total**; build succeeded with **0 warnings / 0 errors**. `git diff --check` completed with no errors. The first invocation from the agent workspace failed before build with `MSB1009` because that workspace did not contain the target project; rerunning the same suite against the requested worktree project path produced the successful result above.

Self-review: only the B3 shard and this task report are task-scoped changes; no allowlist update was needed because no corrected value became English-identical or ceased to be English-identical. Runtime-generated `bin`/`obj` artifacts are not staged.
