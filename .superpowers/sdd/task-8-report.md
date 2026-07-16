# Task 8 B5 Evidence

Status: complete

## Final B5 gate closure

- Applied the final parser position fix: structural signatures now include ordinary text segments and escaped-brace tokens, so moving ASS tags relative to text or placeholders is rejected while prior placeholder, escaping, and ASS atom semantics remain intact.
- Corrected residual B5 prose and labels, including merged-line status, ASSA position-tag help, checkered-image label, re-encode status, drawing help, and rotate/style labels.
- Reduced the B5 allowlist to the exact six surviving technical invariants in `tests/UI/TestData/VietnameseUntranslatedAllowlist.json`; each reason is path-specific.
- Final review covered all 607 B5 values against the English extract, including OCR, TTS, video processing, embedded tracks, wrapping, styles, colors, rotation, and advanced effects.

### Final gate commands

- `dotnet test D:/toolsinhton/.claude/worktrees/vietnamese-localization/tests/UI/UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests|FullyQualifiedName~CompositeFormatPlaceholderParserTests" --no-restore --verbosity quiet`
- `git diff --check`


Commit: `042ca69cd feat: translate Vietnamese video OCR and ASSA UI`

## Scope

- Added complete 607-leaf B5 shard in English source order for `$.video`, `$.ocr`, and `$.assa`.
- Enabled B5 review metadata with reviewer `ChiBaoDev` and exact required reviewer note.
- Added exact B5 technical allowlist records and updated glossary and inventory.
- Preserved placeholders, line breaks, markup, mnemonic structure, and ASSA syntax. Added parser coverage for literal ASSA override tags.
- No runtime-generated Settings.json, Languages/, or unrelated runtime artifacts were staged.

## Test evidence

- RED captured after enabling B5 metadata before creating the shard: reviewed-batch test failed because the B5 shard was absent.
- Required absolute-path command: `dotnet test D:/toolsinhton/.claude/worktrees/vietnamese-localization/tests/UI/UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests" --no-restore --verbosity quiet`
- Result: Passed 9, Failed 0, Skipped 0.
- `git diff --check`: passed.
- Working tree after commit: clean.

## Concerns

- A separate full solution build was not run; the required test command built the affected project graph successfully.


## Corrective re-review evidence

- Re-reviewed all 607 B5 leaves in `tests/UI/TestData/VietnameseDraft/05-video-ocr-assa.json` against the English source structure; source order and count remain exact (607 B5 leaves; 3,243 reviewed leaves total).
- Rewrote machine-mixed prose and internal-key labels into Vietnamese, including video burn-in, speech-to-text, TTS, embedded tracks, video OCR, OCR preprocessing, and ASSA effects/status text. Technical identifiers and syntax were retained only where required.
- Removed 18 stale/generic B5 allowlist entries that became translated; remaining allowlist entries are exact English identifiers/syntax/examples with path-specific reasons.
- Parser fix: valid ASS/SSA override tags are parsed narrowly, exact tag sequence/content is included in the comparison signature, malformed and arbitrary `{\...}` composites are rejected, and numeric composite placeholders, bare `{language}`, escaped braces, repeated placeholders, alignment/format, and reorder behavior remain covered.
- Focused parser regression coverage added for valid tags, altered/removed tags, malformed/arbitrary tags, and mixed `{language}` plus ASS tags.

### Exact verification output

Command:
`dotnet test D:/toolsinhton/.claude/worktrees/vietnamese-localization/tests/UI/UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests|FullyQualifiedName~CompositeFormatPlaceholderParserTests" --no-restore --verbosity quiet`

Result: `Passed! - Failed: 0, Passed: 34, Skipped: 0, Total: 34`

Command: `git diff --check`
Result: passed.

## Final all-607 corrective review

- Manually re-reviewed all 607 B5 values against `.superpowers/sdd/b5-english-extract.json`, preserving exact source order/count, placeholders, markup, ASS syntax, shortcuts, bullets, significant spacing, and source-significant line breaks.
- Rewrote internal-key values and machine-mixed prose across video burn-in, speech-to-text, TTS, shot changes, video OCR, embedded tracks, OCR preprocessing, VobSub, ASSA drawing/styles, and advanced effects.
- The B5 untranslated allowlist contains exactly six exact path-specific invariants: CRF, URL, the resolution `x` separator, `Tesseract + LSTM`, standalone OCR, and the OCR title pattern `OCR - {0}`.
- Confirmed parser hardening retains ordered ASS atoms and supports the B5 `\fsp` atom while rejecting malformed/arbitrary/non-finite forms.

### Final exact verification output

Command:
`dotnet test D:/toolsinhton/.claude/worktrees/vietnamese-localization/tests/UI/UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests|FullyQualifiedName~CompositeFormatPlaceholderParserTests" --no-restore --verbosity quiet`

Result: `Passed! - Failed: 0, Passed: 34, Skipped: 0, Total: 34`

Command: `git diff --check`
Result: passed (Git emitted only the configured LF-to-CRLF working-copy warning for the B5 JSON shard).


### Final B5 gate evidence

- Raw-byte and decoded-value audit confirmed `$.video.videoOcr.addAssaPositionTag` decodes to literal `{n8}` and `$.assa.advancedEffectWordSpacingDescription` decodes to literal `sp`; no reviewed B5 syntax value contains an unexpected control character.
- Reviewed all 607 B5 values after the final rewrite, including OCR paths, audio-to-text, TTS, embedded tracks, progress/error messages, wrapping, styles, and advanced effects.
- The allowlist is exactly six entries, consistently listed as `$.video.burnIn.crf=CRF`, `$.video.videoOcr.url=URL`, `$.video.resolutionSeparator=x`, `$.ocr.tesseractEngineModeBoth=Tesseract + LSTM`, `$.ocr.ocr=OCR`, and `$.ocr.ocrX=OCR - {0}`.
- Added decoded ASS syntax/control-character regression coverage to `VietnameseTranslationBatchTests`.
- Final verification commands: `dotnet test D:/toolsinhton/.claude/worktrees/vietnamese-localization/tests/UI/UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests|FullyQualifiedName~CompositeFormatPlaceholderParserTests" --no-restore --verbosity quiet`; `git diff --check`; direct decoded ASS/control-character audit.

Verification result: direct decoded validation passed for literal `{n8}` and `sp`, zero unexpected control characters, and exactly six allowlist entries. Combined suite passed 44/44 with 0 failures. `git diff --check` passed; Git emitted only configured LF-to-CRLF working-copy warnings.
