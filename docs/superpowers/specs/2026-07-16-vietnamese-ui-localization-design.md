# Thiết kế Việt hóa toàn bộ giao diện Subtitle Edit

**Ngày:** 2026-07-16  
**Nhánh đích:** `tintutien`  
**Trạng thái:** Chờ người dùng duyệt tài liệu

## 1. Mục tiêu

Cung cấp giao diện tiếng Việt đầy đủ, tự nhiên và nhất quán cho mọi bề mặt tương tác first-party do Subtitle Edit kiểm soát. Trên mọi nền tảng, hồ sơ cấu hình được tạo mới bởi bản fork này ưu tiên tiếng Việt ngay lần chạy đầu; người dùng vẫn có thể đổi sang ngôn ngữ khác và lựa chọn đó được lưu lại.

Công việc được chia thành các giai đoạn phụ thuộc tuần tự. Mỗi commit phải build và test xanh; “độc lập” ở đây nghĩa là dễ review/hoàn tác, không có nghĩa các commit sau không phụ thuộc commit trước.

## 2. Hiện trạng đã xác minh

- Cây lớp C# dưới `src/ui/Logic/Config/Language/` định nghĩa schema và fallback runtime. `src/ui/Assets/Languages/English.json` là baseline dữ liệu để kiểm tra parity, không phải nguồn schema runtime duy nhất.
- Runtime định danh locale UI bằng **tên file không phần mở rộng**, lưu tại `Se.Settings.General.Language` (ví dụ `English`, sau này là `Vietnamese`). `cultureName` chỉ là metadata trong JSON; locale tiếng Việt dùng `vi-VN`.
- `LanguageInitializer.LanguageFiles`, thư mục asset và các `AvaloniaResource` trong `UI.csproj` hiện bị drift. Vietnamese là một trường hợp thiếu asset; Basque, Estonian, Farsi, Slovak và Thai cũng được liệt kê nhưng không có asset tương ứng. `Unpack()` bỏ qua exception nên che giấu drift này.
- Hướng dẫn `docs/translating.md` dùng quy ước `{CultureName}.json`, nhưng locale tích hợp hiện chọn theo tên file. Bản tích hợp của dự án sẽ là `Vietnamese.json`; tài liệu dịch cần được hiệu chỉnh để phân biệt locale cài thủ công và locale tích hợp.
- Thứ tự khởi động hiện tại là `Se.LoadSettings()` → `Se.LoadLanguage()` trong `Program.cs`; `LanguageInitializer.UpdateLanguagesIfNeeded()` chạy muộn hơn từ `MainViewModel`. Vì vậy, chỉ đổi mặc định sang Vietnamese sẽ không đủ cho lần chạy sạch nếu asset chưa có trên đĩa.
- `SeGeneral.Language` mặc định là `English`. `SeOptions.LastLanguage = "en-us"` liên quan ngôn ngữ nội dung/tác vụ khác, không được đổi chỉ để đặt locale UI nếu không có bằng chứng sử dụng tương ứng.
- Bộ cài có 11 khóa `vi.*` riêng còn tiếng Anh trong `Subtitle_Edit_Localization.iss`, đồng thời thông báo thiếu .NET 10 trong `Subtitle_Edit_Installer.iss` cũng là chuỗi first-party viết cứng.
- Baseline hiện tại của `English.json` có khoảng 3.266 leaf value, bao gồm metadata. Số này chỉ mô tả snapshot; tiêu chí hoàn tất là parity 100% với baseline sau tất cả khóa mới.
- `LanguageJsonFilesTests` hiện chỉ parse JSON nguồn, chưa chứng minh parity, placeholder, CI gate hoặc asset thực sự mở được qua `AssetLoader`.

## 3. Phạm vi hữu hạn

### 3.1 Bao gồm

Inventory bắt buộc bao phủ:

