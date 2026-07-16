# Task 8 B5 Review Findings

Verdicts: Spec compliance NOT APPROVED; code/translation quality NOT APPROVED.

## Critical

1. The parser's ASS override-tag branch accepts arbitrary `{\\...}` forms as opaque content and omits tags from the comparison signature. This can accept malformed forms and lets translations alter/remove `{\\pos(...)}`, `{\\an8}`, drawing commands, or other literal override syntax without detection. Narrow handling to valid required ASS/SSA override syntax, preserve exact tag sequence/content in the signature, and reject malformed/arbitrary composite forms. Add tests for valid tags, altered/removed tags, malformed tags, arbitrary named forms, and mixed `{language}` plus ASS tags while preserving all prior numeric/escaped/alignment/format/reordering behavior.

## Important

2. B5 contains extensive machine-mixed English/Vietnamese prose and is not acceptable as a reviewed translation. Re-review all 607 values, not a sample, especially video burn-in, audio-to-text, text-to-speech, VAD, video OCR, embedded tracks, OCR preprocessing, and ASSA effects/descriptions.
3. Many ordinary labels are internal key names rather than UI text, including `matchSourceVideoSize`, `outputSettings`, `advancedWhisperSettings`, `engineSettingsSubtitle`, `reviewAudioSegments`, `voiceSettings`, `scanArea`, `noLinesFoundMessage`, `goToVideoPosition`, `reEncodeInfo`, `imagePreProcessing`, `preProcessingTitle`, `llamaCppOcrSettingsTitle`, `drawHelpText`, `progressBarTitle`, `resolutionResamplerTitle`, and `stylesTitle`. Translate them according to English source/context.
4. B5 allowlist reasons are generic and several allowlisted entries are ordinary translatable prose. Remove such entries and translate them; retain only actual identifiers/syntax/examples with exact path-specific reasons.
5. Correct progress/status/error grammar and placeholder context throughout B5, including extraction/analyzing progress, no-lines-found, generated-file status, PaddleOCR download, imported-line status, and ASSA generation progress.
6. Apply consistent media/OCR terminology for container, codec, encoding, bitrate, frame, sample rate, model, engine, speech-to-text, text-to-speech, VAD, cue, margin, and resampling. Keep actual identifiers such as FFmpeg, libmpv, Tesseract, PaddleOCR, WSOLA, rubberband, ASSA, and literal tags unchanged only where justified.
7. Preserve ASSA syntax and translate surrounding prose: `{\\an8}`, `\\N`, `\\u1`, override tags, drawing/effect descriptions, and related examples must match source syntax exactly.
8. Because parser behavior is shared, add regression coverage proving prior B1-B4 parser invariants plus exact ASS tag identity/order/content; do not weaken existing gates.

## Minor

9. Clarify glossary distinction between full technical identifiers and descriptive Vietnamese prose for Advanced SubStation Alpha and related B5 terminology.
10. Remove awkward English terminology in ordinary prose (`burned-in`, `folder`, `box`, `margin`, `track`, `stream`, `embedded`, `time codes`, etc.) unless the specific path is a legitimate technical invariant.
11. Avoid unrelated BOM/encoding churn in parser/test files.


## Re-review findings after commit `abbe29fb0`

Verdicts remain: Spec compliance NOT APPROVED; code/translation quality NOT APPROVED.

### Important

12. B5 still contains extensive internal key names and machine-mixed/residual English; re-review and rewrite all affected families, not just examples. Required examples include `matchSourceVideoSize`, `outputSettings`, `advancedWhisperSettings`, `engineSettings`, `reviewAudioSegments`, `voiceSettings`, `scanArea`, `noLinesFoundMessage`, `goToVideoPosition`, `reEncodeInfo`, `imagePreProcessing`, `preProcessingTitle`, `drawHelpText`, `progressBarTitle`, `resolutionResamplerTitle`, `stylesTitle`, plus the listed audio-to-text, TTS, OCR, embedded-track, and ASSA descriptions.
13. Restore all source-significant CRLF/newline, leading-newline, blank-line, bullet, spacing, shortcut, and ASSA syntax content, including `downloadPiperPrompt`, `selectTheBuildToDownload`, `downloadTheLatestOmniVoiceTtsPrompt`, `vulkanRuntimeNotDetectedMessage`, `mergeContinuationLinesPromptMessage`, `reEncodeInfo`, five-space progress separators, `nOcrDrawHelp`, `noWrapping`, smart wrapping, `\\fsp`, and ASSA drawing shortcuts.
14. Expand ASSA parser support to every valid atom actually used by English B5, including at least `\\fsp`; preserve complete ordered tag tokens and their positions relative to text/placeholders; reject malformed parentheses, non-finite coordinates, altered/removed/reordered tags. Add tests for `\\fsp`, repeated/reordered/moved tags, malformed numeric values, and mixed `{language}` plus multiple tags.
15. Replace generic B5 allowlist reasons with exact path-specific reasons and remove/translate ordinary prose such as TTS options, shot-change list, OCR/Karaoke labels where not fixed identifiers.

### Minor

16. Clarify glossary invariant-vs-descriptive Advanced SubStation Alpha terminology and remove awkward English in ordinary prose.
17. Avoid unrelated BOM/encoding churn.

Run the exact combined reviewed-batch/parser suite, append all-607-value review evidence, commit, regenerate package, and re-review.


## Independent final evidence re-review

Verdicts remain: Spec compliance NOT APPROVED; code/translation quality NOT APPROVED.

### Important

18. `task-8-report.md` contained actual BEL (`0x07`) and form-feed (`0x0C`) bytes where its evidence intended the printable literal ASS syntax `{\an8}` and `\fsp`. This contradicted the report's zero-control-character claim. Replace the control bytes with unambiguous printable Markdown code spans, audit every Task 8 evidence Markdown file for unexpected C0 controls other than CR/LF/TAB, record an exact failing verification command and its result, rerun the focused Vietnamese translation/parser tests and `git diff --check`, and commit the evidence-only correction without changing the B5 shard/parser unless separate evidence proves them wrong.


## Latest re-review after commit `ff01cf627`

Verdict: Spec compliance NOT APPROVED; code/translation quality NOT APPROVED.

### Documentation evidence finding

51. `task-8-report.md` records the post-fix passing C0 audit, but it did not preserve the exact pre-fix audit command/script and failing output that demonstrated the BEL (`0x07`) and form-feed (`0x0C`) bytes existed before `ff01cf627`. Append that reproducible pre-fix audit evidence, retain the post-fix passing audit, and rerun/record the focused 44-test command and `git diff --check`. This remains documentation-only; do not alter B5 shard/parser/runtime files or begin Task 9/push.
