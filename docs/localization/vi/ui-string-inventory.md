| Source | Location/key | Classification | Language key / reason |
|---|---|---|---|
| English catalog | `$.general.ok` | localized | `$.general.ok` |
| English catalog | `$.title`, `$.version`, `$.translatedBy`, `$.cultureName`, `$.general`, `$.file`, `$.edit`, `$.help`, `$.about` | localized | B1 reviewed by ChiBaoDev; complete reviewed shard in `tests/UI/TestData/VietnameseDraft/01-general-file-edit.json` with exact invariant values recorded in `VietnameseUntranslatedAllowlist.json` |
| English catalog | `$.main.menu`, `$.main.toolbar`, `$.main.waveform` | localized | B2 reviewed by ChiBaoDev; all 171 owned menu, toolbar, and waveform-navigation leaves are in `tests/UI/TestData/VietnameseDraft/02-main-navigation.json`; exact invariant retained in `VietnameseUntranslatedAllowlist.json` |
| English catalog | `$.main` excluding `$.main.menu`, `$.main.toolbar`, and `$.main.waveform`; plus `$.waveform`, `$.sync` | localized | B3 reviewed by ChiBaoDev; all 203 owned status, waveform-operation, timing, and synchronization leaves are in `tests/UI/TestData/VietnameseDraft/03-main-sync-waveform.json`; exact invariants recorded in `VietnameseUntranslatedAllowlist.json` |
| English catalog | `$.tools`, `$.spellCheck`, `$.options`, `$.plugins` | localized | B4 reviewed by ChiBaoDev; all 1,257 owned tool, spell-check, settings, shortcut, and plugin leaves are in `tests/UI/TestData/VietnameseDraft/04-tools-options.json`; 26 exact technical identifiers, formats, examples, units, standard modifier/category labels, and macOS symbols are recorded in `VietnameseUntranslatedAllowlist.json` |

| English catalog | `$.video`, `$.ocr`, `$.assa` | localized | B5 reviewed by ChiBaoDev; all 864 owned video, media-processing, OCR, and Advanced SubStation Alpha leaves were re-reviewed path by path against the English extract in `tests/UI/TestData/VietnameseDraft/05-video-ocr-assa.json`; exact path-specific invariant identifiers and syntax are recorded in `VietnameseUntranslatedAllowlist.json` |

| `src/ui/Features/Main/Layout/LayoutWindow.cs:27` | `Choose layout` | localized | `$.main.layoutTitle`; `Se.Language.Main.LayoutTitle` |
| `src/ui/Features/Files/ExportPac/ExportPacWindow.cs:13,19` | `Export Pac`; `Choose PAC code page` | localized | `$.file.exportPacTitle`; `$.file.choosePacCodePage` |
| `src/ui/Features/Files/ExportCavena890/ExportCavena890Window.cs:13` | `Export Cavena 890` | localized | `$.file.exportCavena890Title` |
| `src/ui/Features/Files/ExportEbuStl/ExportEbuStlWindow.cs:16` | `Export EBU STL` | localized | `$.file.exportEbuStlTitle` |
| `src/ui/Features/Shared/DownloadFfmpegWindow.cs:17` | `Downloading ffmpeg` | localized | `$.main.downloadingFfmpeg` |
| `src/ui/Features/Shared/DownloadLibMpvWindow.cs:17` | `Downloading libmpv` | localized | `$.main.downloadingLibMpv` |
| `src/ui/Features/Shared/BinaryEdit/SetText/SetTextWindow.cs:18` | `Set text` | localized | `$.tools.imageBasedEdit.setText` |
| `src/ui/Features/Shared/BinaryEdit/BinaryApplyDurationLimits/BinaryApplyDurationLimitsWindow.cs:44,66` | `Minimum duration (milliseconds):`; `Maximum duration (milliseconds):` | localized | `$.tools.applyDurationLimits.minimumDurationMilliseconds`; `$.tools.applyDurationLimits.maximumDurationMilliseconds` |
| `src/ui/Features/Shared/PickVobSubLanguage/PickVobSubLanguageViewModel.cs:51` | `Pick VobSub language - {0}` | localized | `$.main.pickVobSubLanguageTitle`; `Se.Language.Main.PickVobSubLanguageTitle` |
| `src/ui/Features/Main/MainView.cs:39; Layout/InitVideoPlayer.cs:151` | diagnostic exception text | technical-exception | diagnostics preserved |
| `src/ui/Features/Edit/MultipleReplace/CsvExporter.cs:13` | CSV header identifiers | non-ui | schema identifiers preserved |
| `src/ui/Features/Shared/PickMp4Track/PickMp4TrackWindow.cs:98-181` | media metadata column identifiers | external-runtime | technical media metadata preserved |