- `src/ui/**/*.cs`, gồm window/view model/control/helper, dialog, notification/toast, validation, trạng thái, lỗi first-party, first-run/update UI, tray/native menu và nhãn shortcut/toolbar.
- Mọi markup/resource/template giao diện dưới `src/ui`, gồm `.xaml`, `.axaml` nếu có và tài nguyên text tương đương.
- Cây localization C# dưới `src/ui/Logic/Config/Language/`.
- `src/ui/Assets/Languages/English.json` và locale mới `Vietnamese.json`.
- Chuỗi first-party trong `installer/WindowsInno/*.iss`, gồm 11 khóa `vi.*` và thông báo thiếu .NET 10.
- Chuỗi lỗi first-party đi qua wrapper của thư viện. Nội dung nguyên văn do thư viện/dịch vụ bên ngoài trả về được loại trừ, nhưng phần tiêu đề/giải thích do ứng dụng thêm vào phải được dịch.

Inventory được lưu tại `docs/localization/vi/ui-string-inventory.md`, mỗi mục có: nguồn, vị trí/key, phân loại (`localized`, `technical-exception`, `external-runtime`, `non-ui`) và lý do. Hoàn tất được đối chiếu với inventory đã version-control, không chỉ dựa trên quét literal.

### 3.2 Không bao gồm

- Nội dung phụ đề của người dùng.
- Kết quả nhận dạng, dịch máy hoặc nội dung model sinh.
- Nội dung nguyên văn do hệ điều hành, thư viện hoặc dịch vụ ngoài trả về và ứng dụng không sở hữu.
- Website, tài liệu người dùng, CLI/seconv và metadata hệ sinh thái, ngoại trừ cập nhật kỹ thuật nhỏ trong `docs/translating.md` để mô tả đúng cách đặt tên/đóng gói locale.
- Dịch tên sản phẩm, format, codec, engine, model, extension, switch, API token và định danh máy.

## 4. Artifact ngôn ngữ và quy tắc dịch

### 4.1 Locale

- Asset tích hợp: `src/ui/Assets/Languages/Vietnamese.json`.
- Tên locale runtime: `Vietnamese`.
- Metadata culture: `vi-VN`.
- `English.json` là baseline parity dữ liệu; các lớp `SeLanguage` vẫn là schema/fallback runtime hiện hữu.

### 4.2 Glossary

Tạo `docs/localization/vi/glossary.md`, dạng bảng gồm:

- Thuật ngữ tiếng Anh.
- Bản dịch chuẩn hoặc chỉ dẫn giữ nguyên.
- Ngữ cảnh/ngoại lệ.
- Quy tắc viết hoa.

Mọi thay đổi thuật ngữ phải cập nhật glossary trong cùng commit. Mỗi cụm chức năng chỉ hoàn tất sau khi toàn bộ chuỗi trong cụm được review theo glossary; người dùng là người sign-off ngôn ngữ cuối cùng qua bản chạy/checklist hoặc diff bản dịch.

### 4.3 Ngoại lệ chuỗi giữ nguyên

Tạo `tests/UI/TestData/VietnameseUntranslatedAllowlist.json`, mỗi entry bắt buộc có `key`, `value`, `reason`; cấm wildcard. So sánh chưa dịch dùng exact value sau khi chuẩn hóa newline Unicode, không lower-case và không xóa dấu câu. Allowlist chỉ chấp nhận brand, acronym, format/codec/engine/model, token máy hoặc chuỗi không nên dịch.

Kiểm tra exact-equality chỉ là một gate chống sao chép nguyên văn, không chứng minh chất lượng ngôn ngữ. Chuỗi dịch một phần được phát hiện qua inventory review và checklist UI.

### 4.4 Placeholder

Trước triển khai phải inventory toàn bộ grammar placeholder thực tế. Gate tối thiểu hỗ trợ:

- .NET composite format: index, số lần xuất hiện, alignment và format specifier; cho phép đổi thứ tự index nhưng không được thiếu/thừa/đổi semantic specifier.
- Escaped braces `{{` và `}}`.
- Placeholder Inno Setup như `%1`, `%n` và constant `{app}`; giữ số lần và token chính xác.
- Token miền nghiệp vụ khác được ghi vào fixture ngay khi inventory phát hiện.

