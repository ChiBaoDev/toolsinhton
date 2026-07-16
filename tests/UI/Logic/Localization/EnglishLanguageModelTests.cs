using System.Text.Json;
using Nikse.SubtitleEdit.Logic.Config.Language;

namespace UITests.Logic.Localization;

public class EnglishLanguageModelTests
{
    [Fact]
    public void EnglishJsonAndDefaultLanguageModel_HaveTheSameShape()
    {
        using var english = LocalizationJsonHelper.LoadDocument(LocalizationTestPaths.EnglishLanguageFile());
        var modelJson = JsonSerializer.Serialize(new SeLanguage(), new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });
        using var model = JsonDocument.Parse(modelJson);

        var englishToModel = LocalizationJsonHelper.CompareNodeShape(english.RootElement, model.RootElement);
        var modelToEnglish = LocalizationJsonHelper.CompareNodeShape(model.RootElement, english.RootElement);

        Assert.True(englishToModel.Count == 0, string.Join(Environment.NewLine, englishToModel));
        Assert.True(modelToEnglish.Count == 0, string.Join(Environment.NewLine, modelToEnglish));
    }
}
