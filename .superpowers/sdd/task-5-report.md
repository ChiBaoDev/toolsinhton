# Task 5 Report — Vietnamese B2 main navigation

## Status

Completed. Batch B2 is reviewed by `ChiBaoDev` with the required review note, and the reviewed shard covers every and only leaf owned by `$.main.menu`, `$.main.toolbar`, and `$.main.waveform` in English structural/property order.

## Files

- `tests/UI/TestData/VietnameseDraft/02-main-navigation.json` — created the reviewed B2 shard.
- `tests/UI/TestData/VietnameseTranslationBatches.json` — marked B2 reviewed with the exact required reviewer and review note.
- `tests/UI/TestData/VietnameseUntranslatedAllowlist.json` — added the exact path-specific invariant for `$.main.menu.video`.
- `tests/UI/Logic/Localization/VietnameseTranslationBatchTests.cs` — updated the stable B2 contract and added deterministic mnemonic-preservation/uniqueness validation for displayed sibling groups.
- `docs/localization/vi/glossary.md` — standardized B2 terms for burned-in subtitle, visual sync, seek, and audio track.
- `docs/localization/vi/ui-string-inventory.md` — recorded the reviewed B2 catalog scope and count.

## Commit

- `99597ed3e16e0d5fef30f0f05d726a8b96d40088` — `feat: translate Vietnamese main navigation UI`

## Red evidence

After marking B2 reviewed and updating the stable manifest expectation, before creating the shard:

```text
Failed: 1, Passed: 6, Skipped: 0, Total: 7
B2: shard file 'VietnameseDraft/02-main-navigation.json' does not exist.
```

Command: `dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests" --verbosity minimal`

## Green evidence

Final Windows .NET 10 / xUnit v3 focused suite:

```text
Test Run Successful.
Total tests: 8
Passed: 8
Build succeeded.
0 Warning(s)
0 Error(s)
```

Command: `dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests" --verbosity normal`

`git diff --check` also completed successfully before commit.

## Catalog counts

- B2 leaves: **171**
  - `$.main.menu`: 125
  - `$.main.toolbar`: 25
  - `$.main.waveform`: 21
- English-identical invariants: **1**
  - `$.main.menu.video` = `_Video`, with an exact allowlist reason.

## Mnemonic validation

- Preserved exactly one mnemonic underscore for every B2 menu value where English supplies one; added none where English supplies none.
- Added deterministic validation for mnemonic uniqueness across 11 displayed sibling groups: top-level, File, Edit, Tools, Spell check, Video, Synchronization, Options, Translate, Help, and ASSA tools.
- The final mnemonic test passes and reports no duplicates within those groups.

## Terminology notes

- Used `Tệp`, `Phụ đề`, `Mã thời gian`, `Dạng sóng`, `Tốc độ khung hình`, and `Chuyển giọng nói thành văn bản` consistently with the B1 glossary.
- Standardized `phụ đề ghi sẵn trên hình` for burned-in subtitles, distinct from embedding a selectable subtitle track.
- Standardized `đồng bộ bằng hình ảnh`, `tua`, and `rãnh âm thanh` for visual synchronization, waveform/video seek, and media audio tracks.
- Reviewed polysemous actions in context: open/close original versus translation, offset versus seek, merge/join/split operations, selection versus current subtitle, and toolbar versus waveform navigation.
- Preserved `{0}`, technical identifiers/acronyms, punctuation semantics, and format names.

## Self-review

- Confirmed root and leaf order exactly follows `English.json`.
- Confirmed no missing or extra B2 paths through the reviewed-batch gate.
- Confirmed placeholder parity and exact bidirectional allowlist behavior.
- Confirmed the only English-identical B2 leaf is allowlisted and no stale reviewed allowlist entry exists.
- Confirmed no runtime-generated `Settings.json`, `Languages/`, `Dictionaries/version.txt`, `Ocr/version.txt`, or `Themes/version.txt` was staged.

## Concerns

None.


## Review fix

### Status and files

Implemented every Important and Minor Task 5 review finding in:

- `tests/UI/TestData/VietnameseDraft/02-main-navigation.json`
- `tests/UI/Logic/Localization/VietnameseTranslationBatchTests.cs`
- `docs/localization/vi/glossary.md`

### Corrections

- Corrected the waveform offset hint to state that the remaining subtitles' timing is shifted.
- Corrected `changeCasing` to `Đổi chữ hoa/thường...` and selected a collision-free Vietnamese mnemonic.
- Standardized burned-in subtitles as `phụ đề nhúng cứng` in the glossary, Video OCR, burn-in menu text, and toolbar hint.
- Improved the new-file, play-next, seek-forward, and copy/paste translation hints requested by review.
- Re-reviewed all **171** B2 values for context and polysemy after the listed corrections. Corrected nine additional concrete issues: multiple replace, bridge gaps, file-level join/split subtitles, duplicate lines, subtitle timing speed, selected-line playback/repeat, and playback-speed wording.

### Deterministic validation

- Retained exact underscore-count parity for every B2 menu value.
- Modeled the actual displayed sibling groups from `src/ui/Features/Main/Layout/InitMenu.cs`, including `goToLineNumber`, ASSA/SSA top-level variants, ASSA layer filtering, and co-displayed reviewed B1 paths (`general.invertSelection`, `general.selectAll`, and `general.more`). Non-B2 paths are included when their owning shard becomes reviewed, so the model does not encode only the current B2 subset.
- Extracted access keys as Unicode text elements, normalized them for comparison, and accepted only a letter/digit followed by combining marks; whitespace, punctuation, underscore, emoji, and malformed graphemes are rejected.
- Added English structural/property-order validation for every reviewed shard while retaining explicit missing/extra leaf diagnostics.
- Deterministically validated **661 reviewed leaves**: B1 **490** + B2 **171**, including ownership, exact paths, ordered paths, placeholder parity, and bidirectional allowlist consistency.
- Validated **12 displayed mnemonic sibling groups** plus a focused Unicode access-key edge-case test.

### Verification

Command:

```powershell
dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests" --verbosity normal
```

Result:

```text
Test Run Successful.
Total tests: 9
Passed: 9
Build succeeded.
0 Warning(s)
0 Error(s)
```

Formatting command:

```powershell
git diff --check
```

Result: completed with no output.

### Self-review

- Confirmed B1 remains green and the B1/B2 reviewed-shard validator passes as one deterministic suite.
- Confirmed B2 still has exactly **171** leaves in English order and preserves all placeholders.
- Confirmed the burned-in-subtitle term is consistent between the B2 shard and glossary.
- Confirmed all modeled displayed mnemonic groups have unique usable access keys among currently reviewed localized siblings.
- Confirmed no runtime-generated files are present in the task change set or staged.

### Concerns

None.
