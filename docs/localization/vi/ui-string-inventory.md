| Source | Location/key | Classification | Language key / reason |
|---|---|---|---|
| English catalog | `$.general.ok` | localized | `$.general.ok` |
| English catalog | `$.title`, `$.version`, `$.translatedBy`, `$.cultureName`, `$.general`, `$.file`, `$.edit`, `$.help`, `$.about` | localized | B1 reviewed by ChiBaoDev; complete reviewed shard in `tests/UI/TestData/VietnameseDraft/01-general-file-edit.json` with exact invariant values recorded in `VietnameseUntranslatedAllowlist.json` |
| English catalog | `$.main.menu`, `$.main.toolbar`, `$.main.waveform` | localized | B2 reviewed by ChiBaoDev; all 171 owned menu, toolbar, and waveform-navigation leaves are in `tests/UI/TestData/VietnameseDraft/02-main-navigation.json`; exact invariant retained in `VietnameseUntranslatedAllowlist.json` |
| English catalog | `$.main` excluding `$.main.menu`, `$.main.toolbar`, and `$.main.waveform`; plus `$.waveform`, `$.sync` | localized | B3 reviewed by ChiBaoDev; all 188 owned status, waveform-operation, timing, and synchronization leaves are in `tests/UI/TestData/VietnameseDraft/03-main-sync-waveform.json`; exact invariants recorded in `VietnameseUntranslatedAllowlist.json` |
| English catalog | `$.tools`, `$.spellCheck`, `$.options`, `$.plugins` | localized | B4 reviewed by ChiBaoDev; all 1,224 owned tool, spell-check, settings, shortcut, and plugin leaves are in `tests/UI/TestData/VietnameseDraft/04-tools-options.json`; 26 exact technical identifiers, formats, examples, units, standard modifier/category labels, and macOS symbols are recorded in `VietnameseUntranslatedAllowlist.json` |

| English catalog | `$.video`, `$.ocr`, `$.assa` | localized | B5 reviewed by ChiBaoDev; all 607 owned video, media-processing, OCR, and Advanced SubStation Alpha leaves were re-reviewed path by path against the English extract in `tests/UI/TestData/VietnameseDraft/05-video-ocr-assa.json`; exact path-specific invariant identifiers and syntax are recorded in `VietnameseUntranslatedAllowlist.json` |

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

## Task 10 fixed-scan inventory

