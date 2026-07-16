| Source | Location/key | Classification | Language key / reason |
|---|---|---|---|
| English catalog | `$.general.ok` | localized | `$.general.ok` |
| English catalog | `$.title`, `$.version`, `$.translatedBy`, `$.cultureName`, `$.general`, `$.file`, `$.edit`, `$.help`, `$.about` | localized | B1 reviewed by ChiBaoDev; complete reviewed shard in `tests/UI/TestData/VietnameseDraft/01-general-file-edit.json` with exact invariant values recorded in `VietnameseUntranslatedAllowlist.json` |
| English catalog | `$.main.menu`, `$.main.toolbar`, `$.main.waveform` | localized | B2 reviewed by ChiBaoDev; all 171 owned menu, toolbar, and waveform-navigation leaves are in `tests/UI/TestData/VietnameseDraft/02-main-navigation.json`; exact invariant retained in `VietnameseUntranslatedAllowlist.json` |
| English catalog | `$.main` excluding `$.main.menu`, `$.main.toolbar`, and `$.main.waveform`; plus `$.waveform`, `$.sync` | localized | B3 reviewed by ChiBaoDev; all 188 owned status, waveform-operation, timing, and synchronization leaves are in `tests/UI/TestData/VietnameseDraft/03-main-sync-waveform.json`; exact invariants recorded in `VietnameseUntranslatedAllowlist.json` |
| English catalog | `$.tools`, `$.spellCheck`, `$.options`, `$.plugins` | localized | B4 reviewed by ChiBaoDev; all 1,224 owned tool, spell-check, settings, shortcut, and plugin leaves are in `tests/UI/TestData/VietnameseDraft/04-tools-options.json`; 26 exact technical identifiers, formats, examples, units, standard modifier/category labels, and macOS symbols are recorded in `VietnameseUntranslatedAllowlist.json` |

| English catalog | `$.video`, `$.ocr`, `$.assa` | localized | B5 reviewed by ChiBaoDev; all 607 owned video, media-processing, OCR, and Advanced SubStation Alpha leaves were re-reviewed path by path against the English extract in `tests/UI/TestData/VietnameseDraft/05-video-ocr-assa.json`; exact path-specific invariant identifiers and syntax are recorded in `VietnameseUntranslatedAllowlist.json` |

| `src/ui/Features/Main/Layout/LayoutWindow.cs:26` | `Choose layout` | localized | `$.main.layoutTitle`; resolved via `Se.Language.Main.LayoutTitle` |
| `src/ui/Features/Files/ExportPac/ExportPacWindow.cs:11,17` | `Export Pac`; `Choose PAC code page` | localized | `$.file.exportPacTitle`; `$.file.choosePacCodePage` |
| `src/ui/Features/Files/ExportCavena890/ExportCavena890Window.cs:13` | `Export Cavena 890` | localized | `$.file.exportCavena890Title` |
| `src/ui/Features/Files/ExportEbuStl/ExportEbuStlWindow.cs:16` | `Export EBU STL` | localized | `$.file.exportEbuStlTitle` |
| `src/ui/Features/Shared/DownloadFfmpegWindow.cs:15,24` | `Downloading ffmpeg` | localized | `$.main.downloadingFfmpeg` |
| `src/ui/Features/Shared/DownloadLibMpvWindow.cs:16` | `Downloading libmpv` | localized | `$.main.downloadingLibMpv` |
| `src/ui/Features/Shared/BinaryEdit/SetText/SetTextWindow.cs:18` | `Set Text` | localized | `$.tools.imageBasedEdit.setText` |
| `src/ui/Features/Shared/BinaryEdit/BinaryApplyDurationLimits/BinaryApplyDurationLimitsWindow.cs:44,66` | `Minimum duration (milliseconds):`; `Maximum duration (milliseconds):` | localized | `$.tools.applyDurationLimits.minimumDurationMilliseconds`; `$.tools.applyDurationLimits.maximumDurationMilliseconds` |
| `src/ui/Features/Main/MainView.cs:39; Layout/InitVideoPlayer.cs:151` | diagnostic exception text | technical-exception | diagnostics preserved |
| `src/ui/Features/Edit/MultipleReplace/CsvExporter.cs:13` | CSV header identifiers | non-ui | schema identifiers preserved |
| `src/ui/Features/Shared/PickMp4Track/PickMp4TrackWindow.cs:98-181` | media metadata column identifiers | external-runtime | technical media metadata preserved |

## Task 10 fixed-scan inventory

| Source | Candidate | Classification | Language key | Resolution |
|---|---|---|---|---|
| `src/ui/Features/Main/Layout/InitMenu.cs` | `Header = "IMSC 1.1 image profile"`; `Header = "DOST/png"`; `Header = "Final Cut Pro + image"`; `Header = "WebVTT png"` | technical-exception | — | profile/format names retained verbatim |
| `src/ui/Features/Shared/BinaryEdit/BinaryEditWindow.cs` | `Header = "IMSC 1.1 image profile"`; `Header = "DOST/png"`; `Header = "Final Cut Pro + image"`; `Header = "WebVTT png"` | technical-exception | — | profile/format names retained verbatim |
| `src/ui/Features/Edit/MultipleReplace/CsvExporter.cs` | `Header = "Category,Find,ReplaceWith,Description,Active,Type"` | non-ui | — | CSV schema header retained verbatim |
| `src/ui/Features/Shared/PickMp4Track/PickMp4TrackWindow.cs` | `Header = "HandlerName"`; `Header = "Name"`; `Header = "Duration"`; `Header = "IsVobSubSubtitle"`; `Header = "StartPosition"`; `Header = "#"`; `Header = "Show"`; `Header = "Text/Image"` | external-runtime | — | media metadata/property identifiers retained verbatim |
| `src/ui/Features/Main/MainView.cs` | `throw new InvalidOperationException("MainViewModel is not registered in the service provider.")` | technical-exception | — | diagnostic exception retained verbatim |
| `src/ui/Features/Main/Layout/InitVideoPlayer.cs` | `throw new InvalidOperationException("Failed to create video player control.")` | technical-exception | — | diagnostic exception retained verbatim |

## Task 10 review-fix evidence

- Fixed-scan scope is exactly `src/ui/Features/Main`, `Files`, `Edit`, `Sync`, and `Shared`; scans were rerun with the three brief patterns. Results: 18 property/markup hits and 15 call/exception hits; all retained hits are listed in the fixed-scan table above, including `InitMenu.cs` and `BinaryEditWindow.cs`.
- Localized additions use `Se.Language`: `Main.PickVobSubLanguageTitle`, `File.ExportPacTitle`, `File.ChoosePacCodePage`, `File.ExportCavena890Title`, and `File.ExportEbuStlTitle`; PAC is rendered consistently as `PAC`.
- Deterministic merge: `C:\Windows\System32\WindowsPowerShell1.0\powershell.exe -NoProfile -ExecutionPolicy Bypass -File D:	oolsinhton\.claude\worktreesietnamese-localization	ools\localization\Merge-VietnameseLanguage.ps1` — PASS.
- Focused inventory/runtime tests: `dotnet test tests/UI/UITests.csproj -c Debug --filter FullyQualifiedName~FirstPartyUiLiteralInventoryTests --no-restore --verbosity minimal` — 2/2 PASS.
- Complete localization suite: `dotnet test tests/UI/UITests.csproj -c Debug --filter FullyQualifiedName~UITests.Logic.Localization --no-restore --verbosity minimal` — 90/90 PASS.
- Provenance for this review-fix pass: base `e5e738d9a` through the final commit recorded below; no Task 11 work, runtime-generated files, or push included.
