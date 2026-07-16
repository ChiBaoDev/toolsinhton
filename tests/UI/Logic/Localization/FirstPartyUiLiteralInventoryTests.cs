using System.Text.Json;
using Nikse.SubtitleEdit.Logic.Config;
using Nikse.SubtitleEdit.Logic.Config.Language;

namespace UITests.Logic.Localization;

public class FirstPartyUiLiteralInventoryTests
{
    [Fact]
    public void TargetedSourceContainsNoKnownFirstPartyEnglishLiterals()
    {
        var root = RepositoryRoot();
        var candidates = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["src/ui/Features/Main/Layout/LayoutWindow.cs"] = ["Choose layout"],
            ["src/ui/Features/Files/ExportPac/ExportPacWindow.cs"] = ["Export Pac", "Choose PAC code page"],
            ["src/ui/Features/Files/ExportCavena890/ExportCavena890Window.cs"] = ["Export Cavena 890"],
            ["src/ui/Features/Files/ExportEbuStl/ExportEbuStlWindow.cs"] = ["Export EBU STL"],
            ["src/ui/Features/Shared/DownloadFfmpegWindow.cs"] = ["Downloading ffmpeg"],
            ["src/ui/Features/Shared/DownloadLibMpvWindow.cs"] = ["Downloading libmpv"],
            ["src/ui/Features/Shared/BinaryEdit/SetText/SetTextWindow.cs"] = ["Set Text"],
            ["src/ui/Features/Shared/BinaryEdit/BinaryApplyDurationLimits/BinaryApplyDurationLimitsWindow.cs"] = ["Minimum duration (milliseconds):", "Maximum duration (milliseconds):"],
        };
        var remaining = candidates.SelectMany(pair => pair.Value.Select(literal => (Path: pair.Key, Literal: literal)))
            .Where(candidate => File.ReadAllText(Path.Combine(root, candidate.Path)).Contains(candidate.Literal, StringComparison.Ordinal))
            .Select(candidate => $"{candidate.Path}: {candidate.Literal}").ToArray();
        Assert.True(remaining.Length == 0, "Unlocalized first-party literals: " + string.Join("; ", remaining));
    }

    [Fact]
    public void LocalizedRuntimeValuesAreVietnamese()
    {
        var previous = Se.Language;
        Se.Language = System.Text.Json.JsonSerializer.Deserialize<SeLanguage>(File.ReadAllText(Path.Combine(RepositoryRoot(), "src", "ui", "Assets", "Languages", "Vietnamese.json")), new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        try
        {
            using var document = System.Text.Json.JsonDocument.Parse(File.ReadAllText(Path.Combine(RepositoryRoot(), "src", "ui", "Assets", "Languages", "Vietnamese.json")));
        Assert.Equal("Chọn bố cục", document.RootElement.GetProperty("main").GetProperty("layoutTitle").GetString());
        Assert.Equal("Chọn bố cục", Se.Language.Main.LayoutTitle);
        Assert.Equal("Xuất PAC", Se.Language.File.ExportPacTitle);
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

    private static string RepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "SubtitleEdit.sln"))) return directory.FullName;
            directory = directory.Parent;
        }
        throw new DirectoryNotFoundException("Could not locate repository root.");
    }
}