Test fixture bắt buộc có ca lặp, đổi thứ tự hợp lệ, thiếu, thừa, đổi format specifier và escaped token.

## 5. Kiến trúc triển khai

### 5.1 Parity và schema

Kiểm thử duyệt JSON đệ quy, so sánh Vietnamese với English theo key path và loại node. Sau Giai đoạn 3, mọi khóa mới đưa chuỗi viết cứng vào localization trở thành một phần baseline động; parity luôn được tính lại, không đóng băng ở con số 3.266.

### 5.2 Đồng bộ và đóng gói locale

Thiết lập một nguồn danh sách locale tích hợp hoặc kiểm tra ba tập hợp hiện có: file nguồn, `AvaloniaResource`, và `LanguageInitializer.LanguageFiles`. Gate yêu cầu chúng đồng bộ. `Vietnamese.json` phải được khai báo đúng một cách như `AvaloniaResource`, mở được bằng `AssetLoader`, giải nén thành công và có nội dung đúng.

Không bắt buộc máy móc cả `None Remove` lẫn `AvaloniaResource`; dùng mẫu MSBuild được build/resource test chứng minh không trùng item.

### 5.3 Startup và mặc định tiếng Việt

Thiết kế startup mới phải bảo đảm locale nhúng có thể được tải **trước lần dựng UI đầu tiên**, thay vì phụ thuộc lần giải nén muộn. Phương án triển khai được ưu tiên: khi file locale trên đĩa chưa tồn tại, loader mở asset nhúng tương ứng làm nguồn fallback khởi động, sau đó initializer đồng bộ asset ra thư mục runtime.

Quy tắc trạng thái:

1. Không có `Settings.json`: `SeGeneral.Language = "Vietnamese"`; loader tải Vietnamese từ file đĩa hoặc embedded asset, sau đó lưu lựa chọn khi settings được lưu.
2. Settings hiện hữu có `Language` hợp lệ: giữ nguyên chính xác lựa chọn đó.
3. Settings hiện hữu thiếu trường `Language`, trường rỗng hoặc trỏ locale không tồn tại: coi là cấu hình legacy/hỏng, fallback `English`, ghi log và không tự chuyển sang Vietnamese. Quy tắc này tránh ghi đè người dùng cũ.
4. `SetupLanguage.txt` của bộ cài không được tạo race với loader. Trên bản fork này, UI app mặc định Vietnamese khi không có settings; file installer chỉ có thể chọn locale khác nếu logic first-run gán và lưu lựa chọn một cách xác định trước khi dựng UI. Nếu không bảo đảm được, bỏ override installer và dùng Vietnamese thống nhất.
5. `SeOptions.LastLanguage` không thay đổi trừ khi test/tracing chứng minh nó điều khiển locale UI.

Các trường hợp trên áp dụng cho Windows installed/portable, Linux và macOS.

### 5.4 Đổi ngôn ngữ live

Giữ hành vi live reload hiện có: menu, native macOS menu, layout direction, toolbar và shortcut label được dựng lại, rồi lựa chọn được persist.

Việc rollback giao dịch khi file locale lỗi là hạng mục cần thiết vì setting hiện được đổi trước khi tải file. Luồng mới là: validate/load candidate → nếu thành công mới commit `Se.Settings.General.Language` và rebuild UI; nếu thất bại giữ locale/setting cũ, hiển thị thông báo localized và ghi log. Không mở rộng thành framework reliability tổng quát.

## 6. Các giai đoạn và quality gate

### Giai đoạn 1 — Hạ tầng và inventory

