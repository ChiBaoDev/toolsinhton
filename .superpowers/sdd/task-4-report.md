# Task 4 Report: Vietnamese Translation Batch B1

## Status
Completed and committed. B1 is marked reviewed with the exact required reviewer and review note, and the complete B1 shard is present.

## Exact changed files
- `docs/localization/vi/glossary.md`
- `docs/localization/vi/ui-string-inventory.md`
- `tests/UI/Logic/Localization/VietnameseTranslationBatchTests.cs`
- `tests/UI/TestData/VietnameseDraft/01-general-file-edit.json`
- `tests/UI/TestData/VietnameseTranslationBatches.json`
- `tests/UI/TestData/VietnameseUntranslatedAllowlist.json`

## Commit
- `67a566700155d1669adcc21696275365a2a561c6` — `feat: translate Vietnamese general file and edit UI`

## Red command/result
Command requested in the brief:

`dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests&DisplayName~B1" --verbosity normal`

Result: build succeeded, but xUnit reported no matching tests because the current test methods do not have B1 in their display names. To capture the actual contract failure after marking B1 reviewed and before creating the shard, ran:

`dotnet test D:/toolsinhton/.claude/worktrees/vietnamese-localization/tests/UI/UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests" --verbosity minimal`

Result: failed as required with `B1: shard file 'VietnameseDraft/01-general-file-edit.json' does not exist.` The stable-manifest test also failed until its expected B1 review metadata was updated.

## Green command/result
`dotnet test D:/toolsinhton/.claude/worktrees/vietnamese-localization/tests/UI/UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests" --verbosity normal`

Result: passed 6/6 tests, 0 failed; build succeeded with 0 warnings and 0 errors.

Additional deterministic whole-shard validation compared current English ownership with the shard and checked string leaves, accelerator underscore counts, line-break counts, and exact allowlist equality. Result: `B1 leaves=1053; English-identical=21; ownership/string/accelerator/newline/allowlist checks passed`.

`git diff --check` also passed before commit.

## Counts
- B1 leaf count: **1,053**
- English-identical count: **21**

The 21 exact invariants are individually recorded with exact key, value, and reason. They are restricted to product names, the current catalog version, established format/standard identifiers, symbols, and technical syntax. No wildcard entries are used.

## Terminology review notes
- Standardized core subtitle terminology including **phụ đề**, **tệp**, **mã thời gian**, **điểm chuyển cảnh**, **khung hình**, **tốc độ khung hình**, **mã hóa**, **dạng sóng**, **phiên âm**, and **chuyển giọng nói thành văn bản**.
- Preserved product names and technical identifiers such as Subtitle Edit, OCR, Teletext, codec names, file-format identifiers, and model/service names.
- Corrected ambiguous senses in prominent UI terms and polished About text and primary File/Edit actions.
- Expanded the glossary only with terminology concretely used in B1 and recorded the reviewed B1 catalog scope in the inventory; no later C#/markup literal inventory was attempted.

## Test strengthening
`VietnameseTranslationBatchTests.cs` now enforces, for every reviewed shard:
- exact longest-root-owned leaf set (no missing or extra leaves),
- string-only leaves,
- .NET composite placeholder integrity,
- exact allowlist coverage for every value identical to English.

No gate was weakened.

## Self-review
- Confirmed metadata is exactly `Subtitle Edit`, current English `v5.1.0-beta15`, `ChiBaoDev`, and `vi-VN`.
- Confirmed all and only B1-owned leaves are included in current English structural/property order.
- Confirmed placeholder, accelerator, line-break, and allowlist checks pass for every leaf.
- Confirmed runtime-generated `Settings.json`, `Languages/`, `Dictionaries/version.txt`, `Ocr/version.txt`, and `Themes/version.txt` were not staged.
- Confirmed only the six task-scoped files are in the commit.

## Concerns
- The brief's B1 display-name filter currently selects no tests; the actual red failure and final green suite therefore used the full `VietnameseTranslationBatchTests` class filter.
- This is a large catalog (1,053 strings). Automated structural gates are comprehensive, but future native-speaker in-application review may still identify contextual wording refinements in less frequently used dialogs.
## Review fix

