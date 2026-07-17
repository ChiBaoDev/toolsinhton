using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Avalonia.Headless.XUnit;
using Nikse.SubtitleEdit.Core.VobSub;
using Nikse.SubtitleEdit.Features.Shared.PickVobSubLanguage;
using Nikse.SubtitleEdit.Logic.Config;
using Nikse.SubtitleEdit.Logic.Config.Language;
using SkiaSharp;

namespace UITests.Logic.Localization;

internal sealed record Task10Inventory(string[] Roots, Task10InventoryRow[] Rows);

internal sealed record Task10InventoryRow(
    string Source, int Line, int Column, string Literal, string Classification, string? Category,
    string? Reason, string? LanguageKey, string? SourceExpression)
{
    public string Identity => $"{Source}:{Line}:{Column}:{Literal}:{SourceExpression}";
}

internal sealed record Task10Candidate(string Source, int Line, int Column, string Kind, string? Literal, string? SourceExpression)
{
    public string Identity => $"{Source}:{Line}:{Column}:{Kind}:{Literal}:{SourceExpression}";
}

public class FirstPartyUiLiteralInventoryTests
{
    private static readonly string[] RequiredRoots =
    {
        "src/ui/Features/Main",
        "src/ui/Features/Files",
        "src/ui/Features/Edit",
        "src/ui/Features/Sync",
        "src/ui/Features/Shared",
    };

    private static readonly string[] RequiredTask11Roots =
    {
        "src/ui/Features/Tools",
        "src/ui/Features/SpellCheck",
        "src/ui/Features/Ocr",
        "src/ui/Features/Video",
        "src/ui/Features/Translate",
        "src/ui/Features/Options",
        "src/ui/Features/Assa",
        "src/ui/Features/Ssa",
        "src/ui/Features/Plugins",
        "src/ui/Controls",
        "src/ui/Logic",
    };

    private static readonly string[] AllowedClassifications =
    {
        "localized",
        "technical-exception",
        "external-runtime",
        "non-ui",
    };