- Tạo glossary, allowlist và inventory versioned.
- Thêm test parse, key/shape parity, placeholder, metadata `vi-VN`, exact-untranslated, đồng bộ locale source/project/initializer và AssetLoader.
- Các test chung phải xanh ngay. Gate riêng cho Vietnamese chỉ được bật trong cùng commit thêm scaffold hợp lệ hoặc thêm locale ở commit kế tiếp rồi bỏ điều kiện trong chính commit đó; không commit suite đỏ/skip vô thời hạn.
- Bảo đảm project test UI thực sự được workflow `build-ui.yml` chạy; nếu chưa, cập nhật workflow để localization suite là CI gate.

### Giai đoạn 2 — Gói Vietnamese

- Tạo/dịch `Vietnamese.json` theo các cụm: main/edit; sync; video/audio; OCR; speech-to-text; machine translation; export; options.
- Review 100% entry từng cụm theo glossary trước khi đánh dấu cụm hoàn tất.
- Đăng ký resource và xác minh mở asset + giải nén.
- Cập nhật `docs/translating.md` để phân biệt tên file locale tích hợp với locale cài thủ công.

### Giai đoạn 3 — Chuỗi first-party ngoài JSON

- Chạy bộ truy vấn cố định trên toàn bộ nguồn trong §3.1, ghi kết quả vào inventory.
- Review toàn bộ kết quả và phân loại từng mục; không dùng tiêu chí “quét đến khi không còn gì mới”.
- Chuyển mọi mục `localized` vào model ngôn ngữ, bổ sung English + Vietnamese, thay literal và cập nhật inventory.
- Chạy lại cùng bộ truy vấn; không được còn mục chưa phân loại.

### Giai đoạn 4 — Installer, startup và live switching

- Dịch 11 khóa `vi.*` và chuỗi thiếu .NET 10 first-party.
- Triển khai quy tắc startup §5.3 và test đủ năm trạng thái.
- Triển khai validate-before-commit cho đổi ngôn ngữ và test persistence/rebuild/rollback.
- Build bộ cài Windows bằng Inno Setup là gate bắt buộc cho giai đoạn này. Nếu runner không có toolchain, trạng thái là `blocked/not-run`, không được coi là hoàn tất.

### Giai đoạn 5 — Kiểm chứng UI và polish

Checklist được lưu ở `docs/localization/vi/ui-verification-checklist.md`, mỗi mục có OS, DPI, kích thước cửa sổ, dữ liệu đầu vào, thao tác, kết quả và bằng chứng.

Matrix tối thiểu:

- Windows 11, 100% và 150% DPI, cửa sổ 1280×720 và 1920×1080.
- Linux và macOS: build + automated tests bắt buộc; smoke test runtime khi runner/host tương ứng của CI dự án sẵn có. Nếu host runtime không có, ghi `not-run` rõ ràng và không tuyên bố đã kiểm chứng trực quan nền tảng đó.

Luồng bắt buộc: startup sạch; đổi Việt→Anh→Việt và restart; mở/lưu; edit/find/replace; sync; video/audio; OCR; speech-to-text; machine translation; export; settings; dialog lỗi; installer success và thiếu .NET.

Tiêu chí UI:

- Không mất nội dung hoặc điều khiển không thể thao tác ở matrix Windows.
- Không có mnemonic trùng trong cùng menu/dialog; mọi accelerator còn hoạt động.
- Không có mục inventory chưa phân loại.
- Lỗi layout P0/P1/P2 phải đóng; P3 chỉ được hoãn khi ghi issue, ảnh và lý do. P0: crash/data loss; P1: không hoàn thành luồng; P2: chữ/điều khiển quan trọng bị cắt hoặc hiểu sai; P3: lỗi thẩm mỹ không cản thao tác.

## 7. Xử lý lỗi

- JSON sai, parity sai, placeholder sai, metadata Vietnamese sai hoặc resource drift: test/CI thất bại.
- Startup không đọc được locale đã chọn: fallback English, ghi log rõ file/exception; không ghi đè setting hiện hữu cho đến khi người dùng chọn locale hợp lệ.
- Đổi locale live thất bại: giữ nguyên setting và UI trước đó, thông báo lỗi localized, ghi log.
- Loader phải phân biệt missing file, malformed JSON và resource packaging failure trong log/test.