### Files
- `tests/UI/Logic/Localization/VietnameseTranslationBatchTests.cs`
- `tests/UI/TestData/VietnameseDraft/01-general-file-edit.json`
- `tests/UI/TestData/VietnameseUntranslatedAllowlist.json`
- `.superpowers/sdd/task-4-report.md`

### Fix commit
- `19f2deb1d` — `fix: refine Vietnamese B1 translations`

### Tests
- Focused command: `dotnet test "D:/toolsinhton/.claude/worktrees/vietnamese-localization/tests/UI/UITests.csproj" -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests" --verbosity normal`
- Result: passed **7/7**, failed 0, skipped 0; build succeeded with 0 warnings and 0 errors.
- Deterministic whole-B1 command: `PYTHONIOENCODING=utf-8 python - <<'PY' ...` (JSON traversal comparing the current English-owned B1 leaf set with the shard, string types, composite placeholders, accelerator underscores, line breaks, and bidirectional exact allowlist equality).
- Result: `B1 leaves=1053; English-identical=21; allowlist=21` and `ownership/string/placeholder/accelerator/newline/exact-allowlist checks passed`.
- `git diff --check` passed.

### Translation corrections
- Replaced every reviewed `shot change` mistranslation with glossary-consistent **điểm chuyển cảnh** wording, including cue, snap, navigation, generation/import, list, move, and video-position actions.
- Standardized media playback actions on **Phát** and speech recognition actions on **Chuyển giọng nói thành văn bản**; corrected transcription service and HTTP header terminology.
- Corrected the reported polysemous/machine-translated terms for formality, API secret, gaps, double lines, effect, forward, media information, casing, post-processing, non-forced lines, waveform cursor, alignment, EBU codes, current-file timing, and user-facing **tệp** wording.
- The systematic B1 re-review also refined the generic header and remaining forward-seek labels. No glossary update was needed because the standardized terms were already present.
- Rewrote all 21 retained allowlist reasons to explain the exact product name, standard, abbreviation, symbol, key label, format, or operator at that path.

### Test strengthening
- Added exact B1 shard assertions for title, current-English version, translator, and culture.
- Enforced bidirectional set equality between English-identical paths in reviewed shards and allowlist keys owned by reviewed batches, so stale extras now fail.
- Preserved ownership, string-leaf, placeholder, exact-English allowlist value, review metadata, and shard existence checks; added deterministic duplicate allowlist-key reporting.

### Self-review
- Re-reviewed all 1,053 B1 values with targeted checks for line, shot, play, case, media, transcription, header, file, forward, effect, and post-processing senses.
- Confirmed exact B1 metadata, 1,053 owned string leaves, 21 English-identical leaves, and 21 matching allowlist entries.
- Confirmed runtime-generated `Settings.json`, `Languages/`, `Dictionaries/version.txt`, `Ocr/version.txt`, and `Themes/version.txt` are not staged.
- No known concerns remain beyond the normal need for future in-application native-speaker review of a large catalog.

## Review fix 2

### Files
- `tests/UI/TestData/VietnameseDraft/01-general-file-edit.json`
- `.superpowers/sdd/task-4-report.md`

### Fix commit
- This commit — `fix: polish Vietnamese B1 terminology`

### Tests
- Requested command: `dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests" --verbosity normal`
- Executed from the worktree with the Git Bash-equivalent project path `./tests/UI/UITests.csproj`.
- Result: test run successful; passed **7/7**, failed 0, skipped 0; build succeeded with **0 warnings** and **0 errors**.
- Deterministic whole-B1 verification result: `B1 leaves=1053; English-identical=21; allowlist=21`; ownership, string, placeholder, accelerator, newline, and exact-allowlist checks passed.

### Translation corrections
- Changed `$.file.import.formattingDotDotDot` to the action/menu label **Định dạng...**.
- Changed `$.general.shadow` to **Đổ bóng**.
- Changed `$.general.styleExaggeration` to **Mức độ cường điệu phong cách**.

### Scope and concerns
- JSON formatting and property order were preserved.
- No test gates were changed or weakened; deterministic coverage still includes all **1,053** B1 leaves.
- Runtime-generated files were not staged.
- No known concerns.
