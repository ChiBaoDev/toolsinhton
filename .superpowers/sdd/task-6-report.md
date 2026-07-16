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