## 8. Chiến lược kiểm thử

### Tự động bắt buộc

- Unit tests JSON tree, metadata, placeholder và allowlist.
- Test đồng bộ asset folder ↔ project resource ↔ initializer.
- Integration test mở `avares://SubtitleEdit/Assets/Languages/Vietnamese.json` và kiểm tra giải nén.
- Test startup cho không settings, settings hợp lệ, thiếu field, field rỗng và locale không tồn tại.
- Test live switch thành công, persistence, rebuild side effects và rollback khi file lỗi.
- `dotnet test` cho suite UI và `dotnet build` cấu hình Debug/Release theo `build-ui.yml` trên Windows và Linux; macOS theo matrix hiện có của workflow nếu được cấu hình.
- Build Inno Setup trên Windows cho installer.

Mọi lệnh không chạy được phải được báo `blocked/not-run` kèm nguyên nhân và không thỏa Definition of Done tương ứng.

### Thủ công/end-to-end

Chạy ứng dụng thật theo checklist §6, lưu bằng chứng cho từng luồng. Review ngôn ngữ dựa trên glossary và ngữ cảnh UI, không chỉ dựa trên test cấu trúc.

## 9. Tổ chức commit dự kiến

1. `test: establish localization inventory and validation`
2. `feat: add complete Vietnamese UI resource`
3. `refactor: localize remaining first-party UI strings`
4. `feat: default new profiles to Vietnamese and harden language switching`
5. `feat: complete Vietnamese installer localization`
6. `fix: polish Vietnamese UI layout and terminology`

Mỗi commit chạy gate áp dụng cho trạng thái đó. Không push/PR ra upstream Subtitle Edit nếu người dùng chưa yêu cầu riêng.

## 10. Definition of Done

- `Vietnamese.json` có `cultureName: vi-VN`, parity 100% với `English.json` sau mọi khóa mới và qua toàn bộ test placeholder/allowlist.
- Asset folder, project resource và initializer đồng bộ; Vietnamese mở được qua AssetLoader và giải nén thành công.
- Inventory bao phủ toàn bộ nguồn §3.1, mọi mục đã phân loại và không còn mục `localized` chưa xử lý.
- Cấu hình mới trên mọi nền tảng mặc định `Vietnamese`; cấu hình hiện hữu hợp lệ được giữ; legacy/hỏng xử lý theo §5.3.
- Live switching validate trước commit, persist đúng, rebuild menu/layout/toolbar/native menu và rollback khi lỗi.
- Toàn bộ chuỗi installer first-party trong phạm vi, gồm 11 khóa và cảnh báo .NET 10, đã dịch; installer build đạt.
- Automated matrix đạt; các kiểm tra runtime thủ công đạt theo checklist và matrix đã cam kết.
- Không còn lỗi UI P0/P1/P2; P3 hoãn phải có issue và bằng chứng.
- Người dùng sign-off chất lượng ngôn ngữ dựa trên glossary, diff và checklist chạy thực tế.

## 11. Rủi ro và giảm thiểu

- **Upstream thay schema:** parity động làm CI fail ngay khi thiếu bản dịch.
- **Locale registry drift:** kiểm tra ba tập hợp hoặc một nguồn danh sách duy nhất loại bỏ lỗi im lặng.
- **Startup race:** embedded fallback được tải trước UI, không phụ thuộc unpack muộn.
- **Bỏ sót chuỗi:** inventory hữu hạn, truy vấn cố định và review toàn bộ kết quả.
- **False positive thuật ngữ:** allowlist exact, có key/value/reason, không wildcard.
- **Sai ngữ cảnh:** review theo cụm và chạy UI thật.
- **Câu tiếng Việt dài:** matrix DPI/kích thước và severity gate cụ thể.
- **Phá cấu hình cũ:** state matrix và regression test cho persistence/fallback.
