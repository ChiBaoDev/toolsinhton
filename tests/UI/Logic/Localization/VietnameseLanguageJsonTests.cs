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
    public void Vietnamese_HasOnlyNonEmptyStringLeaves()
    {
        using var vietnamese = LocalizationJsonHelper.LoadDocument(LocalizationTestPaths.Language("Vietnamese.json"));
        LocalizationJsonHelper.RequireStringLeaves(vietnamese.RootElement);
        Assert.DoesNotContain(LocalizationJsonHelper.FlattenLeaves(vietnamese.RootElement).Values, leaf => string.IsNullOrWhiteSpace(leaf.StringValue));
    }
}
