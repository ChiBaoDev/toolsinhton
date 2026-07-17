using System.Text.Json;
using System.Text.RegularExpressions;
using Avalonia.Headless.XUnit;
using Nikse.SubtitleEdit.Core.VobSub;
using Nikse.SubtitleEdit.Features.Shared.PickVobSubLanguage;
using Nikse.SubtitleEdit.Logic.Config;
using Nikse.SubtitleEdit.Logic.Config.Language;
using SkiaSharp;

namespace UITests.Logic.Localization;

internal sealed record Task10Inventory(string[] Roots, Task10InventoryRow[] Rows);

internal sealed record Task10InventoryRow(
    string Source, int Line, string Literal, string Classification, string? Category,
    string? Reason, string? LanguageKey, string? SourceExpression)
{
    public string Identity => $"{Source}:{Line}:{Literal}";
}

internal sealed record Task10Candidate(string Source, int Line, string Kind, string? Literal, string? SourceExpression)
{
    public string Identity => $"{Source}:{Line}:{Kind}:{Literal}:{SourceExpression}";
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

    private static readonly Regex UiPropertyAssignment = new(
        @"\b(?<kind>[A-Za-z0-9_]*Title|Content|Header|Watermark)\s*=\s*(?:(?<literal>""(?:\\.|[^""\\])*"")|(?<expression>Se\.Language\.[A-Za-z0-9_.]+))",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex MarkupUiAttribute = new(
        @"\b(?<kind>Title|Content|Header|Watermark)\s*=\s*""(?<literal>[^""]+)""",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex UiCall = new(
        @"(?<kind>ToolTip\.SetTip|(?:[A-Za-z0-9_]+\.)?(?:ShowMessageBox|ShowToast|ShowNotification|ShowMessage|MessageBox\.Show|MessageBox|Toast|Notification)|UiUtil\.(?:Show|Display)[A-Za-z0-9_]*|throw\s+new\s+[A-Za-z0-9_]*Exception)\s*\((?<arguments>[^;]*?)\)",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);

    private static readonly Regex StringLiteral = new(
        @"""(?<literal>(?:\\.|[^""\\])*)""",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    [Fact]
    public void TargetedSourceCandidatesMatchStructuredInventory()
    {
        var root = RepositoryRoot();
        var inventory = LoadInventory(root);

        Assert.Equal(RequiredRoots, inventory.Roots);
        Assert.All(RequiredRoots, relativeRoot =>
            Assert.True(Directory.Exists(ToAbsolutePath(root, relativeRoot)), $"Required root does not exist: {relativeRoot}"));
        Assert.All(inventory.Rows, row =>
            Assert.Contains(RequiredRoots, requiredRoot => row.Source.StartsWith(requiredRoot + "/", StringComparison.Ordinal)));
        Assert.All(inventory.Rows, row =>
            Assert.Contains(Path.GetExtension(row.Source), new[] { ".cs", ".xaml", ".axaml" }));
        Assert.Equal(inventory.Rows.Length, inventory.Rows.Select(row => row.Identity).Distinct(StringComparer.Ordinal).Count());

        var candidates = ScanCandidates(root, RequiredRoots);
        Assert.Equal(candidates.Length, candidates.Select(candidate => candidate.Identity).Distinct(StringComparer.Ordinal).Count());
        var retainedRows = inventory.Rows.Where(row => row.Classification != "localized").ToArray();

        var candidateMatches = candidates
            .Select(candidate => (candidate, rows: retainedRows.Where(row => CandidateMatches(row, candidate)).ToArray()))
            .ToArray();
        var incorrectlyClassifiedCandidates = candidateMatches.Where(match => match.rows.Length != 1).ToArray();
        Assert.True(
            incorrectlyClassifiedCandidates.Length == 0,
            "Candidates without exactly one inventory row: " + string.Join("; ", incorrectlyClassifiedCandidates.Select(match => $"{Describe(match.candidate)} ({match.rows.Length} rows)")));

        var rowMatches = retainedRows
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

            var sourceText = File.ReadAllText(ToAbsolutePath(root, row.Source));
            var localizedCandidate = ScanLocalizedCandidate(row.Source, sourceText, row.SourceExpression!);
            Assert.Equal(row.Line, localizedCandidate.Line);
        }

        foreach (var row in inventory.Rows.Where(row => row.Classification != "localized"))
        {
            Assert.False(string.IsNullOrWhiteSpace(row.Category));
            Assert.False(string.IsNullOrWhiteSpace(row.Reason));
            Assert.True(string.IsNullOrWhiteSpace(row.LanguageKey));
            Assert.True(string.IsNullOrWhiteSpace(row.SourceExpression));
        }
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

    private static SeLanguage LoadVietnameseLanguage() =>
        JsonSerializer.Deserialize<SeLanguage>(
            File.ReadAllText(ToAbsolutePath(RepositoryRoot(), "src/ui/Assets/Languages/Vietnamese.json")),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
        ?? throw new InvalidDataException("Could not load Vietnamese language catalog.");

    private static Task10Inventory LoadInventory(string root) =>
        JsonSerializer.Deserialize<Task10Inventory>(
            File.ReadAllText(ToAbsolutePath(root, "tests/UI/TestData/Task10LiteralInventory.json")),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
        ?? throw new InvalidDataException("Could not load Task 10 literal inventory.");

    private static Task10Candidate[] ScanCandidates(string root, IReadOnlyList<string> roots)
    {
        var candidates = new List<Task10Candidate>();
        foreach (var relativeRoot in roots)
        {
            foreach (var file in Directory.EnumerateFiles(ToAbsolutePath(root, relativeRoot), "*.*", SearchOption.AllDirectories)
                         .Where(IsScannedSourceFile)
                         .OrderBy(file => file, StringComparer.OrdinalIgnoreCase))
            {
                var source = Path.GetRelativePath(root, file).Replace(Path.DirectorySeparatorChar, '/');
                var text = File.ReadAllText(file);
                if (file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                {
                    AddCSharpCandidates(candidates, source, text);
                }
                else
                {
                    AddMarkupCandidates(candidates, source, text);
                }
            }
        }

        return candidates.ToArray();
    }

    private static void AddCSharpCandidates(List<Task10Candidate> candidates, string source, string text)
    {
        foreach (Match match in UiPropertyAssignment.Matches(text))
        {
            var literal = Unquote(match.Groups["literal"].Value);
            if (literal is not null)
            {
                AddCandidate(candidates, source, text, match.Index, match.Groups["kind"].Value, literal, null);
            }
        }

        foreach (Match call in UiCall.Matches(text))
        {
            var arguments = call.Groups["arguments"];
            foreach (Match literal in StringLiteral.Matches(arguments.Value))
            {
                AddCandidate(candidates, source, text, arguments.Index + literal.Index, call.Groups["kind"].Value,
                    literal.Groups["literal"].Value, null);
            }

        }
    }

    private static void AddMarkupCandidates(List<Task10Candidate> candidates, string source, string text)
    {
        foreach (Match match in MarkupUiAttribute.Matches(text))
        {
            AddCandidate(candidates, source, text, match.Index, match.Groups["kind"].Value, match.Groups["literal"].Value, null);
        }
    }

    private static void AddCandidate(List<Task10Candidate> candidates, string source, string text, int index, string kind, string? literal, string? sourceExpression)
    {
        if (string.IsNullOrEmpty(literal) && string.IsNullOrEmpty(sourceExpression))
        {
            return;
        }

        candidates.Add(new Task10Candidate(source, LineNumber(text, index), kind, literal, sourceExpression));
    }

    private static Task10Candidate ScanLocalizedCandidate(string source, string text, string sourceExpression)
    {
        var matches = text.Split("\n")
            .Select((line, index) => (line, index))
            .Where(item => item.line.Contains(sourceExpression, StringComparison.Ordinal) &&
                           (Regex.IsMatch(item.line, @"\b(?:[A-Za-z0-9_]*Title|Content|Header|Watermark)\s*=", RegexOptions.CultureInvariant) ||
                            item.line.Contains("UiUtil.", StringComparison.Ordinal)))
            .Select(item => new Task10Candidate(source, item.index + 1, "localized-source", null, sourceExpression))
            .ToArray();
        Assert.True(matches.Length == 1, $"Localized expression is not a unique direct UI assignment: {source}:{sourceExpression} ({matches.Length})");
        return matches[0];
    }

    private static bool CandidateMatches(Task10InventoryRow row, Task10Candidate candidate) =>
        row.Source == candidate.Source &&
        row.Line == candidate.Line &&
        (row.Classification == "localized" ? row.SourceExpression == candidate.SourceExpression : row.Literal == candidate.Literal);

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

    private static string? Unquote(string value)
    {
        var quote = value.IndexOf('"');
        return quote >= 0 && value.Length > quote + 1 ? value[(quote + 1)..^1] : null;
    }


    private static string Describe(Task10Candidate candidate) =>
        $"{candidate.Source}:{candidate.Line}:{candidate.Kind}: {candidate.Literal ?? candidate.SourceExpression}";

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
