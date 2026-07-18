| English | Vietnamese | Context / invariant | Capitalization |
|---|---|---|---|
| Subtitle | Phụ đề | Generic UI noun | Sentence case |
| OCR | OCR | Technical acronym; keep unchanged | Uppercase |
| Whisper | Whisper | Engine/model family; keep unchanged | Product spelling |
| Save as | Lưu thành | File operation | Sentence case |
| File | Tệp | Filesystem object in UI | Sentence case |
| Time code | Mã thời gian | Subtitle timing | Sentence case |
| Shot change | Điểm chuyển cảnh | Video scene boundary | Sentence case |
| Frame | Khung hình | Video timing unit | Sentence case |
| Encoding | Mã hóa | Character encoding | Sentence case |
| Codec | Codec | Established technical term; keep spelling | Sentence case |
| Speech to text | Chuyển giọng nói thành văn bản | Transcription feature | Sentence case |
| Transcription | Phiên âm | Speech-recognition output/process | Sentence case |
| Waveform | Dạng sóng | Audio visualization | Sentence case |
| Regular expression | Biểu thức chính quy | Search and replace | Sentence case |
| Case-sensitive | Phân biệt chữ hoa chữ thường | Search matching option | Sentence case |
| Placeholder | Phần giữ chỗ | .NET composite-format content | Sentence case |
| Teletext | Teletext | Broadcast subtitle technology; keep identifier | Product spelling |
| Frame rate | Tốc độ khung hình | Video timing | Sentence case |
| Burned-in subtitle | Phụ đề nhúng cứng | Video rendering / OCR; distinguish from embedding a selectable subtitle track | Sentence case |
| Visual sync | Đồng bộ bằng hình ảnh | Synchronization using waveform/video comparison | Sentence case |
| Seek | Tua | Waveform and video navigation | Sentence case |
| Audio track | Rãnh âm thanh | Selectable media audio stream | Sentence case |
| Spectrogram | Phổ âm | Frequency-domain audio visualization; distinguish from waveform | Sentence case |
| Sync point | Điểm đồng bộ | Point-based subtitle synchronization marker | Sentence case |
| Offset | Độ lệch | Timing displacement; use `dịch` for the command that moves timing | Sentence case |
| Playback speed | Tốc độ phát lại | Media playback rate; distinguish from subtitle timing speed factor | Sentence case |
| Drop-frame | Drop-frame | SMPTE time-code counting convention; keep technical term | Lowercase in prose |
| Spell check | Kiểm tra chính tả | Spell-check operation and feature | Sentence case |
| User dictionary | Từ điển người dùng | User-managed spell-check words; distinguish from names list | Sentence case |
| Names list | Danh sách tên riêng | Proper-name list used by spell checking | Sentence case |
| Settings | Cài đặt | Application configuration area | Sentence case |
| Option | Tùy chọn | Individual configurable choice | Sentence case |
| Preferences | Tùy chỉnh | User preference configuration where the source specifically says preferences | Sentence case |
| Plugin | Trình bổ trợ | Installable application plugin; distinguish from file-name extension | Sentence case |
| Extension | Phần mở rộng | File-name suffix or extension; not an application plugin | Sentence case |
| Engine | Bộ máy | Processing implementation such as AI, rendering, or spell-check engine | Sentence case |
| Model | Mô hình | AI or speech-recognition model; distinguish from engine and service | Sentence case |
| Service | Dịch vụ | Remote or operating-system service | Sentence case |
| Executable | Tệp thực thi | Runnable program file; distinguish from folder and path | Sentence case |
| Command line | Dòng lệnh | Executable invocation and arguments | Sentence case |
| Folder | Thư mục | Filesystem container; distinguish from file and path | Sentence case |
| Path | Đường dẫn | Filesystem location string | Sentence case |
| Casing | Kiểu chữ hoa/thường | Text capitalization transformation | Sentence case |
| Gap | Khoảng cách | Timing interval between subtitle cues | Sentence case |
| Duration | Thời lượng | Length of a cue, audio, or video interval | Sentence case |
| Cue | Mục phụ đề | Timed subtitle unit when cue semantics are explicit | Sentence case |
| FFmpeg | FFmpeg | Media-processing framework identifier; keep unchanged | Product spelling |
| libmpv | libmpv | Media playback library identifier; keep unchanged | Product spelling |
| Matroska | Matroska | Container-format identifier; keep unchanged | Product spelling |
| PaddleOCR | PaddleOCR | OCR engine identifier; keep unchanged | Product spelling |
| Tesseract | Tesseract | OCR engine identifier; keep unchanged | Product spelling |
| ASSA | ASSA | Advanced SubStation Alpha format identifier; keep unchanged | Uppercase |
| Advanced SubStation Alpha | Advanced SubStation Alpha | Keep the full name unchanged when it identifies the ASSA format; translate descriptive prose around it naturally | Product spelling |
| Override tag | Thẻ điều khiển | ASSA formatting/control tag | Sentence case |
| Position tag | Thẻ vị trí | ASSA positioning control tag | Sentence case |
| Burn in | Nhúng cứng | Render subtitles into video pixels | Sentence case |
| OCR language | Ngôn ngữ OCR | Language selected for optical character recognition | Sentence case |
| WebVTT | WebVTT | Subtitle format identifier; keep unchanged | Product spelling |
| VAD | VAD | Voice-activity-detection identifier; keep unchanged | Uppercase |
| TTS | TTS | Text-to-speech identifier; keep unchanged | Uppercase |

## Task 14 Windows verification notes

- Use `phụ đề nhúng cứng` consistently for burned-in subtitles and `nhúng cứng` for the burn-in operation. Do not use `phụ đề ghi cứng`.
- Use `Dạng sóng/phổ âm` when a single control or setting names both waveform and spectrogram views.
- Settings labels use sentence case, including `Phông chữ giao diện người dùng`, `Chủ đề`, and `Tệp và nhật ký`.
- Burned-in-subtitle and audio-visualization decisions are covered by `ReviewedTerminology_UsesGlossaryStandardBurnedInSubtitleAndAudioVisualizationTerms`; all listed terminology and capitalization decisions were applied to the reviewed shards and generated catalog in commit `12efe61a7`.
- Genuine Vietnamese language review was approved by the user on 2026-07-18 for commit `12efe61a7`; the verification checklist records the approved scope and remaining blocked and not-run rows.
