# Vietnamese Windows UI verification checklist

Task 14 target: Windows 11, Subtitle Edit v5.1.0-rc5. `PASS` means observed on the real application. `PARTIAL`, `BLOCKED`, and `NOT RUN` are not treated as passing. All disposable profiles, screenshots, logs, and generated binaries remained outside the repository.

## Automated gates after polish

| Gate | Result | Exact result |
|---|---|---|
| Restore | PASS | 2 projects restored; 6 of 8 up-to-date |
| Debug build | PASS | 0 errors; 1 pre-existing CS8600 warning at `tests/libse/SubtitleFormats/EbuTtDTest.cs:103` |
| Debug UI tests | PASS | 832 passed, 0 failed, 0 skipped |
| Release build | PASS | 0 errors; same pre-existing warning |
| Release UI tests | PASS | 832 passed, 0 failed, 0 skipped |
| Release solution tests | PASS | 1,579 passed, 0 failed, 0 skipped: 11 + 499 + 237 + 832 |
| Accepted Inno Setup 6.7.3 compiler | PASS | The user accepted `%LOCALAPPDATA%\Programs\Inno Setup 6\ISCC.exe` in place of the absent Program Files path; it compiled the production installer successfully with the existing admin/per-user-area warning only |

## Language lifecycle

| Mode | DPI | Size | Input | Action | Expected result | Actual result | Evidence path | Defect ID | Result |
|---|---:|---:|---|---|---|---|---|---|---|
| Portable | Host 125% | 1920x1080 physical | Fresh disposable profile | Start without settings | Vietnamese is active before the main window appears | First captured main window was `Không có tiêu đề - Subtitle Edit v5.1.0-rc5`; pre-main-window timing was not instrumented, so earlier transient UI was not verified | `%TEMP%\SE14-portable-tmpr9t_q\startup.png` | - | PARTIAL |
| Portable | Host 125% | 1920x1080 physical | Vietnamese profile | Switch Vietnamese → English | Live UI and persisted setting become English | English title; `General.Language=English` | Disposable profile | - | PASS |
| Portable | Host 125% | 1920x1080 physical | English profile | Restart | English persists | Initialized in English | Disposable profile | - | PASS |
| Portable | Host 125% | 1920x1080 physical | English profile | Switch English → Vietnamese | Live UI and persisted setting become Vietnamese | Vietnamese title; `General.Language=Vietnamese` | Disposable profile | - | PASS |
| Portable | Host 125% | 1920x1080 physical | Vietnamese profile | Restart | Vietnamese persists | Initialized in Vietnamese | Disposable profile | - | PASS |
| Portable | Host 125% | 1920x1080 physical | Malformed `RollbackProbe.json` without embedded counterpart | Attempt live switch | Failed switch preserves prior UI and setting | UI and setting remained Vietnamese | Disposable profile | - | PASS |
| Portable | Host 125% | 1920x1080 physical | Corrupt writable `Vietnamese.json` | Start | Embedded Vietnamese fallback loads | Initialized in Vietnamese | Disposable profile | - | PASS |
| Portable | Host 125% | 1920x1080 physical | Corrupt writable `Vietnamese.json` while another language is active | Select Vietnamese live | Embedded Vietnamese fallback loads without losing prior state | Live-selection variant was not separately observed | - | BLOCK-ENV-03 | NOT RUN |
| Portable | Host 125% | 1920x1080 physical | Writable Vietnamese removed | Start | Embedded Vietnamese fallback loads | Initialized in Vietnamese | Disposable profile | - | PASS |
| Installed | Required matrix | Required matrix | All required lifecycle inputs | Run all lifecycle scenarios | Same behavior using installed profile | Not run: installed-mode isolation still requires a UAC-authorized Program Files copy and disposable Windows user; real `%APPDATA%` was not touched | Environment investigation | BLOCK-ENV-01 | BLOCKED |

## Runtime flow rows

| Mode | DPI | Size | Input | Action | Expected result | Actual result | Evidence path | Defect ID | Result |
|---|---:|---:|---|---|---|---|---|---|---|
| Portable | Host 125% | 1920x1080 physical | `test.srt` | Open subtitle | Subtitle loads in Vietnamese UI | Title became `test.srt - Subtitle Edit v5.1.0-rc5`; grid and edit controls appeared | UIA inspection | - | PASS |
| Portable | Host 125% | 1920x1080 physical | `sample_MP4.mp4` | Open video with bundled libmpv | Media initializes | Video layout and `Vị trí video` appeared; no crash or download prompt | `%TEMP%\SE14-polish-check\video-loaded.png` | - | PASS |
| Portable | Host 125% | 1920x1080 physical | Help menu | Expose Help and About commands | Localized commands are accessible | `Kiểm tra bản cập nhật...`, `Trợ giúp...`, and `Giới thiệu...` were exposed | UIA inspection | - | PASS |
| Portable | 100% | 1920x1080 | Disposable `input.srt` and local MP4 | Start application and exercise attempted shortcuts/playback | Vietnamese UI starts without clipping or crash; requested flows open | Application started and remained alive; screenshot captured. Shortcut-driven dialogs were not reliably detected, so find, replace, spell-check, synchronization, translation, and Save As are not claimed as passed | `%TEMP%\SE14-matrix-evidence\100-1920x1080 (Recommended).png` | BLOCK-ENV-03 | PARTIAL |
| Portable | 150% | 1920x1080 | Disposable `input.srt` and local MP4 | Start application and exercise attempted shortcuts/playback | Vietnamese UI starts without clipping or crash; requested flows open | Application started and remained alive; Vietnamese top-level menus were exposed and screenshot captured. Shortcut-driven dialogs were not reliably detected, so the remaining flows are not claimed as passed | `%TEMP%\SE14-matrix-evidence\150-1920x1080 (Recommended).png` | BLOCK-ENV-03 | PARTIAL |
| Portable | Required points | Required points | Disposable subtitle/media | Save, Save As, edit, find, replace, spell-check, synchronization, waveform, playback, OCR, speech-to-text, machine translation, export, settings/options, error dialog, About, Help | Each flow completes and remains usable | Save was invoked on a disposable subtitle; open/video and Help/About exposure passed separately. The other end-to-end completions could not be observed reliably after UI Automation stopped exposing an active interactive desktop; no private data was sent | External disposable profiles only | BLOCK-ENV-03 | PARTIAL |
| Installed | Required points | Required points | Required subtitle/media/dialog inputs | Run every required flow | Each flow completes | Not run with installed profile | Environment investigation | BLOCK-ENV-01 | BLOCKED |
| Installer | Host | Host | Production installer source | Compile normal path | Compiler succeeds | Accepted per-user Inno Setup 6.7.3 compiler succeeded | Compiler output; generated binary excluded from Git | - | PASS |
| Installer | Host | Host | Forced missing-.NET conditions | Exercise warning | Correct Vietnamese/English/fallback text; No aborts; Yes opens exact URL | Task 13 verified all language paths and `https://dotnet.microsoft.com/download/dotnet/10.0` | `.superpowers/sdd/task-13-report.md` | - | PASS |