    private static readonly Regex UiPropertyAssignment = new(
        @"\b(?<kind>[A-Za-z0-9_]*(?:Title|Content|Header|Watermark))\s*=\s*(?:(?<literal>""(?:\\.|[^""\\])*"")|(?<expression>Se\.Language\.[A-Za-z0-9_.]+))",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex MarkupAttribute = new(
        @"(?<kind>[A-Za-z_][A-Za-z0-9_.:-]*)\s*=\s*""(?<literal>[^""]*[A-Za-z][^""]*)""",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly HashSet<string> TechnicalMarkupAttributes = new(StringComparer.OrdinalIgnoreCase)
    {
        "BasedOn", "Classes", "Class", "Command", "CommandParameter", "CompiledBinding", "DataContext",
        "DataType", "FontFamily", "Icon", "IsChecked", "IsEnabled", "IsSelected", "IsVisible", "Key",
        "Name", "Path", "Selector", "Source", "TargetType", "Theme", "ThemeVariant", "Uri", "x:Class",
        "x:CompileBindings", "x:DataType", "x:Key", "x:Name",
    };

    private static readonly Regex LanguageMemberExpression = new(
        @"Se\.Language\.[A-Za-z_][A-Za-z0-9_.]*",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex UiCall = new(
        @"(?<kind>ToolTip\.SetTip|(?:[A-Za-z0-9_]+\.)?(?:ShowMessageBox|ShowToast|ShowNotification|ShowMessage|MessageBox\.Show|MessageBox|Toast|Notification)|UiUtil\.(?:Show|Display)[A-Za-z0-9_]*|throw\s+new\s+[A-Za-z0-9_]*Exception)\s*\((?<arguments>[^;]*?)\)",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);

    private static readonly Regex Task11UiCall = new(
        @"(?<kind>ToolTip\.SetTip|Show(?:MessageBox|Toast|Notification)|UiUtil\.(?:Show|Display)[A-Za-z0-9_]*|throw\s+new\s+[A-Za-z0-9_]*Exception)\s*\((?<arguments>[^;]*?)\)",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);

    private static readonly Regex StringLiteral = new(
        @"""(?<literal>(?:\\.|[^""\\])*)""",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    [Fact]
    public void TargetedSourceCandidatesMatchStructuredInventory()
    {
        var root = RepositoryRoot();
        var inventory = LoadInventory(root, "tests/UI/TestData/Task10LiteralInventory.json");

        Assert.Equal(RequiredRoots, inventory.Roots);
        Assert.All(RequiredRoots, relativeRoot =>
            Assert.True(Directory.Exists(ToAbsolutePath(root, relativeRoot)), $"Required root does not exist: {relativeRoot}"));
        Assert.All(inventory.Rows, row =>
            Assert.Contains(RequiredRoots, requiredRoot => row.Source.StartsWith(requiredRoot + "/", StringComparison.Ordinal)));
        Assert.All(inventory.Rows, row =>
            Assert.Contains(Path.GetExtension(row.Source), new[] { ".cs", ".xaml", ".axaml" }));
        Assert.Equal(inventory.Rows.Length, inventory.Rows.Select(row => row.Identity).Distinct(StringComparer.Ordinal).Count());
        Assert.All(inventory.Rows, row => Assert.Contains(row.Classification, AllowedClassifications));

        var candidates = ScanCandidates(root, RequiredRoots);
        Assert.Equal(candidates.Length, candidates.Select(candidate => candidate.Identity).Distinct(StringComparer.Ordinal).Count());
        var candidateMatches = candidates
            .Select(candidate => (candidate, rows: inventory.Rows.Where(row => CandidateMatches(row, candidate)).ToArray()))
            .ToArray();
        var incorrectlyClassifiedCandidates = candidateMatches.Where(match => match.rows.Length != 1).ToArray();
        Assert.True(
            incorrectlyClassifiedCandidates.Length == 0,
            "Candidates without exactly one inventory row: " + string.Join("; ", incorrectlyClassifiedCandidates.Select(match => $"{Describe(match.candidate)} ({match.rows.Length} rows)")));

        var rowMatches = inventory.Rows
            .Select(row => (row, candidates: candidates.Where(candidate => CandidateMatches(row, candidate)).ToArray()))
            .ToArray();
        var staleRows = rowMatches.Where(match => match.candidates.Length != 1).ToArray();
        Assert.True(
            staleRows.Length == 0,
            "Inventory rows without exactly one current candidate: " + string.Join("; ", staleRows.Select(match => $"{match.row.Identity} ({match.candidates.Length} candidates)")));

        foreach (var row in inventory.Rows.Where(row => row.Classification == "localized"))
        {
            Assert.False(string.IsNullOrWhiteSpace(row.Category));
            Assert.False(string.IsNullOrWhiteSpace(row.LanguageKey));
            Assert.StartsWith("$.", row.LanguageKey, StringComparison.Ordinal);
            Assert.False(string.IsNullOrWhiteSpace(row.SourceExpression));
            Assert.StartsWith("Se.Language.", row.SourceExpression, StringComparison.Ordinal);
            Assert.Equal(ExpectedSourceExpression(row.LanguageKey!), row.SourceExpression);
            Assert.Equal(row.Literal, CatalogValue(root, "English.json", row.LanguageKey!));
            Assert.False(string.IsNullOrWhiteSpace(CatalogValue(root, "Vietnamese.json", row.LanguageKey!)));

        }

        foreach (var row in inventory.Rows.Where(row => row.Classification != "localized"))
        {
            Assert.False(string.IsNullOrWhiteSpace(row.Category));
            Assert.False(string.IsNullOrWhiteSpace(row.Reason));
            Assert.True(string.IsNullOrWhiteSpace(row.LanguageKey));
            if (!string.IsNullOrWhiteSpace(row.SourceExpression))
            {
                Assert.StartsWith("Se.Language.", row.SourceExpression, StringComparison.Ordinal);
                Assert.Equal("non-ui", row.Classification);
                Assert.Contains(row.Category, new[] { "language-object-alias", "commented-code" });
            }
        }
    }

    [Fact]
    public void Task11ScannerMatchesFixedScanScope()
    {
        const string source = "Title = Se.Language.General.Title; Title = \"Visible title\"; MessageBox.Show(\"Excluded alias\"); ShowMessageBox(\"Included message\");";
        var candidates = new List<Task10Candidate>();

        AddCSharpCandidates(candidates, "sample.cs", source, includeLanguageMembers: false, Task11UiCall);

        Assert.Equal(new[] { "Visible title", "Included message" }, candidates.Select(candidate => candidate.Literal));
    }

    [AvaloniaFact]
    public void Task11HotspotRuntimeValuesUseVietnameseCatalog()
    {
        var previous = Se.Language;
        Se.Language = LoadVietnameseLanguage();
        try
        {
            Assert.Equal("Cài đặt đầu ra", Se.Language.Video.BurnIn.OutputSettingsTitle);
            Assert.Equal("Đang tải mô hình Tesseract", Se.Language.Ocr.DownloadingTesseractModel);
            Assert.Equal("Chọn từ điển Tesseract:", Se.Language.Ocr.SelectTesseractDictionary);
            Assert.Equal("Thuật toán", Se.Language.Ocr.Algorithm);
            Assert.Equal("Thuật toán dùng để Tự động vẽ tạo các đường tiền cảnh/nền", Se.Language.Ocr.AutoDrawAlgorithmTooltip);
            Assert.Equal("Màu khi rê chuột", Se.Language.Assa.MouseOverColor);
            Assert.Equal("Màu đã nhấp", Se.Language.Assa.ClickedColor);
            Assert.Equal("✓ Đã sao chép!", Se.Language.Assa.Copied);
            Assert.Equal("Cài đặt sửa lỗi thường gặp", Se.Language.Tools.FixCommonErrors.SettingsTitle);
            Assert.Equal("Kiểm tra chính tả - Chỉnh sửa toàn bộ văn bản", Se.Language.SpellCheck.EditWholeTextTitle);
            Assert.Equal("Kiểm tra các mục bổ sung Binary OCR", Se.Language.Ocr.InspectBinaryOcrAdditions);
            Assert.Equal("Cơ sở dữ liệu So khớp ảnh nhị phân mới/đổi tên", Se.Language.Ocr.NewRenameBinaryImageCompareDatabase);
            Assert.Equal("Đang tải CrispEmbed", Se.Language.Ocr.DownloadingCrispEmbed);
            Assert.Equal("Đang tải OCR Google Lens", Se.Language.Ocr.DownloadingGoogleLensOcr);
            Assert.Equal("Đang tải PaddleOCR", Se.Language.Ocr.DownloadingPaddleOcr);
            Assert.Equal("Đang tải Tesseract", Se.Language.Ocr.DownloadingTesseract);
            Assert.Equal("Cơ sở dữ liệu nOCR mới/đổi tên", Se.Language.Ocr.NewRenameNOcrDatabase);
            Assert.Equal("Kiểu ASSA hiện tại sẽ được sử dụng\n\nHãy đổi định dạng phụ đề nếu bạn muốn đặt kiểu tại đây", Se.Language.Video.BurnIn.CurrentAssaStyleInfo);
            Assert.Equal("TTS - Tải bộ máy", Se.Language.Video.TextToSpeech.DownloadEngineTitle);
            Assert.Equal("Dùng hình để xóa (iclip)", Se.Language.Assa.DrawUseShapeForErase);
            Assert.Equal("Cài đặt CosyVoice3 (CrispASR)", Se.Language.Video.TextToSpeech.CosyVoice3CrispAsrSettings);
            Assert.Equal("Cài đặt F5-TTS (CrispASR)", Se.Language.Video.TextToSpeech.F5TtsCrispAsrSettings);
            Assert.Equal("Cài đặt IndexTTS (CrispASR)", Se.Language.Video.TextToSpeech.IndexTtsCrispAsrSettings);
            Assert.Equal("Cài đặt Qwen3 TTS (CrispASR)", Se.Language.Video.TextToSpeech.Qwen3TtsCrispAsrSettings);
            Assert.Equal("Cài đặt VibeVoice (CrispASR)", Se.Language.Video.TextToSpeech.VibeVoiceCrispAsrSettings);
            Assert.Equal("Cài đặt VoxCPM2 (CrispASR)", Se.Language.Video.TextToSpeech.VoxCpm2CrispAsrSettings);
            Assert.Equal("Không tìm thấy tệp mô hình.", Se.Language.Video.AudioToText.ModelFileNotFound);
            Assert.Equal("Mô hình whisper.cpp phải là tệp ggml '.bin'.", Se.Language.Video.AudioToText.WhisperCppModelMustBeGgmlBin);
            Assert.Equal("Không tìm thấy thư mục mô hình: {0}", Se.Language.Video.AudioToText.ModelFolderNotFoundX);
            Assert.Equal("Thư mục mô hình faster-whisper phải chứa tệp 'model.bin'.", Se.Language.Video.AudioToText.FasterWhisperModelFolderMustContainModelBin);
            Assert.Equal("Không tìm thấy tệp âm thanh đầu vào", Se.Language.Video.AudioToText.InputAudioFileNotFound);
            Assert.Equal("Không tìm thấy tệp âm thanh", Se.Language.Video.AudioToText.AudioFileNotFound);
            Assert.Equal("Hết thời gian chờ phiên chép lời DashScope sau {0} giây.", Se.Language.Video.AudioToText.DashScopeTranscriptionTimedOut);
            Assert.Equal("Hết thời gian chờ yêu cầu STT sau {0} giây.", Se.Language.Video.AudioToText.SttRequestTimedOut);
            Assert.Equal("Hết thời gian chờ phiên chép lời OpenRouter sau {0} giây.", Se.Language.Video.AudioToText.OpenRouterTranscriptionTimedOut);
            Assert.Equal("Không tìm thấy tệp thực thi CrispASR. Hãy cài đặt CrispASR qua Video → Âm thanh thành văn bản trước.", Se.Language.Video.TextToSpeech.CrispAsrExecutableNotFound);
            Assert.Equal("Không tìm thấy tệp thực thi máy chủ Kokoro TTS.", Se.Language.Video.TextToSpeech.KokoroServerExecutableNotFound);
            Assert.Equal("Thiếu mô hình hoặc tệp giọng nói của Kokoro TTS.", Se.Language.Video.TextToSpeech.KokoroModelOrVoicesFileMissing);
            Assert.Equal("Không tìm thấy tệp thực thi omnivoice-tts.", Se.Language.Video.TextToSpeech.OmniVoiceExecutableNotFound);
            Assert.Equal("Nhân bản giọng nói OmniVoice TTS cần tệp bản chép lời tại {0}. Hãy nhập lại giọng nói để cung cấp bản chép lời.", Se.Language.Video.TextToSpeech.OmniVoiceTranscriptRequiredX);
            Assert.Equal("Không thể khởi động omnivoice-tts.", Se.Language.Video.TextToSpeech.OmniVoiceStartFailed);
            Assert.Equal("Không thể khởi động {0}.", Se.Language.Video.TextToSpeech.ServerStartFailedX);
            Assert.Equal("Máy chủ {0} không báo trạng thái sẵn sàng trong vòng {1} giây. Kết quả đầu ra cuối: {2}", Se.Language.Video.TextToSpeech.ServerHealthTimedOutXXX);
            Assert.Equal("Không thể kết nối đến máy chủ AllTalk TTS. Hãy kiểm tra xem máy chủ có đang chạy hay không.", Se.Language.Video.TextToSpeech.AllTalkServerNotReachable);
            Assert.Equal("Yêu cầu đến máy chủ AllTalk TTS đã hết thời gian chờ. Hãy kiểm tra xem máy chủ có đang chạy hay không.", Se.Language.Video.TextToSpeech.AllTalkRequestTimedOut);
            Assert.Equal("Hãy đặt khu vực Azure trong cài đặt công cụ TTS trước khi làm mới danh sách giọng nói.", Se.Language.Video.TextToSpeech.AzureRegionRequiredForVoiceRefresh);
            Assert.Equal("Không tìm thấy tệp thực thi máy chủ Qwen3 TTS.", Se.Language.Video.TextToSpeech.Qwen3ServerExecutableNotFound);
            Assert.Equal("Không thể phân tích phản hồi chính sách tải lên của DashScope. Phản hồi: {0}", Se.Language.Video.AudioToText.DashScopeUploadPolicyParseFailedX);
            Assert.Equal("Phản hồi gửi bất đồng bộ của DashScope không chứa task_id. Phản hồi: {0}", Se.Language.Video.AudioToText.DashScopeTaskIdMissingX);
            Assert.Equal("Tác vụ DashScope đã hoàn tất nhưng không chứa transcription_url. Phản hồi: {0}", Se.Language.Video.AudioToText.DashScopeTranscriptionUrlMissingX);
            Assert.Equal("Tác vụ phiên âm DashScope kết thúc với trạng thái {0}. Phản hồi: {1}", Se.Language.Video.AudioToText.DashScopeTaskFailedXX);
            Assert.Equal("Yêu cầu chính sách tải lên DashScope thất bại ({0}). Phản hồi: {1}", Se.Language.Video.AudioToText.DashScopeUploadPolicyRequestFailedXX);
            Assert.Equal("Tải tệp lên DashScope OSS thất bại ({0}). Phản hồi: {1}", Se.Language.Video.AudioToText.DashScopeOssUploadFailedXX);
            Assert.Equal("Gửi tác vụ bất đồng bộ đến DashScope thất bại ({0}). Phản hồi: {1}", Se.Language.Video.AudioToText.DashScopeAsyncSubmitFailedXX);
            Assert.Equal("Truy vấn tác vụ DashScope thất bại ({0}). Phản hồi: {1}", Se.Language.Video.AudioToText.DashScopeTaskPollFailedXX);
            Assert.Equal("ffmpeg không thể trích xuất đoạn {0}/{1} ({2} giây → {3} giây) từ {4}", Se.Language.Video.AudioToText.FfmpegChunkExtractionFailedXXXXX);
            Assert.Equal("Không thể trích xuất khung hình hiện tại - hãy xem nhật ký để biết dòng lệnh ffmpeg.", Se.Language.Video.VideoOcr.CurrentFrameExtractionFailed);
            Assert.Equal("Không trích xuất được khung hình nào từ video - hãy xem nhật ký để biết dòng lệnh ffmpeg.", Se.Language.Video.VideoOcr.NoFramesExtracted);
            Assert.Equal("PaddleOCR thất bại: {0}", Se.Language.Video.VideoOcr.PaddleOcrFailedX);
            Assert.Equal("yt-dlp đã hoàn tất nhưng không tạo ra tệp video.\nThư mục tạm: {0}\nNội dung: {1}", Se.Language.Video.OpenFromUrlNoVideoProducedXX);
            Assert.Equal("Thư mục đầu ra chưa được đặt.", Se.Language.Tools.BatchConvert.OutputFolderNotSet);
            Assert.Equal("Lệnh gọi API Cloud Vision thất bại với mã trạng thái {0}.", Se.Language.Ocr.CloudVisionApiStatusErrorX);
            Assert.Equal("Lỗi khi gọi API Cloud Vision: {0}", Se.Language.Ocr.CloudVisionApiCallErrorX);
            Assert.Equal("Khóa API Cloud Vision không hợp lệ hoặc tính năng thanh toán/API chưa được bật.", Se.Language.Ocr.CloudVisionApiKeyInvalid);
            Assert.Equal("Cloud Vision từ chối yêu cầu. Hãy kiểm tra tính năng thanh toán, trạng thái API và khóa API.", Se.Language.Ocr.CloudVisionRequestForbidden);
            Assert.Equal("Chatterbox cần CrispASR v0.6.0 trở lên. Hãy tải lại CrispASR qua Video → Âm thanh thành văn bản → Cài đặt bộ máy → Tải lại, rồi thử lại.", Se.Language.Video.TextToSpeech.ChatterboxRequiresCrispAsrUpdate);
            Assert.Equal("Chatterbox không thể tải mô hình — các tệp GGUF trong {0} có thể đã cũ hoặc tải xuống chưa hoàn tất. Hãy xóa chúng rồi thử lại để tải xuống lại. Kết quả đầu ra gốc: {1}", Se.Language.Video.TextToSpeech.ChatterboxModelCacheStaleXX);
            Assert.Equal("Chatterbox TTS \"Turbo\" không tải được với CrispASR 0.8.0. Mô hình turbo không có lỗi — bước kiểm tra tokenizer/từ vựng của phiên bản 0.8.0 quá nghiêm ngặt và đã từ chối tập nhúng mở rộng hợp lệ (tokenizer 50257 token, kích thước từ vựng văn bản 50276). Lỗi này đã được sửa ở thượng nguồn (CrispStrobe/CrispASR#181): CrispASR mới hơn tải Turbo bình thường mà không cần tải lại. Trong thời gian chờ, hãy chuyển sang mô hình Chatterbox \"Base\"; mô hình này hoạt động.\n\n{0}", Se.Language.Video.TextToSpeech.ChatterboxTurboTokenizerMismatchX);
            Assert.Equal("Mô hình Chatterbox TTS \"Turbo\" đã làm CrispASR gặp sự cố khi khởi động. Đây là sự cố đã biết ở phần phụ trợ chatterbox-turbo (đặc biệt trên macOS/CPU). Hãy thử mô hình \"Base\", hoặc báo lỗi tại https://github.com/CrispStrobe/CrispASR/issues kèm nhật ký bên dưới.\n\n{0}", Se.Language.Video.TextToSpeech.ChatterboxTurboStartupCrashX);
            Assert.Equal("\"{0} {1}\" không hoàn tất trong vòng {2} giây và đã bị dừng.", Se.Language.Video.TextToSpeech.ProcessTimedOutXXX);
            Assert.Equal("crispasr (chatterbox) đã thoát trong khi khởi động (mã {0}). Kết quả đầu ra: {1}", Se.Language.Video.TextToSpeech.ChatterboxExitedDuringStartupXX);
            Assert.Equal("crispasr (chatterbox) không báo trạng thái sẵn sàng trong vòng 15 phút. Kết quả đầu ra cuối: {0}", Se.Language.Video.TextToSpeech.ChatterboxHealthTimedOutX);
            Assert.Equal("{0} không khả dụng cho Linux ARM64.", Se.Language.General.DownloadUnavailableForLinuxArm64X);
            Assert.Equal("Kiến trúc macOS không được hỗ trợ.", Se.Language.General.UnsupportedMacOsArchitecture);
            Assert.Equal("Không hỗ trợ tải xuống {0} trên nền tảng này.", Se.Language.General.DownloadNotSupportedOnPlatformX);
            Assert.Equal("Không tìm thấy URL được yêu cầu: {0}", Se.Language.General.RequestedUrlNotFoundX);
            Assert.Equal("Tải xuống chưa hoàn tất: dự kiến {0} byte, đã nhận {1} byte", Se.Language.General.DownloadIncompleteXX);
            Assert.Equal("Không thể tải tệp sau {0} lần thử. URL: {1}. Đã tải: {2}/{3} byte", Se.Language.General.DownloadFailedAfterAttemptsXXXX);
            Assert.Equal("Không thể giải nén bằng {0}, mã thoát {1}: {2}", Se.Language.General.ArchiveExtractionFailedXXX);
            Assert.Equal("Không tìm thấy tệp thực thi {0} tại {1}", Se.Language.General.ArchiveExecutableNotFoundXX);
            Assert.Equal("yt-dlp chưa được cài đặt.", Se.Language.Video.YtDlpNotInstalled);
            Assert.Equal("Không thể khởi động yt-dlp.", Se.Language.Video.YtDlpStartFailed);
            Assert.Equal("yt-dlp đã thoát với mã {0}.", Se.Language.Video.YtDlpExitedWithCodeX);
            Assert.Equal("Quá trình tải phụ đề bằng yt-dlp đã thoát với mã {0}.", Se.Language.Video.YtDlpSubtitleDownloadExitedWithCodeX);
            Assert.Equal("Tệp yt-dlp đã tải xuống ({0}) không vượt qua bước xác minh SHA-256 — dự kiến {1}, nhận được {2}. Tệp đã bị xóa.", Se.Language.Video.YtDlpChecksumFailedXXX);
        }
        finally
        {
            Se.Language = previous;
        }
    }

    [Fact]
    public void Task11TargetedSourceCandidatesMatchStructuredInventory()
    {
        var root = RepositoryRoot();
        var inventory = LoadInventory(root, "tests/UI/TestData/Task11LiteralInventory.json");

        Assert.Equal(RequiredTask11Roots, inventory.Roots);
        var candidates = ScanTask11Candidates(root, inventory);
        AssertInventoryMatchesCandidates(root, inventory, candidates);
    }

    [AvaloniaFact]
    public void PickVobSubLanguageRuntimePathUsesVietnameseTitle()
    {
        var previous = Se.Language;
        Se.Language = LoadVietnameseLanguage();
        try
        {
            var viewModel = new PickVobSubLanguageViewModel();
            viewModel.Initialize(
                new Dictionary<int, List<VobSubMergedPack>> { [0x20] = [] },
                new List<SKColor>(),
                ["English (0x20)"],
                @"C:\subtitles\sample.idx");

            var window = new PickVobSubLanguageWindow(viewModel);

            Assert.Equal("Chọn ngôn ngữ VobSub - sample.idx", viewModel.WindowTitle);
            Assert.Equal(viewModel.WindowTitle, window.Title);
            var language = Assert.Single(viewModel.Languages);
            Assert.Equal(0x20, language.StreamId);
            Assert.Equal("English (0x20)", language.Language);
            Assert.Same(window, viewModel.Window);
        }
        finally
        {
            Se.Language = previous;
        }
    }

    [Fact]
    public void LocalizedRuntimeValuesAreVietnamese()
    {
        var previous = Se.Language;
        Se.Language = LoadVietnameseLanguage();
        try
        {
            Assert.Equal("Chọn bố cục", Se.Language.Main.LayoutTitle);
            Assert.Equal("Xuất PAC", Se.Language.File.ExportPacTitle);
            Assert.Equal("Chọn bảng mã PAC", Se.Language.File.ChoosePacCodePage);
            Assert.Equal("Đang tải ffmpeg", Se.Language.Main.DownloadingFfmpeg);
            Assert.Equal("Đang tải libmpv", Se.Language.Main.DownloadingLibMpv);
            Assert.Equal("Xuất Cavena 890", Se.Language.File.ExportCavena890Title);
            Assert.Equal("Xuất EBU STL", Se.Language.File.ExportEbuStlTitle);
            Assert.Equal("Đặt văn bản", Se.Language.Tools.ImageBasedEdit.SetText);
            Assert.Equal("Thời lượng tối thiểu (mili giây):", Se.Language.Tools.ApplyDurationLimits.MinimumDurationMilliseconds);
            Assert.Equal("Thời lượng tối đa (mili giây):", Se.Language.Tools.ApplyDurationLimits.MaximumDurationMilliseconds);
        }
        finally
        {
            Se.Language = previous;
        }
    }

    [Theory]
    [InlineData("<TextBlock>(Optional)</TextBlock>", "(Optional)")]
    [InlineData("<TextBlock>1 item</TextBlock>", "1 item")]
    [InlineData("<TextBlock>&amp; English</TextBlock>", "& English")]
    public void MarkupElementTextFindsEnglishLettersAnywhereAfterDecodingAndTrimming(string markup, string expected)
    {
        var candidates = new List<Task10Candidate>();

        AddMarkupCandidates(candidates, "sample.axaml", markup);

        var candidate = Assert.Single(candidates);
        Assert.Equal("element-text", candidate.Kind);
        Assert.Equal(expected, candidate.Literal);
    }

    [Theory]
    [InlineData("<TextBlock>Hello <Run>world</Run></TextBlock>", "Hello", "world")]
    [InlineData("<TextBlock><![CDATA[English text]]></TextBlock>", "English text")]
    public void MarkupElementTextScansNestedAndCDataTextNodes(string markup, params string[] expected)
    {
        var candidates = new List<Task10Candidate>();

        AddMarkupCandidates(candidates, "sample.axaml", markup);

        Assert.Equal(expected, candidates.Select(candidate => candidate.Literal));
    }

    [Fact]
    public void MarkupElementTextIgnoresXmlComments()
    {
        var candidates = new List<Task10Candidate>();

        AddMarkupCandidates(candidates, "sample.axaml", "<Root><!-- <TextBlock>English</TextBlock> --></Root>");

        Assert.Empty(candidates);
    }

    [Fact]
    public void IdenticalLiteralsOnOneLineRemainDistinctCandidates()
    {
        const string markup = "<StackPanel><TextBlock>English</TextBlock><TextBlock>English</TextBlock></StackPanel>";
        var candidates = new List<Task10Candidate>();

        AddMarkupCandidates(candidates, "sample.axaml", markup);

        Assert.Equal(2, candidates.Count);
        Assert.Equal(2, candidates.Select(candidate => candidate.Column).Distinct().Count());
    }

    [Fact]
    public void FixedScanIncludesIdentifiersEndingInContent()
    {
        const string source = "userContent = \"Context text\";";
        var candidates = new List<Task10Candidate>();

        AddCSharpCandidates(candidates, "sample.cs", source, includeLanguageMembers: true, UiCall);

        var candidate = Assert.Single(candidates);
        Assert.Equal("userContent", candidate.Kind);
        Assert.Equal("Context text", candidate.Literal);
    }

    [Fact]
    public void GenericLanguageMemberScanFindsPreviouslyUnlistedExpressionAtEveryOccurrence()
    {
        const string source = "var file = Pick(Se.Language.General.TextFiles, Se.Language.General.TextFiles);";
        var candidates = new List<Task10Candidate>();

        AddCSharpCandidates(candidates, "sample.cs", source, includeLanguageMembers: true, UiCall);

        var expressions = candidates.Where(candidate => candidate.SourceExpression == "Se.Language.General.TextFiles").ToArray();
        Assert.Equal(2, expressions.Length);
        Assert.Equal(new[] { 17, 48 }, expressions.Select(candidate => candidate.Column));
    }

    [Fact]
    public void Task10ReportsContainNoTabsOrC0ControlCharacters()
    {
        var root = RepositoryRoot();
        var paths = new[]
        {
            ".superpowers/sdd/task-10-report.md",
            "docs/localization/vi/ui-string-inventory.md",
        };

        foreach (var path in paths)
        {
            var text = File.ReadAllText(ToAbsolutePath(root, path));
            var invalid = text
                .Select((character, index) => (character, index))
                .Where(item => item.character == '\t' || item.character < ' ' && item.character is not '\r' and not '\n')
                .ToArray();
            Assert.True(invalid.Length == 0, $"{path} contains tabs/C0 controls at offsets: {string.Join(", ", invalid.Select(item => item.index))}");
        }
    }


    private static void AssertInventoryMatchesCandidates(string root, Task10Inventory inventory, Task10Candidate[] candidates)
    {
        Assert.All(inventory.Rows, row => Assert.Contains(row.Classification, AllowedClassifications));
        Assert.Equal(inventory.Rows.Length, inventory.Rows.Select(row => row.Identity).Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(candidates.Length, candidates.Select(candidate => candidate.Identity).Distinct(StringComparer.Ordinal).Count());
        var candidateMatches = candidates.Select(candidate => (candidate, rows: inventory.Rows.Where(row => CandidateMatches(row, candidate)).ToArray())).ToArray();
        var missing = candidateMatches.Where(match => match.rows.Length != 1).ToArray();
        Assert.True(missing.Length == 0, "Candidates without exactly one inventory row: " + string.Join("; ", missing.Select(match => $"{Describe(match.candidate)} ({match.rows.Length} rows)")));
        var rowMatches = inventory.Rows.Select(row => (row, candidates: candidates.Where(candidate => CandidateMatches(row, candidate)).ToArray())).ToArray();
        var stale = rowMatches.Where(match => match.candidates.Length != 1).ToArray();
        Assert.True(stale.Length == 0, "Inventory rows without exactly one current candidate: " + string.Join("; ", stale.Select(match => $"{match.row.Identity} ({match.candidates.Length} candidates)")));
        foreach (var row in inventory.Rows.Where(row => row.Classification == "localized"))
        {
            Assert.False(string.IsNullOrWhiteSpace(row.Category));
            Assert.False(string.IsNullOrWhiteSpace(row.LanguageKey));
            Assert.StartsWith("$.", row.LanguageKey, StringComparison.Ordinal);
            Assert.False(string.IsNullOrWhiteSpace(row.SourceExpression));
            Assert.Equal(ExpectedSourceExpression(row.LanguageKey!), row.SourceExpression);
            Assert.Equal(row.Literal, CatalogValue(root, "English.json", row.LanguageKey!));
            Assert.False(string.IsNullOrWhiteSpace(CatalogValue(root, "Vietnamese.json", row.LanguageKey!)));
        }
        foreach (var row in inventory.Rows.Where(row => row.Classification != "localized"))
        {
            Assert.False(string.IsNullOrWhiteSpace(row.Category));
            Assert.False(string.IsNullOrWhiteSpace(row.Reason));
            Assert.True(string.IsNullOrWhiteSpace(row.LanguageKey));
        }
    }

    private static SeLanguage LoadVietnameseLanguage() =>
        JsonSerializer.Deserialize<SeLanguage>(
            File.ReadAllText(ToAbsolutePath(RepositoryRoot(), "src/ui/Assets/Languages/Vietnamese.json")),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
        ?? throw new InvalidDataException("Could not load Vietnamese language catalog.");

    private static Task10Inventory LoadInventory(string root, string path) =>
        JsonSerializer.Deserialize<Task10Inventory>(
            File.ReadAllText(ToAbsolutePath(root, path)),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
        ?? throw new InvalidDataException($"Could not load literal inventory: {path}");

    private static Task10Candidate[] ScanTask11Candidates(string root, Task10Inventory inventory)
    {
        var candidates = ScanCandidates(root, RequiredTask11Roots, includeLanguageMembers: false, uiCall: Task11UiCall).ToList();
        foreach (var row in inventory.Rows.Where(row => row.Classification == "localized"))
        {
            var path = ToAbsolutePath(root, row.Source);
            var lines = File.ReadAllLines(path);
            if (row.Line <= 0 || row.Line > lines.Length || string.IsNullOrWhiteSpace(row.SourceExpression))
            {
                continue;
            }

            var line = lines[row.Line - 1];
            var column = line.IndexOf(row.SourceExpression, StringComparison.Ordinal) + 1;
            if (column > 0)
            {
                AddCandidate(candidates, row.Source, row.Line, column, "localized", null, row.SourceExpression);
            }
        }

        return candidates.ToArray();
    }

    private static Task10Candidate[] ScanCandidates(string root, IReadOnlyList<string> roots, bool includeLanguageMembers = true, Regex? uiCall = null)
    {
        var candidates = new List<Task10Candidate>();
        foreach (var relativeRoot in roots)
        {
            var absoluteRoot = ToAbsolutePath(root, relativeRoot);
            if (!Directory.Exists(absoluteRoot))
            {
                continue;
            }

            foreach (var file in Directory.EnumerateFiles(absoluteRoot, "*.*", SearchOption.AllDirectories)
                         .Where(IsScannedSourceFile)
                         .OrderBy(file => file, StringComparer.OrdinalIgnoreCase))
            {
                var source = Path.GetRelativePath(root, file).Replace(Path.DirectorySeparatorChar, '/');
                var text = File.ReadAllText(file);
                if (file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                {
                    if (!includeLanguageMembers && source.StartsWith("src/ui/Logic/Config/Language/", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    AddCSharpCandidates(candidates, source, text, includeLanguageMembers, uiCall ?? UiCall);
                }
                else
                {
                    AddMarkupCandidates(candidates, source, text);
                }
            }
        }

        return candidates.ToArray();
    }

    private static void AddCSharpCandidates(List<Task10Candidate> candidates, string source, string text, bool includeLanguageMembers, Regex uiCall)
    {
        foreach (Match match in UiPropertyAssignment.Matches(text))
        {
            var literal = Unquote(match.Groups["literal"].Value);
            if (literal is not null)
            {
                AddCandidate(candidates, source, text, match.Index, match.Groups["kind"].Value, literal, null);
            }
        }

        foreach (Match call in uiCall.Matches(text))
        {
            var arguments = call.Groups["arguments"];
            foreach (Match literal in StringLiteral.Matches(arguments.Value))
            {
                AddCandidate(candidates, source, text, arguments.Index + literal.Index, call.Groups["kind"].Value,
                    literal.Groups["literal"].Value, null);
            }

        }

        if (includeLanguageMembers)
        {
            foreach (Match expression in LanguageMemberExpression.Matches(text))
            {
                AddCandidate(candidates, source, text, expression.Index, "language-member", null, expression.Value);
            }
        }
    }

    private static void AddMarkupCandidates(List<Task10Candidate> candidates, string source, string text)
    {
        foreach (Match match in MarkupAttribute.Matches(text))
        {
            var kind = match.Groups["kind"].Value;
            var literal = match.Groups["literal"].Value;
            if (!IsTechnicalMarkupAttribute(kind, literal))
            {
                AddCandidate(candidates, source, text, match.Index, $"attribute:{kind}", literal, null);
            }
        }

        var document = XDocument.Parse(text, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
        foreach (var node in document.DescendantNodes().Where(node => node is XText or XCData))
        {
            var literal = ((XText)node).Value.Trim();
            if (!literal.Any(character => character is >= 'A' and <= 'Z' or >= 'a' and <= 'z'))
            {
                continue;
            }

            var lineInfo = (IXmlLineInfo)node;
            AddCandidate(candidates, source, lineInfo.LineNumber, lineInfo.LinePosition, "element-text", literal, null);
        }
    }

    private static bool IsTechnicalMarkupAttribute(string kind, string literal) =>
        kind.StartsWith("xmlns", StringComparison.OrdinalIgnoreCase) ||
        TechnicalMarkupAttributes.Contains(kind) ||
        IsTechnicalAttachedProperty(kind) ||
        literal.StartsWith('{') ||
        literal.Contains("://", StringComparison.Ordinal) ||
        literal.StartsWith("avares://", StringComparison.OrdinalIgnoreCase) ||
        literal.StartsWith("resm:", StringComparison.OrdinalIgnoreCase) ||
        literal.StartsWith("#", StringComparison.Ordinal) ||
        literal.EndsWith(".axaml", StringComparison.OrdinalIgnoreCase) ||
        literal.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase);

    private static bool IsTechnicalAttachedProperty(string kind) =>
        kind.Contains('.', StringComparison.Ordinal) &&
        !kind.Equals("ToolTip.Tip", StringComparison.OrdinalIgnoreCase) &&
        !kind.EndsWith(".Header", StringComparison.OrdinalIgnoreCase) &&
        !kind.EndsWith(".Content", StringComparison.OrdinalIgnoreCase) &&
        !kind.EndsWith(".Watermark", StringComparison.OrdinalIgnoreCase) &&
        !kind.EndsWith(".Title", StringComparison.OrdinalIgnoreCase);

    private static void AddCandidate(List<Task10Candidate> candidates, string source, string text, int index, string kind, string? literal, string? sourceExpression)
    {
        if (string.IsNullOrEmpty(literal) && string.IsNullOrEmpty(sourceExpression))
        {
            return;
        }

        AddCandidate(candidates, source, LineNumber(text, index), ColumnNumber(text, index), kind, literal, sourceExpression);
    }

    private static void AddCandidate(List<Task10Candidate> candidates, string source, int line, int column, string kind, string? literal, string? sourceExpression)
    {
        candidates.Add(new Task10Candidate(source, line, column, kind, literal, sourceExpression));
    }

    private static bool CandidateMatches(Task10InventoryRow row, Task10Candidate candidate) =>
        row.Source == candidate.Source &&
        row.Line == candidate.Line &&
        (!string.IsNullOrWhiteSpace(row.SourceExpression)
            ? row.Column == candidate.Column && row.SourceExpression == candidate.SourceExpression
            : row.Literal == candidate.Literal);

    private static string ExpectedSourceExpression(string path) =>
        "Se.Language." + string.Join('.', path[2..].Split('.').Select(segment => char.ToUpperInvariant(segment[0]) + segment[1..]));

    private static string CatalogValue(string root, string catalogFile, string path)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(ToAbsolutePath(root, $"src/ui/Assets/Languages/{catalogFile}")));
        var current = document.RootElement;
        foreach (var segment in path[2..].Split('.'))
        {
            Assert.True(current.TryGetProperty(segment, out current), $"Unknown {catalogFile} catalog path: {path}");
        }

        Assert.Equal(JsonValueKind.String, current.ValueKind);
        return current.GetString()!;
    }

    private static bool IsScannedSourceFile(string file) =>
        file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) ||
        file.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase) ||
        file.EndsWith(".axaml", StringComparison.OrdinalIgnoreCase);

    private static int LineNumber(string text, int index) => text[..index].Count(character => character == '\n') + 1;

    private static int ColumnNumber(string text, int index)
    {
        var lineStart = text.LastIndexOf('\n', Math.Max(0, index - 1));
        return index - lineStart;
    }

    private static string? Unquote(string value)
    {
        var quote = value.IndexOf('"');
        return quote >= 0 && value.Length > quote + 1 ? value[(quote + 1)..^1] : null;
    }


    private static string Describe(Task10Candidate candidate) =>
        $"{candidate.Source}:{candidate.Line}:{candidate.Column}:{candidate.Kind}: {candidate.Literal ?? candidate.SourceExpression}";

    private static string ToAbsolutePath(string root, string relativePath) =>
        Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar));

    private static string RepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "SubtitleEdit.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate repository root.");
    }
}
