| Source | Location/key | Classification | Language key / reason |
|---|---|---|---|
| English catalog | `$.general.ok` | localized | `$.general.ok` |
| English catalog | `$.title`, `$.version`, `$.translatedBy`, `$.cultureName`, `$.general`, `$.file`, `$.edit`, `$.help`, `$.about` | localized | B1 reviewed by ChiBaoDev; complete reviewed shard in `tests/UI/TestData/VietnameseDraft/01-general-file-edit.json` with exact invariant values recorded in `VietnameseUntranslatedAllowlist.json` |
| English catalog | `$.main.menu`, `$.main.toolbar`, `$.main.waveform` | localized | B2 reviewed by ChiBaoDev; all 171 owned menu, toolbar, and waveform-navigation leaves are in `tests/UI/TestData/VietnameseDraft/02-main-navigation.json`; exact invariant retained in `VietnameseUntranslatedAllowlist.json` |
| English catalog | `$.main` excluding `$.main.menu`, `$.main.toolbar`, and `$.main.waveform`; plus `$.waveform`, `$.sync` | localized | B3 reviewed by ChiBaoDev; all 188 owned status, waveform-operation, timing, and synchronization leaves are in `tests/UI/TestData/VietnameseDraft/03-main-sync-waveform.json`; exact invariants recorded in `VietnameseUntranslatedAllowlist.json` |
| English catalog | `$.tools`, `$.spellCheck`, `$.options`, `$.plugins` | localized | B4 reviewed by ChiBaoDev; all 1,224 owned tool, spell-check, settings, shortcut, and plugin leaves are in `tests/UI/TestData/VietnameseDraft/04-tools-options.json`; 26 exact technical identifiers, formats, examples, units, standard modifier/category labels, and macOS symbols are recorded in `VietnameseUntranslatedAllowlist.json` |

8. English catalog | `$.video`, `$.ocr`, `$.assa` | localized | B5 reviewed by ChiBaoDev; all 607 owned video, media-processing, OCR, and Advanced SubStation Alpha leaves were re-reviewed path by path against the English extract in `tests/UI/TestData/VietnameseDraft/05-video-ocr-assa.json`; exact path-specific invariant identifiers and syntax are recorded in `VietnameseUntranslatedAllowlist.json` |

| `src/ui/Features/Main/Layout/LayoutWindow.cs:26` | `Choose layout` | localized | `$.main.layoutTitle`; resolved via `Se.Language.Main.LayoutTitle` |
| `src/ui/Features/Files/ExportPac/ExportPacWindow.cs:11,17` | `Export Pac`; `Choose PAC code page` | localized | `$.file.exportPacTitle`; `$.file.choosePacCodePage` |
| `src/ui/Features/Files/ExportCavena890/ExportCavena890Window.cs:13` | `Export Cavena 890` | localized | `$.file.exportCavena890Title` |
| `src/ui/Features/Files/ExportEbuStl/ExportEbuStlWindow.cs:16` | `Export EBU STL` | localized | `$.file.exportEbuStlTitle` |
| `src/ui/Features/Shared/DownloadFfmpegWindow.cs:15,24` | `Downloading ffmpeg` | localized | `$.main.downloadingFfmpeg` |
| `src/ui/Features/Shared/DownloadLibMpvWindow.cs:16` | `Downloading libmpv` | localized | `$.main.downloadingLibMpv` |
| `src/ui/Features/Shared/BinaryEdit/SetText/SetTextWindow.cs:18` | `Set Text` | localized | `$.tools.imageBasedEdit.setText` |
| `src/ui/Features/Shared/BinaryEdit/BinaryApplyDurationLimits/BinaryApplyDurationLimitsWindow.cs:44,66` | `Minimum/Maximum duration (milliseconds):` | localized | `$.tools.applyDurationLimits.minimumDurationMilliseconds`; `$.tools.applyDurationLimits.maximumDurationMilliseconds` |
| `src/ui/Features/Main/MainView.cs:39; Layout/InitVideoPlayer.cs:151` | diagnostic exception text | technical-exception | diagnostics preserved |
| `src/ui/Features/Edit/MultipleReplace/CsvExporter.cs:13` | CSV header identifiers | non-ui | schema identifiers preserved |
| `src/ui/Features/Shared/PickMp4Track/PickMp4TrackWindow.cs:98-181` | media metadata column identifiers | external-runtime | technical media metadata preserved |
