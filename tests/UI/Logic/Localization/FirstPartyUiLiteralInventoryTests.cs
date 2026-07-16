using System.Text.Json;
using System.Text.RegularExpressions;
using Nikse.SubtitleEdit.Core.VobSub;
using Nikse.SubtitleEdit.Features.Shared.PickVobSubLanguage;
using Nikse.SubtitleEdit.Logic.Config;
using SkiaSharp;
using Nikse.SubtitleEdit.Logic.Config.Language;

namespace UITests.Logic.Localization;

internal sealed record Task10Inventory(string[] Roots, Task10InventoryRow[] Rows);

internal sealed record Task10InventoryRow(
    string Source, int Line, string Literal, string Classification, string? Category,
    string? Reason, string? LanguageKey, string? SourceExpression)
{
    public string Identity => $"{Source}:{Line}:{Literal}";
}

internal sealed record Task10Candidate(string Source, int Line, string Literal)
{
    public string Identity => $"{Source}:{Line}:{Literal}";
}

public class FirstPartyUiLiteralInventoryTests
{
    [Fact]
    public void TargetedSourceCandidatesMatchStructuredInventory()
    {
        var root = RepositoryRoot();
        var inventory = LoadInventory(root);
        var actual = ScanCandidates(root, inventory.Roots);
        var classified = inventory.Rows.Select(row => row.Identity).ToHashSet(StringComparer.Ordinal);

        var unclassified = actual.Where(candidate => !classified.Contains(candidate.Identity)).ToArray();
        Assert.True(unclassified.Length == 0, "Unclassified first-party literal candidates: " + string.Join("; ", unclassified.Select(Describe)));

        var stale = inventory.Rows.Where(row => row.Classification != "localized" && !actual.Any(candidate => candidate.Identity == row.Identity)).ToArray();
        Assert.True(stale.Length == 0, "Stale inventory classifications: " + string.Join("; ", stale.Select(row => $"{row.Source}:{row.Line}: {row.Literal}")));

        foreach (var row in inventory.Rows.Where(row => row.Classification == "localized"))
        {
            Assert.False(string.IsNullOrWhiteSpace(row.LanguageKey));
            Assert.StartsWith("$.", row.LanguageKey, StringComparison.Ordinal);
            Assert.True(CatalogPathExists(root, row.LanguageKey), $"Unknown English catalog path {row.LanguageKey} for {row.Source}");
            Assert.StartsWith("Se.Language.", row.SourceExpression, StringComparison.Ordinal);
            Assert.Contains(row.SourceExpression!, File.ReadAllText(Path.Combine(root, row.Source.Replace('/', Path.DirectorySeparatorChar))), StringComparison.Ordinal);
        }

        foreach (var row in inventory.Rows.Where(row => row.Classification != "localized"))
        {
            Assert.False(string.IsNullOrWhiteSpace(row.Category));
            Assert.False(string.IsNullOrWhiteSpace(row.Reason));
        }
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


    private static Task10Inventory LoadInventory(string root) =>
        JsonSerializer.Deserialize<Task10Inventory>(File.ReadAllText(Path.Combine(root, "tests", "UI", "TestData", "Task10LiteralInventory.json")), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
        ?? throw new InvalidDataException("Could not load Task 10 literal inventory.");

    private static Task10Candidate[] ScanCandidates(string root, IReadOnlyList<string> roots)
    {
        var candidates = new List<Task10Candidate>();
        foreach (var relativeRoot in roots)
        {
            var absoluteRoot = Path.Combine(root, relativeRoot.Replace('/', Path.DirectorySeparatorChar));
            foreach (var file in Directory.EnumerateFiles(absoluteRoot, "*.cs", SearchOption.AllDirectories))
            {
                var source = Path.GetRelativePath(root, file).Replace(Path.DirectorySeparatorChar, '/');
                var lines = File.ReadAllLines(file);
                for (var lineNumber = 0; lineNumber < lines.Length; lineNumber++)
                {
                    var line = lines[lineNumber];
                    if (line.Contains("Header =", StringComparison.Ordinal))
                    {
                        var literal = ExtractFirstLiteral(line);
                        if (literal is not null) candidates.Add(new Task10Candidate(source, lineNumber + 1, literal));
                    }
                    if (line.Contains("ShowHelp(", StringComparison.Ordinal) || line.Contains("InvalidOperationException(", StringComparison.Ordinal))
                    {
                        foreach (var literal in ExtractLiterals(line))
                            candidates.Add(new Task10Candidate(source, lineNumber + 1, literal));
                    }
                }
            }
        }
        return candidates.ToArray();
    }

    private static string? ExtractFirstLiteral(string line)
    {
        var start = line.IndexOf('"');
        if (start < 0) return null;
        var end = line.IndexOf('"', start + 1);
        return end > start ? line[(start + 1)..end] : null;
    }

    private static IEnumerable<string> ExtractLiterals(string line)
    {
        var start = 0;
        while ((start = line.IndexOf('"', start)) >= 0)
        {
            var end = line.IndexOf('"', start + 1);
            if (end < 0) yield break;
            yield return line[(start + 1)..end];
            start = end + 1;
        }
    }

    private static bool CatalogPathExists(string root, string path)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "src", "ui", "Assets", "Languages", "English.json")));
        var current = document.RootElement;
        foreach (var segment in path[2..].Split('.'))
            if (!current.TryGetProperty(segment, out current)) return false;
        return current.ValueKind == JsonValueKind.String;
    }

    private static string Describe(Task10Candidate candidate) => $"{candidate.Source}:{candidate.Line}: {candidate.Literal}";

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