## Task 11 fixed-scan inventory

The structured source of truth is `tests/UI/TestData/Task11LiteralInventory.json`. The fixed scanner covers the required Tools, SpellCheck, OCR, Video, Translate, Options, ASSA, SSA, Plugins, Controls, and Logic roots, including direct `MessageBox.Show(...)`, qualified aliases such as `Dialogs.MessageBox.Show(...)`, the existing UI-property patterns, and the Task 11 toast/notification/exception patterns. Every candidate and inventory row must match exactly once by source, line, column, scanner kind, and literal or source expression.

The independently maintained finite source manifest is `tests/UI/TestData/Task11LocalizedSourceRanges.json`. It contains 401 reviewed source ranges and drives a separate bidirectional `Se.Language.*` expression scan; expression discovery is not seeded from the inventory rows and does not sweep unrelated pre-existing catalog uses.

| Inventory | Total | localized | technical-exception | external-runtime | non-ui |
|---|---:|---:|---:|---:|---:|
| Task 11 fixed scan | 586 | 425 | 101 | 2 | 58 |

- The two `external-runtime` rows are Google Lens HTTP redirect and response-parser diagnostics written to the error log rather than rendered as application UI.
- Every one of the 161 non-localized rows has a concrete, row-specific reason; no deferred classification remains.
- Task 11 additions retain ownership in existing typed `Language*` classes and reviewed B4, B5, and B6 shards. Current reviewed ownership is B1 1,076; B2 171; B3 203; B4 1,257; B5 864; B6 30; total 3,601 leaves.
- Direct-message localization explicitly covers the reviewed Batch Convert, Fix Common Errors, Join Subtitles, SSA Styles, OCR, video, speech-to-text, text-to-speech, Translate, and Options hotspots.

## Task 10 fixed-scan inventory

The structured source of truth is `tests/UI/TestData/Task10LiteralInventory.json`. The scanner covers C# UI literals and every `Se.Language.*` member-expression occurrence across the exact five roots, plus all English-bearing `.xaml`/`.axaml` attributes and decoded/trimmed element text, even when ASCII English letters follow punctuation, digits, or entities. Every candidate and inventory row must match exactly once.

