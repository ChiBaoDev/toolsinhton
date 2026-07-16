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
- `tests/UI/TestData/VietnameseUntranslatedAllowlist.json` - 21 exact bidirectional B4 invariants.
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
There are **21** English-identical B4 values, and exact bidirectional comparison confirms all 21 and only those 21 have allowlist entries. Reasons classify:
- Four language-independent or deliberately English correction examples.
- `Netflix`, `SDI`, `Hunspell`, and `MS Word` product/engine identifiers.
- `SubRip (.srt)` and `Advanced Sub Station Alpha (.ass)` format identifiers.
- `HH:MM:SS:MS`, `HH:MM:SS:FF`, `ms`, and `{0} ms` patterns/units.
- `libmpv - OpenGL` engine/API identifiers.
- `Gamma` and `Video` established technical terms.
- Four macOS modifier symbols.

## Terminology review
Systematically reviewed all 1,224 values against English source and the UI context. The review distinguishes spell checking, normal words, proper-name lists, and user dictionaries; settings, options, and preferences; plugins versus file extensions; engines, models, and services; audio, video, and text operations; casing; cues, frames, duration, gaps, offsets, and time codes; files, folders, and paths; and executables versus command-line terminology. Product, engine, model, executable, format, extension, and path identifiers remain unchanged only where the complete value is precisely allowlisted.

Structural checks confirmed 1,224 exact source-ordered paths, 120 placeholder-bearing values, nine line-break values, four mnemonic-bearing values, tag parity for `<i>`, `</i>`, `<br />`, and `</br>`, and arrow semantics. The parser now validates the named `{language}` runtime token without converting it into a numeric .NET placeholder. The plugin top-level mnemonic was selected to remain unique among displayed siblings.

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