The structured source of truth is `tests/UI/TestData/Task10LiteralInventory.json`. Each current scanner candidate has one row below.

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
| `src/ui/Features/Main/MainHelpers/SubtitleFileService.cs:85` | `No subtitle found` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Main/MainHelpers/SubtitleFileService.cs:86` | `The Matroska file does not seem to contain any subtitles.` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Main/MainViewModel.cs:3551` | `Could not extract audio clip from video.` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Main/MainViewModel.cs:6963` | `Turn SMPTE timing off?` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Main/MainViewModel.cs:15718` | `This file seems to be an .mp3 audio file which does not contains subtitles.` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Main/MainViewModel.cs:15720` | `You can open media files via the Video menu.` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Main/MainViewModel.cs:15727` | `This file seems to be a .wav audio file which does not contains subtitles.` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Main/MainViewModel.cs:15729` | `You can open media files via the Video menu.` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Main/MainViewModel.cs:17809` | `Download mpv?` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Main/MainViewModel.cs:17810` | `{Environment.NewLine}\"Subtitle Edit\" requires mpv to play video/audio.{Environment.NewLine}{Environment.NewLine}Download and use mpv?` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Files/ExportImageBased/ExportImageBasedViewModel.cs:338` | `Delete lines?` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Files/ExportImageBased/ExportImageBasedViewModel.cs:339` | `Do you want to delete {selectedItems.Count} lines?` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Files/ExportImageBased/ImageBasedProfileViewModel.cs:86` | `Error` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Files/ExportImageBased/ImageBasedProfileViewModel.cs:87` | `Please enter a profile name` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Files/ExportImageBased/ImageBasedProfileViewModel.cs:97` | `Error` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Files/ExportImageBased/ImageBasedProfileViewModel.cs:98` | `Profile name '{profile.Name}' can only be used once. Please choose a different name.` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Files/ImportImages/ImportImagesViewModel.cs:96` | `Remove image?` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Files/ImportImages/ImportImagesViewModel.cs:97` | `Do you want to remove {selectedStyle.FileName}?` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Files/ImportPlainText/ImportPlainTextViewModel.cs:323` | `Alignment matched {result.MatchedLines} of {result.TotalLines} lines. ` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Edit/MultipleReplace/CategoryExportViewModel.cs:51` | `No rule categories selected for export` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Edit/MultipleReplace/MultipleReplaceViewModel.cs:566` | `Unable to import replace rules: ` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Edit/MultipleReplace/MultipleReplaceViewModel.cs:578` | `No replace rules found in file` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:731` | `Image based subtitle format not found/supported.` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:875` | `Encrypted VobSub subtitles are not supported.` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:1441` | `No subtitles to resize.` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:1505` | `No subtitles to adjust.` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:1569` | `No subtitles to adjust.` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:1633` | `No subtitles to adjust.` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:2215` | `Unable to load image file.` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:2229` | `Failed to import image: {ex.Message}` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:2245` | `No subtitle selected` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:2245` | `Please select exactly one subtitle.` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:2333` | `Image based subtitle format not found/supported.` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:2340` | `No subtitles found in the file.` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:2546` | `Unexported changes` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:2547` | `You have unexported changes. Close and discard them?` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:2755` | `Do you want to delete one line?` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditViewModel.cs:2764` | `Do you want to delete {selectedItems.Count} lines?` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Shared/DownloadLibMpvViewModel.cs:100` | `Error` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Shared/DownloadLibMpvViewModel.cs:101` | `Download complete, but could not delete existing file.` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Shared/DownloadLibMpvViewModel.cs:102` | `Please restart SE to use the new libmpv.` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Shared/GetAudioClips/GetAudioClipsViewModel.cs:97` | `Could not extract audio clip from video.` | deferred-localization | User-visible message candidate is retained for the next localization task. |
| `src/ui/Features/Shared/PickMatroskaTrack/PickMatroskaTrackViewModel.cs:179` | `Format not supported: ` | deferred-localization | User-visible message candidate is retained for the next localization task. |

## Task 10 review-fix evidence

- Fixed-scan scope is exactly `src/ui/Features/Main`, `Files`, `Edit`, `Sync`, and `Shared`; scans were rerun with the three brief patterns. The structured inventory contains 89 rows: 11 localized candidates and 78 retained candidates (including 43 deferred user-visible message candidates); exact bidirectional cardinality is enforced by the focused test.
- Localized additions use `Se.Language`: `Main.PickVobSubLanguageTitle`, `File.ExportPacTitle`, `File.ChoosePacCodePage`, `File.ExportCavena890Title`, and `File.ExportEbuStlTitle`; PAC is rendered consistently as `PAC`.
- Deterministic merge: `C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -NoProfile -ExecutionPolicy Bypass -File D:\toolsinhton\.claude\worktrees\vietnamese-localization\tools\localization\Merge-VietnameseLanguage.ps1` — PASS.
- Focused inventory/runtime tests: `dotnet test tests/UI/UITests.csproj -c Debug --filter FullyQualifiedName~FirstPartyUiLiteralInventoryTests --no-restore --verbosity minimal` — 4/4 PASS, including headless `PickVobSubLanguageViewModel.Initialize` plus window construction and the report/inventory C0-tab audit.
- Complete localization suite: `dotnet test tests/UI/UITests.csproj -c Debug --filter FullyQualifiedName~UITests.Logic.Localization --no-restore --verbosity minimal` — 92/92 PASS.
- Provenance for this review-fix pass: base `e5e738d9a` through the final commit recorded below; no Task 11 work, runtime-generated files, or push included.