| Source | Candidate | Classification | Language key / reason |
|---|---|---|---|
| `src/ui/Features/Main/Layout/LayoutWindow.cs:27` | `Choose layout` | localized | `$.main.layoutTitle` via `Se.Language.Main.LayoutTitle` |
| `src/ui/Features/Files/ExportPac/ExportPacWindow.cs:13` | `Export Pac` | localized | `$.file.exportPacTitle` via `Se.Language.File.ExportPacTitle` |
| `src/ui/Features/Files/ExportPac/ExportPacWindow.cs:19` | `Choose PAC code page` | localized | `$.file.choosePacCodePage` via `Se.Language.File.ChoosePacCodePage` |
| `src/ui/Features/Files/ExportCavena890/ExportCavena890Window.cs:13` | `Export Cavena 890` | localized | `$.file.exportCavena890Title` via `Se.Language.File.ExportCavena890Title` |
| `src/ui/Features/Files/ExportEbuStl/ExportEbuStlWindow.cs:16` | `Export EBU STL` | localized | `$.file.exportEbuStlTitle` via `Se.Language.File.ExportEbuStlTitle` |
| `src/ui/Features/Shared/DownloadFfmpegWindow.cs:17` | `Downloading ffmpeg` | localized | `$.main.downloadingFfmpeg` via `Se.Language.Main.DownloadingFfmpeg` |
| `src/ui/Features/Shared/DownloadLibMpvWindow.cs:17` | `Downloading libmpv` | localized | `$.main.downloadingLibMpv` via `Se.Language.Main.DownloadingLibMpv` |
| `src/ui/Features/Shared/BinaryEdit/SetText/SetTextWindow.cs:18` | `Set text` | localized | `$.tools.imageBasedEdit.setText` via `Se.Language.Tools.ImageBasedEdit.SetText` |
| `src/ui/Features/Shared/BinaryEdit/BinaryApplyDurationLimits/BinaryApplyDurationLimitsWindow.cs:44` | `Minimum duration (milliseconds):` | localized | `$.tools.applyDurationLimits.minimumDurationMilliseconds` via `Se.Language.Tools.ApplyDurationLimits.MinimumDurationMilliseconds` |
| `src/ui/Features/Shared/BinaryEdit/BinaryApplyDurationLimits/BinaryApplyDurationLimitsWindow.cs:66` | `Maximum duration (milliseconds):` | localized | `$.tools.applyDurationLimits.maximumDurationMilliseconds` via `Se.Language.Tools.ApplyDurationLimits.MaximumDurationMilliseconds` |
| `src/ui/Features/Shared/PickVobSubLanguage/PickVobSubLanguageViewModel.cs:51` | `Pick VobSub language - {0}` | localized | `$.main.pickVobSubLanguageTitle` via `Se.Language.Main.PickVobSubLanguageTitle` |
| `src/ui/Features/Main/Layout/InitMenu.cs:214` | `IMSC 1.1 image profile` | technical-exception | Format/profile identifier must remain interoperable and verbatim. |
| `src/ui/Features/Main/Layout/InitMenu.cs:254` | `DOST/png` | technical-exception | Format/profile identifier must remain interoperable and verbatim. |
| `src/ui/Features/Main/Layout/InitMenu.cs:264` | `Final Cut Pro + image` | technical-exception | Format/profile identifier must remain interoperable and verbatim. |
| `src/ui/Features/Main/Layout/InitMenu.cs:289` | `WebVTT png` | technical-exception | Format/profile identifier must remain interoperable and verbatim. |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditWindow.cs:204` | `IMSC 1.1 image profile` | technical-exception | Format/profile identifier must remain interoperable and verbatim. |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditWindow.cs:209` | `DOST/png` | technical-exception | Format/profile identifier must remain interoperable and verbatim. |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditWindow.cs:214` | `Final Cut Pro + image` | technical-exception | Format/profile identifier must remain interoperable and verbatim. |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditWindow.cs:234` | `WebVTT png` | technical-exception | Format/profile identifier must remain interoperable and verbatim. |
| `src/ui/Features/Edit/MultipleReplace/CsvExporter.cs:13` | `Category,Find,ReplaceWith,Description,Active,Type` | non-ui | CSV header is a machine-readable export schema, not display copy. |
| `src/ui/Features/Shared/PickMp4Track/PickMp4TrackWindow.cs:98` | `HandlerName` | external-runtime | Media metadata column/property identifier comes from the external runtime contract. |
| `src/ui/Features/Shared/PickMp4Track/PickMp4TrackWindow.cs:105` | `Name` | external-runtime | Media metadata column/property identifier comes from the external runtime contract. |
| `src/ui/Features/Shared/PickMp4Track/PickMp4TrackWindow.cs:112` | `Duration` | external-runtime | Media metadata column/property identifier comes from the external runtime contract. |
| `src/ui/Features/Shared/PickMp4Track/PickMp4TrackWindow.cs:119` | `IsVobSubSubtitle` | external-runtime | Media metadata column/property identifier comes from the external runtime contract. |
| `src/ui/Features/Shared/PickMp4Track/PickMp4TrackWindow.cs:126` | `StartPosition` | external-runtime | Media metadata column/property identifier comes from the external runtime contract. |
| `src/ui/Features/Shared/PickMp4Track/PickMp4TrackWindow.cs:160` | `#` | external-runtime | Media metadata column/property identifier comes from the external runtime contract. |
| `src/ui/Features/Shared/PickMp4Track/PickMp4TrackWindow.cs:167` | `Show` | external-runtime | Media metadata column/property identifier comes from the external runtime contract. |
| `src/ui/Features/Shared/PickMp4Track/PickMp4TrackWindow.cs:174` | `Duration` | external-runtime | Media metadata column/property identifier comes from the external runtime contract. |
| `src/ui/Features/Shared/PickMp4Track/PickMp4TrackWindow.cs:181` | `Text/Image` | external-runtime | Media metadata column/property identifier comes from the external runtime contract. |
| `src/ui/Features/Main/MainView.cs:39` | `MainViewModel is not registered in the service provider.` | technical-exception | Exception text is diagnostic output, not user-facing UI copy. |
| `src/ui/Features/Main/Layout/InitVideoPlayer.cs:151` | `Failed to create video player control.` | technical-exception | Exception text is diagnostic output, not user-facing UI copy. |
| `src/ui/Features/Main/MainViewModel.cs:1209` | `index` | non-ui | Documentation route identifier is passed to UiUtil.ShowHelp and is not displayed as UI copy. |
| `src/ui/Features/Sync/VisualSync/VisualSyncViewModel.cs:573` | `features/visual-sync` | non-ui | Documentation route identifier is passed to UiUtil.ShowHelp and is not displayed as UI copy. |
| `src/ui/Features/Files/Statistics/StatisticsViewModel.cs:707` | `features/statistics` | non-ui | Documentation route identifier is passed to UiUtil.ShowHelp and is not displayed as UI copy. |
| `src/ui/Features/Edit/Replace/ReplaceViewModel.cs:156` | `features/edit` | non-ui | Documentation route identifier is passed to UiUtil.ShowHelp and is not displayed as UI copy. |
| `src/ui/Features/Edit/Replace/ReplaceViewModel.cs:156` | `replace` | non-ui | Documentation route identifier is passed to UiUtil.ShowHelp and is not displayed as UI copy. |
| `src/ui/Features/Sync/VisualSync/ManualSyncViewModel.cs:50` | `features/change-speed` | non-ui | Documentation route identifier is passed to UiUtil.ShowHelp and is not displayed as UI copy. |
| `src/ui/Features/Sync/PointSyncViaOther/PointSyncViaOtherViewModel.cs:308` | `features/point-sync-via-other` | non-ui | Documentation route identifier is passed to UiUtil.ShowHelp and is not displayed as UI copy. |
| `src/ui/Features/Edit/MultipleReplace/MultipleReplaceViewModel.cs:969` | `features/edit` | non-ui | Documentation route identifier is passed to UiUtil.ShowHelp and is not displayed as UI copy. |
| `src/ui/Features/Sync/PointSync/PointSyncViewModel.cs:143` | `features/point-sync` | non-ui | Documentation route identifier is passed to UiUtil.ShowHelp and is not displayed as UI copy. |
| `src/ui/Features/Sync/ChangeSpeed/ChangeSpeedViewModel.cs:155` | `features/change-speed` | non-ui | Documentation route identifier is passed to UiUtil.ShowHelp and is not displayed as UI copy. |
| `src/ui/Features/Sync/AdjustAllTimes/AdjustAllTimesViewModel.cs:224` | `features/adjust-all-times` | non-ui | Documentation route identifier is passed to UiUtil.ShowHelp and is not displayed as UI copy. |
| `src/ui/Features/Sync/ChangeFrameRate/ChangeFrameRateViewModel.cs:160` | `features/change-frame-rate` | non-ui | Documentation route identifier is passed to UiUtil.ShowHelp and is not displayed as UI copy. |
| `src/ui/Features/Edit/Find/FindViewModel.cs:143` | `features/edit` | non-ui | Documentation route identifier is passed to UiUtil.ShowHelp and is not displayed as UI copy. |
| `src/ui/Features/Edit/Find/FindViewModel.cs:143` | `find` | non-ui | Documentation route identifier is passed to UiUtil.ShowHelp and is not displayed as UI copy. |
| `src/ui/Features/Files/Compare/CompareViewModel.cs:854` | `features/compare` | non-ui | Documentation route identifier is passed to UiUtil.ShowHelp and is not displayed as UI copy. |
| `src/ui/Features/Main/MainHelpers/SubtitleFileService.cs:85` | `No subtitle found` | localized | `$.main.noSubtitleFound` via `Se.Language.Main.NoSubtitleFound` |
| `src/ui/Features/Main/MainHelpers/SubtitleFileService.cs:86` | `The Matroska file does not seem to contain any subtitles.` | localized | `$.main.matroskaContainsNoSubtitles` via `Se.Language.Main.MatroskaContainsNoSubtitles` |
| `src/ui/Features/Main/MainViewModel.cs:3551` | `Could not extract audio clip from video.` | localized | `$.main.couldNotExtractAudioClipFromVideo` via `Se.Language.Main.CouldNotExtractAudioClipFromVideo` |
| `src/ui/Features/Main/MainViewModel.cs:6963` | `Turn SMPTE timing off?` | localized | `$.main.turnSmpteTimingOff` via `Se.Language.Main.TurnSmpteTimingOff` |
| `src/ui/Features/Main/MainViewModel.cs:15718` | `This file seems to be an .mp3 audio file which does not contain subtitles.` | localized | `$.main.mp3ContainsNoSubtitles` via `Se.Language.Main.Mp3ContainsNoSubtitles` |
| `src/ui/Features/Main/MainViewModel.cs:15720` | `You can open media files via the Video menu.` | localized | `$.main.openMediaViaVideoMenu` via `Se.Language.Main.OpenMediaViaVideoMenu` |
| `src/ui/Features/Main/MainViewModel.cs:15727` | `This file seems to be a .wav audio file which does not contain subtitles.` | localized | `$.main.wavContainsNoSubtitles` via `Se.Language.Main.WavContainsNoSubtitles` |
| `src/ui/Features/Main/MainViewModel.cs:15729` | `You can open media files via the Video menu.` | localized | `$.main.openMediaViaVideoMenu` via `Se.Language.Main.OpenMediaViaVideoMenu` |
| `src/ui/Features/Main/MainViewModel.cs:17809` | `Download mpv?` | localized | `$.main.downloadMpvTitle` via `Se.Language.Main.DownloadMpvTitle` |
| `src/ui/Features/Main/MainViewModel.cs:17810` | `Subtitle Edit requires mpv to play video/audio.` | localized | `$.main.downloadMpvQuestion` via `Se.Language.Main.DownloadMpvQuestion` |
| `src/ui/Features/Files/ExportImageBased/ExportImageBasedViewModel.cs:338` | `Delete lines?` | localized | `$.file.deleteLinesTitle` via `Se.Language.File.DeleteLinesTitle` |
| `src/ui/Features/Files/ExportImageBased/ExportImageBasedViewModel.cs:339` | `Do you want to delete {0} lines?` | localized | `$.file.deleteXLinesQuestion` via `Se.Language.File.DeleteXLinesQuestion` |
| `src/ui/Features/Files/ExportImageBased/ImageBasedProfileViewModel.cs:86` | `Error` | localized | `$.general.error` via `Se.Language.General.Error` |
| `src/ui/Features/Files/ExportImageBased/ImageBasedProfileViewModel.cs:87` | `Please enter a profile name` | localized | `$.file.enterProfileNameMessage` via `Se.Language.File.EnterProfileNameMessage` |
| `src/ui/Features/Files/ExportImageBased/ImageBasedProfileViewModel.cs:97` | `Error` | localized | `$.general.error` via `Se.Language.General.Error` |
| `src/ui/Features/Files/ExportImageBased/ImageBasedProfileViewModel.cs:98` | `Profile name {0} can only be used once. Please choose a different name.` | localized | `$.file.profileNameMustBeUnique` via `Se.Language.File.ProfileNameMustBeUnique` |
| `src/ui/Features/Files/ImportImages/ImportImagesViewModel.cs:96` | `Remove image?` | localized | `$.file.removeImageTitle` via `Se.Language.File.RemoveImageTitle` |
| `src/ui/Features/Files/ImportImages/ImportImagesViewModel.cs:97` | `Do you want to remove {0}?` | localized | `$.file.removeImageQuestion` via `Se.Language.File.RemoveImageQuestion` |
| `src/ui/Features/Files/ImportPlainText/ImportPlainTextViewModel.cs:323` | `Alignment matched {0} of {1} lines.` | localized | `$.file.alignmentMatchedXOfYLines` via `Se.Language.File.AlignmentMatchedXOfYLines` |
| `src/ui/Features/Edit/MultipleReplace/CategoryExportViewModel.cs:51` | `No rule categories selected for export` | localized | `$.edit.multipleReplace.noRuleCategoriesSelectedForExport` via `Se.Language.Edit.MultipleReplace.NoRuleCategoriesSelectedForExport` |
| `src/ui/Features/Edit/MultipleReplace/MultipleReplaceViewModel.cs:566` | `Unable to import replace rules: ` | localized | `$.edit.multipleReplace.unableToImportReplaceRules` via `Se.Language.Edit.MultipleReplace.UnableToImportReplaceRules` |
| `src/ui/Features/Edit/MultipleReplace/MultipleReplaceViewModel.cs:578` | `No replace rules found in file` | localized | `$.edit.multipleReplace.noReplaceRulesFoundInFile` via `Se.Language.Edit.MultipleReplace.NoReplaceRulesFoundInFile` |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:731` | `Image-based subtitle format was not found or is not supported.` | localized | `$.tools.imageBasedEdit.formatNotFoundOrSupported` via `Se.Language.Tools.ImageBasedEdit.FormatNotFoundOrSupported` |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:875` | `Encrypted VobSub subtitles are not supported.` | localized | `$.tools.imageBasedEdit.encryptedVobSubNotSupported` via `Se.Language.Tools.ImageBasedEdit.EncryptedVobSubNotSupported` |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:1441` | `No subtitles to resize.` | localized | `$.tools.imageBasedEdit.noSubtitlesToResize` via `Se.Language.Tools.ImageBasedEdit.NoSubtitlesToResize` |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:1505` | `No subtitles to adjust.` | localized | `$.tools.imageBasedEdit.noSubtitlesToAdjust` via `Se.Language.Tools.ImageBasedEdit.NoSubtitlesToAdjust` |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:1569` | `No subtitles to adjust.` | localized | `$.tools.imageBasedEdit.noSubtitlesToAdjust` via `Se.Language.Tools.ImageBasedEdit.NoSubtitlesToAdjust` |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:1633` | `No subtitles to adjust.` | localized | `$.tools.imageBasedEdit.noSubtitlesToAdjust` via `Se.Language.Tools.ImageBasedEdit.NoSubtitlesToAdjust` |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:2215` | `Unable to load image file.` | localized | `$.tools.imageBasedEdit.unableToLoadImageFile` via `Se.Language.Tools.ImageBasedEdit.UnableToLoadImageFile` |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:2229` | `Failed to import image: {0}` | localized | `$.tools.imageBasedEdit.failedToImportImage` via `Se.Language.Tools.ImageBasedEdit.FailedToImportImage` |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:2245` | `No subtitle selected` | localized | `$.tools.imageBasedEdit.noSubtitleSelected` via `Se.Language.Tools.ImageBasedEdit.NoSubtitleSelected` |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:2245` | `Please select exactly one subtitle.` | localized | `$.tools.imageBasedEdit.selectExactlyOneSubtitle` via `Se.Language.Tools.ImageBasedEdit.SelectExactlyOneSubtitle` |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:2333` | `Image-based subtitle format was not found or is not supported.` | localized | `$.tools.imageBasedEdit.formatNotFoundOrSupported` via `Se.Language.Tools.ImageBasedEdit.FormatNotFoundOrSupported` |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:2340` | `No subtitles found in the file.` | localized | `$.tools.imageBasedEdit.noSubtitlesFoundInFile` via `Se.Language.Tools.ImageBasedEdit.NoSubtitlesFoundInFile` |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:2546` | `Unexported changes` | localized | `$.tools.imageBasedEdit.unexportedChangesTitle` via `Se.Language.Tools.ImageBasedEdit.UnexportedChangesTitle` |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:2547` | `You have unexported changes. Close and discard them?` | localized | `$.tools.imageBasedEdit.unexportedChangesQuestion` via `Se.Language.Tools.ImageBasedEdit.UnexportedChangesQuestion` |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:2755` | `Do you want to delete one line?` | localized | `$.tools.imageBasedEdit.deleteOneLineQuestion` via `Se.Language.Tools.ImageBasedEdit.DeleteOneLineQuestion` |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:2764` | `Do you want to delete {0} lines?` | localized | `$.tools.imageBasedEdit.deleteXLinesQuestion` via `Se.Language.Tools.ImageBasedEdit.DeleteXLinesQuestion` |
| `src/ui/Features/Shared/DownloadLibMpvViewModel.cs:100` | `Error` | localized | `$.general.error` via `Se.Language.General.Error` |
| `src/ui/Features/Shared/DownloadLibMpvViewModel.cs:101` | `Download complete, but could not delete existing file.` | localized | `$.main.downloadCompleteCouldNotDeleteExistingFile` via `Se.Language.Main.DownloadCompleteCouldNotDeleteExistingFile` |
| `src/ui/Features/Shared/DownloadLibMpvViewModel.cs:102` | `Please restart SE to use the new libmpv.` | localized | `$.main.restartSeToUseNewLibMpv` via `Se.Language.Main.RestartSeToUseNewLibMpv` |
| `src/ui/Features/Shared/GetAudioClips/GetAudioClipsViewModel.cs:97` | `Could not extract audio clip from video.` | localized | `$.main.couldNotExtractAudioClipFromVideo` via `Se.Language.Main.CouldNotExtractAudioClipFromVideo` |
| `src/ui/Features/Shared/PickMatroskaTrack/PickMatroskaTrackViewModel.cs:179` | `Format not supported: ` | localized | `$.file.formatNotSupportedPrefix` via `Se.Language.File.FormatNotSupportedPrefix` |

## Task 10 review-fix evidence

- Fixed scan roots are exactly `src/ui/Features/Main`, `Files`, `Edit`, `Sync`, and `Shared`.
- The structured inventory contains 1,566 rows: 1,511 localized, 10 technical exceptions, 9 external-runtime values, and 36 non-UI values. The non-UI set includes 18 language-section object aliases, one commented-out expression, and one internal AI prompt-protocol string; these are scanned but explicitly classified because they are not display strings. No deferred classification remains.
- All 43 formerly deferred user-visible candidates now resolve through typed `Se.Language` properties and matching English/Vietnamese catalog leaves.
- Markup scanning covers every English-bearing attribute and element-text node; namespace URIs, bindings, identifiers, resource keys, paths, type/style metadata, and other technical values are explicitly excluded.
- All 1,530 `Se.Language.*` member-expression occurrences participate in the same symmetric candidate/inventory cardinality check as retained literals; source columns distinguish repeated expressions on one line.
- Catalog ownership remains in the existing B1 (`file`/`edit`), B3 (`main`), and B4 (`tools`) shards under longest-prefix ownership.
