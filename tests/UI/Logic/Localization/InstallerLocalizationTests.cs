namespace UITests.Logic.Localization;

public class InstallerLocalizationTests
{
    private static readonly IReadOnlyDictionary<string, string> ExpectedVietnameseMessages =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["sm_com_Changelog"] = "Nhật ký thay đổi của Subtitle Edit",
            ["run_ViewChangelog"] = "Xem nhật ký thay đổi",
            ["msg_DeleteSettings"] = "Bạn có muốn xóa các thiết lập cá nhân của Subtitle Edit không?",
            ["tsk_AllUsers"] = "Dành cho tất cả người dùng",
            ["tsk_CurrentUser"] = "Chỉ dành cho người dùng hiện tại",
            ["tsk_Other"] = "Khác:",
            ["tsk_ResetDictionaries"] = "Đặt lại từ điển và xóa các tên tùy chỉnh",
            ["tsk_ResetSettings"] = "Đặt lại thiết lập của Subtitle Edit",
            ["tsk_SetFileTypes"] = "Liên kết các tệp phụ đề thông dụng với Subtitle Edit",
            ["types_custom"] = "Cài đặt tùy chỉnh",
            ["types_default"] = "Cài đặt mặc định",
        };

    [Fact]
    public void VietnameseCustomMessages_AllDifferFromEnglish()
    {
        var messages = ReadCustomMessages();
        var errors = ExpectedVietnameseMessages
            .Where(pair => !messages.TryGetValue($"en.{pair.Key}", out var english) ||
                           !messages.TryGetValue($"vi.{pair.Key}", out var vietnamese) ||
                           string.Equals(english, vietnamese, StringComparison.Ordinal) ||
                           !string.Equals(pair.Value, vietnamese, StringComparison.Ordinal))
            .Select(pair => $"vi.{pair.Key} must equal the required Vietnamese text and differ from en.{pair.Key}.")
            .ToArray();

        Assert.True(errors.Length == 0, string.Join(Environment.NewLine, errors));
    }

    [Fact]
    public void DotNet10Warning_HasEnglishFallbackAndVietnameseWithMatchingTokens()
    {
        var messages = ReadCustomMessages();
        Assert.True(messages.TryGetValue("msg_DotNet10Required", out var english), "Missing unqualified English fallback msg_DotNet10Required.");
        Assert.True(messages.TryGetValue("vi.msg_DotNet10Required", out var vietnamese), "Missing vi.msg_DotNet10Required.");
        Assert.NotEqual(english, vietnamese);
        Assert.Equal(
            InnoPlaceholderParser.Parse(english!).OrderBy(pair => pair.Key, StringComparer.Ordinal),
            InnoPlaceholderParser.Parse(vietnamese!).OrderBy(pair => pair.Key, StringComparer.Ordinal));
        Assert.Equal(4, InnoPlaceholderParser.Parse(english!)["%n"]);
        Assert.Equal(
            "Subtitle Edit requires the .NET 10 Runtime, which is not installed on this computer.%n%nPlease download and install the .NET 10 Runtime and run this setup again.%n%nDo you want to open the .NET 10 download page now?",
            english);
        Assert.Equal(
            "Subtitle Edit yêu cầu .NET 10 Runtime nhưng máy tính này chưa cài đặt.%n%nHãy tải xuống và cài đặt .NET 10 Runtime, sau đó chạy lại bộ cài.%n%nBạn có muốn mở trang tải .NET 10 ngay bây giờ không?",
            vietnamese);
    }

    [Fact]
    public void DotNet10Warning_UsesLocalizedCustomMessageInsteadOfHardCodedEnglish()
    {
        var installer = File.ReadAllText(InstallerPath("Subtitle_Edit_Installer.iss"));

        Assert.Contains("CustomMessage('msg_DotNet10Required')", installer, StringComparison.Ordinal);
        Assert.DoesNotContain("Subtitle Edit requires the .NET 10 Runtime, which is not installed on this computer.", installer, StringComparison.Ordinal);
    }

    [Fact]
    public void ForcedDotNet10Missing_DefaultsOffAndCanBypassDetectionAtCompileTime()
    {
        var installer = File.ReadAllText(InstallerPath("Subtitle_Edit_Installer.iss"));

        Assert.Contains("#ifndef FORCE_DOTNET10_MISSING", installer, StringComparison.Ordinal);
        Assert.Contains("#define FORCE_DOTNET10_MISSING 0", installer, StringComparison.Ordinal);
        Assert.Contains("#if FORCE_DOTNET10_MISSING", installer, StringComparison.Ordinal);
    }

    private static IReadOnlyDictionary<string, string> ReadCustomMessages()
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var rawLine in File.ReadLines(InstallerPath("Subtitle_Edit_Localization.iss")))
        {
            var line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith(';') || line.StartsWith('['))
                continue;
            var separator = line.IndexOf('=');
            if (separator <= 0)
                continue;
            result.Add(line[..separator].Trim(), line[(separator + 1)..]);
        }
        return result;
    }

    private static string InstallerPath(string fileName) =>
        Path.Combine(LocalizationTestPaths.RepositoryRoot(), "installer", "WindowsInno", fileName);
}
