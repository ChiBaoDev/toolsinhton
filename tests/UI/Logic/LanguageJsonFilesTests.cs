using System.Text.Json;

namespace UITests.Logic;

/// <summary>
/// Validates that every translation file shipped in <c>src/ui/Assets/Languages</c> is well-formed
/// JSON, so a broken language file is caught here rather than at application start-up.
/// </summary>
public class LanguageJsonFilesTests
{
    /// <summary>One theory case per <c>*.json</c> file in the Languages folder.</summary>
    public static TheoryData<string> LanguageFileNames()
    {
        var data = new TheoryData<string>();
        foreach (var path in LocalizationTestPaths.LanguageFiles())
        {
            data.Add(Path.GetFileName(path));
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(LanguageFileNames))]
    public void LanguageFile_IsValidJson(string fileName)
    {
        var path = Path.Combine(LocalizationTestPaths.LanguagesFolder(), fileName);

        // File.ReadAllText strips a UTF-8 BOM; the language files are saved with one.
        var json = File.ReadAllText(path);

        var exception = Record.Exception(() => JsonDocument.Parse(json).Dispose());

        Assert.True(exception is null, $"{fileName} is not valid JSON: {exception?.Message}");
    }

    [Fact]
    public void LanguagesFolder_ContainsLanguageFiles()
    {
        var files = LocalizationTestPaths.LanguageFiles();

        Assert.NotEmpty(files);
        Assert.Contains(files, f => Path.GetFileName(f) == "English.json");
    }

}

internal static class LocalizationTestPaths
{
    internal static string[] LanguageFiles() =>
        Directory.GetFiles(LanguagesFolder(), "*.json");

    internal static string EnglishLanguageFile() =>
        Language("English.json");

    internal static string Language(string fileName) =>
        Path.Combine(LanguagesFolder(), fileName);

    internal static string[] AvaloniaLanguageResources()
    {
        var projectFile = Path.Combine(RepositoryRoot(), "src", "ui", "UI.csproj");
        var document = System.Xml.Linq.XDocument.Load(projectFile);
        return document.Descendants("AvaloniaResource")
            .Select(element => (string?)element.Attribute("Include"))
            .Where(include => include is not null &&
                              include.Replace('\\', '/').StartsWith("Assets/Languages/", StringComparison.Ordinal) &&
                              include.EndsWith(".json", StringComparison.Ordinal))
            .Select(include => Uri.UnescapeDataString(include!.Replace('\\', '/')))
            .ToArray();
    }

    internal static string LanguagesFolder() =>
        Path.Combine(RepositoryRoot(), "src", "ui", "Assets", "Languages");

    internal static string RepositoryRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "SubtitleEdit.sln")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException(
            $"Could not locate the repository root walking up from '{AppContext.BaseDirectory}'.");
    }
}