## Required display matrix

| Mode | DPI | Size | Input | Action | Expected result | Actual result | Evidence path | Defect ID | Result |
|---|---:|---:|---|---|---|---|---|---|---|
| Portable | 100% | 1280x720 | Disposable subtitle/media profile | Start and run required flows | Localized UI and controls remain usable without important clipping | Authorized display change was attempted. Settings list selection first failed because the target scale item was not exposed as a visible UIA fragment; a direct-resolution retry then lost access to an active interactive desktop. No application row was observed | `%TEMP%\SE14-matrix-evidence\low-resolution-results.json` if produced; automation trace | BLOCK-ENV-02 | BLOCKED |
| Portable | 100% | 1920x1080 | Disposable `input.srt` and local MP4 | Start and inspect | Localized UI and controls remain usable | Main window launched and survived; full flow set was not observed | `%TEMP%\SE14-matrix-evidence\100-1920x1080 (Recommended).png` | BLOCK-ENV-03 | PARTIAL |
| Portable | 150% | 1280x720 | Disposable subtitle/media profile | Start and run required flows | Localized UI and controls remain usable without important clipping | Authorized display change was attempted, but the 1280x720 item was not exposed as a visible UIA fragment; the later direct-resolution retry could not drive the inactive desktop. No application row was observed | Automation trace | BLOCK-ENV-02 | BLOCKED |
| Portable | 150% | 1920x1080 | Disposable `input.srt` and local MP4 | Start and inspect | Localized UI and controls remain usable | Main window launched and survived; Vietnamese menus were exposed; full flow set was not observed | `%TEMP%\SE14-matrix-evidence\150-1920x1080 (Recommended).png` | BLOCK-ENV-03 | PARTIAL |
| Installed | 100% | 1280x720 | Installed disposable profile | Run required lifecycle and flows | All required behavior passes | Not run | Environment investigation | BLOCK-ENV-01 | BLOCKED |
| Installed | 100% | 1920x1080 | Installed disposable profile | Run required lifecycle and flows | All required behavior passes | Not run | Environment investigation | BLOCK-ENV-01 | BLOCKED |
| Installed | 150% | 1280x720 | Installed disposable profile | Run required lifecycle and flows | All required behavior passes | Not run | Environment investigation | BLOCK-ENV-01 | BLOCKED |
| Installed | 150% | 1920x1080 | Installed disposable profile | Run required lifecycle and flows | All required behavior passes | Not run | Environment investigation | BLOCK-ENV-01 | BLOCKED |

The authorized display attempts restored the host to its original physical 1920x1080 and 125% scaling. Final checks reported `EnumDisplaySettingsW` 1920x1080 and system DPI 120 (125%). No 1280x720 row is represented as passed.

## Defects and blockers

| ID | Type | Finding | Disposition |
|---|---|---|---|
| VI-14-01 | P2 | `phụ đề ghi cứng` conflicted with glossary term `phụ đề nhúng cứng` | CLOSED in `12efe61a7`; focused regression added |
| VI-14-02 | P3 | Waveform/spectrogram and capitalization inconsistency | CLOSED in `12efe61a7` |
| BLOCK-ENV-01 | Verification | Installed-mode isolation unavailable without a UAC-authorized Program Files copy and disposable Windows user | OPEN; installed rows honestly blocked/not run |
| BLOCK-ENV-02 | Verification | The active Windows desktop became unavailable to mouse/UI Automation while completing the authorized 1280x720 rows | OPEN; display changes were attempted, but application rows were not completed or observed |
| BLOCK-ENV-03 | Verification | Avalonia duplicate UIA fragments and loss of an active interactive desktop prevented reliable observation of the remaining end-to-end dialogs | OPEN; unobserved flows not inferred as passing |

Portable lifecycle evidence is complete except for the corrupt-writable-catalog live-selection variant, which remains not run. No discovered product P0/P1/P2 defect remains open. Task 14 is still incomplete because required installed, 1280x720, lifecycle live-selection, and end-to-end flow rows remain blocked or not run.

## Human sign-off

The user approved the Vietnamese language review on **2026-07-18**, covering the glossary decisions, translation polish, and observed checklist evidence at commit `12efe61a7`.

Status: **APPROVED**
