using System.Text.Json;
using Avalonia.Headless.XUnit;
using Avalonia.Platform;

namespace UITests.Logic.Localization;

public class EmbeddedLanguageResourceTests
{
    public static TheoryData<string> BuiltInLanguageNames()
    {
        var data = new TheoryData<string>();
        foreach (var name in LocalizationTestPaths.LanguageFiles()
                     .Select(path => Path.GetFileNameWithoutExtension(path)!)
                     .OrderBy(x => x, StringComparer.Ordinal))
        {
            data.Add(name);
        }

        return data;
    }

    [AvaloniaTheory]
    [MemberData(nameof(BuiltInLanguageNames))]
    public void EmbeddedLanguageResource_CanBeOpenedAndParsed(string name)
    {
        var uri = new Uri($"avares://SubtitleEdit/Assets/Languages/{name}.json");
        using var stream = AssetLoader.Open(uri);
        using var document = JsonDocument.Parse(stream);

        Assert.Equal(JsonValueKind.Object, document.RootElement.ValueKind);
    }
}
