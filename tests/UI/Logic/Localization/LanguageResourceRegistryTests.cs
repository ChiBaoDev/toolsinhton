using Nikse.SubtitleEdit.Logic.Initializers;

namespace UITests.Logic.Localization;

public class LanguageResourceRegistryTests
{
    [Fact]
    public void SourceProjectAndInitializerLanguageSets_AreEqualAndDuplicateFree()
    {
        var sourceNames = LocalizationTestPaths.LanguageFiles()
            .Select(Path.GetFileNameWithoutExtension).OrderBy(x => x, StringComparer.Ordinal).ToArray();
        var projectNames = LocalizationTestPaths.AvaloniaLanguageResources()
            .Select(Path.GetFileNameWithoutExtension).OrderBy(x => x, StringComparer.Ordinal).ToArray();
        var initializerNames = LanguageInitializer.BuiltInLanguageNames
            .OrderBy(x => x, StringComparer.Ordinal).ToArray();

        Assert.Equal(sourceNames.Distinct(StringComparer.Ordinal), sourceNames);
        Assert.Equal(projectNames.Distinct(StringComparer.Ordinal), projectNames);
        Assert.Equal(sourceNames, projectNames);
        Assert.Equal(sourceNames, initializerNames);
    }

}
