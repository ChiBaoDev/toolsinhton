# Task 7 Report - Vietnamese B4 tools, spell-check, options, and plugins

## Status
Completed. B4 is reviewed by `ChiBaoDev` with the required note, and its shard contains every and only the B4-owned English string leaves in source order.

## Red evidence
After enabling the B4 reviewed gate and before creating the shard, the concise reviewed-batch run reported **2 failed / 7 passed / 9 total**. Both failures were caused by the intentionally missing `VietnameseDraft/04-tools-options.json` shard:

1. `ReviewedBatches_HaveReviewMetadataAndExistingShardFiles`: `B4: shard file 'VietnameseDraft/04-tools-options.json' does not exist.`
2. `B2_MenuMnemonics_ArePreservedAndUniqueWithinDisplayedSiblingGroups`: `FileNotFoundException` while loading the newly reviewed B4 shard.

Evidence is retained in `.superpowers/sdd/task-7-red-output.txt`.

## Files
- `tests/UI/TestData/VietnameseDraft/04-tools-options.json` - 1,224 manually reviewed B4 leaves.
- `tests/UI/TestData/VietnameseTranslationBatches.json` - B4 reviewer and exact review note.
- `tests/UI/Logic/Localization/VietnameseTranslationBatchTests.cs` - stable B4 expectation and deterministic counts.
- `tests/UI/Logic/Localization/CompositeFormatPlaceholderParser.cs` - named-placeholder support required by `{language}`.
- `tests/UI/Logic/Localization/CompositeFormatPlaceholderParserTests.cs` - named-placeholder parity coverage.
- `tests/UI/TestData/VietnameseUntranslatedAllowlist.json` - 26 exact bidirectional B4 invariants.
- `docs/localization/vi/glossary.md` - spell-check, settings, plugin, engine/model/service, path, executable, casing, and timing terminology.
- `docs/localization/vi/ui-string-inventory.md` - reviewed B4 scope and count.

## Root and total counts
- `$.tools`: **512**
- `$.spellCheck`: **39**
- `$.options`: **640**
- `$.plugins`: **33**
- B4 total: **1,224**
- Reviewed B1+B2+B3+B4 total: **2,636**

## Invariants
There are **26** English-identical B4 values, and exact bidirectional comparison confirms all 26 and only those 26 have allowlist entries. Reasons classify:
- Four language-independent or deliberately English correction examples.
- `Netflix`, `SDI`, `Hunspell`, and `MS Word` product/engine identifiers.
- `SubRip (.srt)` and `Advanced Sub Station Alpha (.ass)` format identifiers.
- `HH:MM:SS:MS`, `HH:MM:SS:FF`, `ms`, and `{0} ms` patterns/units.
- `libmpv - OpenGL` engine/API identifier.
- `Gamma`, both `Video` category/preview values, and `AI` established technical terms or abbreviations.
- `Alt`, `Win`, and `Shift` standard keyboard modifier labels.
- Four macOS modifier symbols.

## Terminology review
Systematically reviewed all 1,224 values against English source and the UI context. The review distinguishes spell checking, normal words, proper-name lists, and user dictionaries; settings, options, and preferences; plugins versus file extensions; engines, models, and services; audio, video, and text operations; casing; cues, frames, duration, gaps, offsets, and time codes; files, folders, and paths; and executables versus command-line terminology. Product, engine, model, executable, format, extension, and path identifiers remain unchanged only where the complete value is precisely allowlisted.

Structural checks confirmed 1,224 exact source-ordered paths, 121 placeholder-bearing values, nine line-break values, four mnemonic-bearing values, tag parity for `<i>`, `</i>`, `<br />`, and `</br>`, and arrow semantics. The parser now validates the named `{language}` runtime token without converting it into a numeric .NET placeholder. The plugin top-level mnemonic was selected to remain unique among displayed siblings.

## Green evidence
Required command:
`dotnet test D:/toolsinhton/.claude/worktrees/vietnamese-localization/tests/UI/UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests" --verbosity normal`

Exact result: **9 passed / 0 failed / 9 total**; build succeeded with **0 warnings / 0 errors**.

Combined parser and reviewed-batch command:
`dotnet test D:/toolsinhton/.claude/worktrees/vietnamese-localization/tests/UI/UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests|FullyQualifiedName~CompositeFormatPlaceholderParserTests" --logger "console;verbosity=minimal"`

Exact result: **18 passed / 0 failed / 18 total**.

`git diff --check` completed with no output.

## Commit
`feat: translate Vietnamese tools and options UI`

## Self-review
The shard has every and only the four B4 roots in true English order. Exact path order, leaf counts, line breaks, markup, arrows, mnemonic counts, placeholders, and bidirectional allowlist equality were checked programmatically. Translatable prose was reviewed for residual English, including follow-up corrections to settings labels and tool examples. Runtime-generated `bin`/`obj` artifacts are not staged.

## Concerns
None.
## Review-fix pass
Addressed all Task 7 Critical/Important and same-pass Minor findings. The full 1,224-leaf B4 shard was re-reviewed by root against English and runtime usage, with focused cross-family sweeps for shortcut commands, unbreak operations, shot-change/frame snapping, settings labels and colors, engine/model/plugin concepts, burned-in subtitles, spell-check wording, status/count strings, actor/dialogue actions, and residual English. Corrections preserve the original path order and leaf count, placeholders, markup, line breaks, and mnemonics.

