using System.Diagnostics;
using System.Text.Json;

namespace UITests.Logic.Localization;

public class VietnameseLanguageJsonTests
{
    [Fact]
    public void Vietnamese_HasExactEnglishShape()
    {
        using var english = LocalizationJsonHelper.LoadDocument(LocalizationTestPaths.Language("English.json"));
        using var vietnamese = LocalizationJsonHelper.LoadDocument(LocalizationTestPaths.Language("Vietnamese.json"));
        Assert.Empty(LocalizationJsonHelper.CompareNodeShape(english.RootElement, vietnamese.RootElement));
    }

    [Fact]
    public void Vietnamese_MetadataIsCorrect()
    {
        using var vietnamese = LocalizationJsonHelper.LoadDocument(LocalizationTestPaths.Language("Vietnamese.json"));
        Assert.Equal("vi-VN", vietnamese.RootElement.GetProperty("cultureName").GetString());
        Assert.Equal("Subtitle Edit", vietnamese.RootElement.GetProperty("title").GetString());
        Assert.False(string.IsNullOrWhiteSpace(vietnamese.RootElement.GetProperty("translatedBy").GetString()));
    }

    [Fact]
    public void Vietnamese_HasOnlyNonEmptyStringLeavesAndMatchingPlaceholders()
    {
        using var english = LocalizationJsonHelper.LoadDocument(LocalizationTestPaths.Language("English.json"));
        using var vietnamese = LocalizationJsonHelper.LoadDocument(LocalizationTestPaths.Language("Vietnamese.json"));
        LocalizationJsonHelper.RequireStringLeaves(vietnamese.RootElement);
        var englishLeaves = LocalizationJsonHelper.FlattenLeaves(english.RootElement);
        var vietnameseLeaves = LocalizationJsonHelper.FlattenLeaves(vietnamese.RootElement);
        var errors = new List<string>();

        foreach (var (path, translated) in vietnameseLeaves)
        {
            if (string.IsNullOrWhiteSpace(translated.StringValue))
                errors.Add($"{path}: value is empty.");
            try
            {
                foreach (var error in CompositeFormatPlaceholderParser.Compare(englishLeaves[path].StringValue!, translated.StringValue!))
                    errors.Add($"{path}: {error}");
            }
            catch (FormatException exception)
            {
                errors.Add($"{path}: invalid composite format: {exception.Message}");
            }
        }

        Assert.True(errors.Count == 0, string.Join(Environment.NewLine, errors));
    }

    [Fact]
    public void Vietnamese_UsesEveryExactEnglishAllowlistEntryExactlyOnce()
    {
        using var english = LocalizationJsonHelper.LoadDocument(LocalizationTestPaths.Language("English.json"));
        using var vietnamese = LocalizationJsonHelper.LoadDocument(LocalizationTestPaths.Language("Vietnamese.json"));
        var englishLeaves = LocalizationJsonHelper.FlattenLeaves(english.RootElement);
        var vietnameseLeaves = LocalizationJsonHelper.FlattenLeaves(vietnamese.RootElement);
        var allowlistPath = Path.Combine(AppContext.BaseDirectory, "TestData", "VietnameseUntranslatedAllowlist.json");
        var allowlist = JsonSerializer.Deserialize<UntranslatedAllowlistEntry[]>(File.ReadAllText(allowlistPath),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        var errors = new List<string>();
        var identicalPaths = englishLeaves.Keys
            .Where(path => string.Equals(englishLeaves[path].StringValue, vietnameseLeaves[path].StringValue, StringComparison.Ordinal))
            .ToArray();

        foreach (var duplicate in allowlist.GroupBy(entry => (entry.Key, entry.Value)).Where(group => group.Count() != 1))
            errors.Add($"{duplicate.Key.Key}: duplicate exact allowlist entry.");
        foreach (var path in identicalPaths)
        {
            var matches = allowlist.Where(entry => string.Equals(entry.Key, path, StringComparison.Ordinal) &&
                                                   string.Equals(entry.Value, vietnameseLeaves[path].StringValue, StringComparison.Ordinal)).ToArray();
            if (matches.Length != 1)
                errors.Add($"{path}: identical English value must have exactly one exact allowlist entry.");
        }
        foreach (var entry in allowlist)
        {
            if (!identicalPaths.Contains(entry.Key, StringComparer.Ordinal) ||
                !string.Equals(vietnameseLeaves[entry.Key].StringValue, entry.Value, StringComparison.Ordinal))
                errors.Add($"{entry.Key}: allowlist entry is stale or not exact.");
        }

        Assert.True(errors.Count == 0, string.Join(Environment.NewLine, errors));
    }

    [Fact]
    public void Vietnamese_CheckedInCatalogMatchesDeterministicMergeOutput()
    {
        var repositoryRoot = LocalizationTestPaths.RepositoryRoot();
        var script = Path.Combine(repositoryRoot, "tools", "localization", "Merge-VietnameseLanguage.ps1");
        var output = Path.Combine(Path.GetTempPath(), $"Vietnamese-{Guid.NewGuid():N}.json");
        try
        {
            var startInfo = new ProcessStartInfo(FindPowerShell())
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            startInfo.ArgumentList.Add("-NoProfile");
            startInfo.ArgumentList.Add("-File");
            startInfo.ArgumentList.Add(script);
            startInfo.ArgumentList.Add("-OutputPath");
            startInfo.ArgumentList.Add(output);
            using var process = Process.Start(startInfo)!;
            var standardOutput = process.StandardOutput.ReadToEnd();
            var standardError = process.StandardError.ReadToEnd();
            process.WaitForExit();
            Assert.True(process.ExitCode == 0, $"Merge exited {process.ExitCode}.{Environment.NewLine}{standardOutput}{standardError}");

            Assert.Equal(NormalizeNewlines(File.ReadAllText(LocalizationTestPaths.Language("Vietnamese.json"))),
                NormalizeNewlines(File.ReadAllText(output)));
        }
        finally
        {
            if (File.Exists(output))
                File.Delete(output);
        }
    }

    private static string FindPowerShell()
    {
        var candidates = new[]
        {
            "pwsh",
            "powershell.exe",
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "WindowsPowerShell", "v1.0", "powershell.exe"),
            "powershell",
        };
        foreach (var candidate in candidates)
        {
            try
            {
                using var process = Process.Start(new ProcessStartInfo(candidate, "-NoProfile -Command exit") { UseShellExecute = false });
                process!.WaitForExit();
                if (process.ExitCode == 0)
                    return candidate;
            }
            catch
            {
                // Try the next supported Windows PowerShell executable.
            }
        }
        throw new FileNotFoundException("Neither pwsh nor Windows PowerShell could be located.");
    }

    private static string NormalizeNewlines(string value) =>
        value.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');
}