Representative fixes include the corrupted multiplication sign, standard Windows modifier labels, operational shortcut verbs (`Lưu`, `Thoát`, `Tìm`), consistent `bỏ ngắt dòng`, `điểm chuyển cảnh`/`căn vào...` terminology, foreground/background and focused-control labels, `bộ máy`, burned-in subtitle and plugin wording, Netflix validation semantics, proper-name/ignored-word list terminology, and malformed imported/skipped and completion messages. Exact English equality remains limited to legitimate allowlist entries; `Alt`, `Win`, and `Shift` were added as standard keyboard modifier labels.

The composite-format parser now preserves numeric integer semantics (`{00}` equals `{0}`), rejects oversized numeric indexes and mixed/unknown identifiers, accepts only the exact bare `{language}` runtime token, and rejects alignment or format syntax on that named token. Regression coverage also retains malformed/escaped brace, repetition, alignment, format, and reordering behavior.

Focused verification command:
`dotnet test D:/toolsinhton/.claude/worktrees/vietnamese-localization/tests/UI/UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests|FullyQualifiedName~CompositeFormatPlaceholderParserTests" --logger "console;verbosity=minimal"`

Exact result: **25 passed / 0 failed / 25 total**.

`git diff --check` completed with no output.
Late independent tools-root review additionally corrected gap allocation terminology, media-rãnh wording, standalone Netflix shot-change and whitespace diagnostics, incrementally accumulated lines, join-file time-code offset behavior, speaker metadata conversion, title-safe subtitle-area wording, and explicit square-bracket terminology.
A subsequent independent options review covered the remaining settings, all shortcut leaves, word lists, and language chooser. It corrected additional keyboard-focus, selection, merge direction, dialogue, clipboard, media, timing, waveform, category, import, ASSA drawing, sort, and sentence-case defects while preserving structural invariants.
## Second review-fix pass (findings 15-28)

Addressed every appended finding 15-28 and swept adjacent same-family B4 values. The parser now accepts only the exact bare `{language}` token and rejects trailing spaces, tabs, newlines, alignment, format syntax, mixed identifiers, and oversized numeric indexes while preserving numeric normalization and composite-format semantics. Corrected Netflix whitespace semantics, continuation labels and case, spell-check engine terminology, conditional error-color labels, end-time controls, spacing, AI-review `rà soát` wording, merge-line labels with `dòng`, Enter-key action, toolbar-open wording, waveform centering, and source-image wording. Existing B4 structure, path order, leaf count, placeholders, markup, line breaks, mnemonics, and allowlist invariants remain intact.

Focused command:
`dotnet test D:/toolsinhton/.claude/worktrees/vietnamese-localization/tests/UI/UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests|FullyQualifiedName~CompositeFormatPlaceholderParserTests" --logger "console;verbosity=minimal"`

Exact output:
```
Passed!  - Failed:     0, Passed:    28, Skipped:     0, Total:    28, Duration: 421 ms - UITests.dll (net10.0)
```

`git diff --check` completed with no output.


## Third review-fix pass (findings 29-34)

Applied the final listed Task 7 review corrections: replaced residual `clipboard` with `bảng nhớ tạm` in both copy commands; corrected `continuationStyleNoneLeadingTrailingEllipsis`, `subtitleGridCenterSelectedRow`, and `waveformSetVideoPositionOnMoveStartEnd`; corrected the B4 exact English-identical count to 26 and the placeholder-bearing count to 121. The allowlist classification explicitly includes `Alt`, `Win`, `Shift`, `Video`, and `AI`. A scoped documentation sweep found no stale B4 `21`/`120` claims in task-scoped docs or reports.

Focused verification command:
`dotnet test D:/toolsinhton/.claude/worktrees/vietnamese-localization/tests/UI/UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests|FullyQualifiedName~CompositeFormatPlaceholderParserTests" --logger "console;verbosity=minimal"`

Exact output:
```
  Determining projects to restore...
  All projects are up-to-date for restore.
  LibSE -> D:\toolsinhton\.claude\worktrees\vietnamese-localization\src\libse\bin\Debug\netstandard2.1\libse.dll
  LibUiLogic -> D:\toolsinhton\.claude\worktrees\vietnamese-localization\src\libuilogic\bin\Debug\net10.0\libuilogic.dll
  UI -> D:\toolsinhton\.claude\worktrees\vietnamese-localization\src\ui\bin\Debug\net10.0\SubtitleEdit.dll
  UITests -> D:\toolsinhton\.claude\worktrees\vietnamese-localization\tests\UI\bin\Debug\net10.0\UITests.dll
Test run for D:\toolsinhton\.claude\worktrees\vietnamese-localization\tests\UI\bin\Debug\net10.0\UITests.dll (.NETCoreApp,Version=v10.0)
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:    28, Skipped:     0, Total:    28, Duration: 406 ms - UITests.dll (net10.0)
```

`git diff --check` completed with no output (Git emitted only LF-to-CRLF normalization warnings for the three changed text files).
