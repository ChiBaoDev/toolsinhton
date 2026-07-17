using Avalonia.Headless.XUnit;
using Avalonia.Platform;
using Nikse.SubtitleEdit.Logic.Config;
using Nikse.SubtitleEdit.Logic.Config.Language;
namespace UITests.Logic.Localization;
public sealed class StartupLanguageTests
{
    [AvaloniaFact] public void MissingSettingsDefaultsToVietnamese() => Run(null, "Vietnamese", false);
    [AvaloniaTheory] [InlineData("English")] [InlineData("French")]
    public void ExistingAvailableChoiceIsPreserved(string name) => Run("{\"general\":{\"language\":\"" + name + "\"}}", name, true);
    [AvaloniaTheory] [InlineData("{\"general\":{}}")][InlineData("{\"general\":{\"language\":\"\"}}")][InlineData("{\"general\":{\"language\":\"Unavailable\"}}")]
    public void LegacyInvalidChoiceFallsBackToEnglishWithoutRewrite(string json) => Run(json, "English", true);
    private static void Run(string? json, string expected, bool preserve)
    {
        var os=Se.Settings; var ol=Se.Language; var op=Se.SettingsFilePathOverride; var d=Path.Combine(Path.GetTempPath(),"SE12",Guid.NewGuid().ToString("N")); var p=Path.Combine(d,"Settings.json"); Directory.CreateDirectory(d);
        try { if(json!=null)File.WriteAllText(p,json); if(json?.Contains("French") == true){var lf=Path.Combine(d,"Languages");Directory.CreateDirectory(lf);using var rs=AssetLoader.Open(new Uri("avares://SubtitleEdit/Assets/Languages/French.json"));using var fs=File.Create(Path.Combine(lf,"French.json"));rs.CopyTo(fs);} Se.SettingsFilePathOverride=p; Se.Settings=new Se(); Se.Language=new SeLanguage(); Se.LoadSettings(p); Se.LoadStartupLanguage(new LanguageCandidateLoader(Path.Combine(d,"Languages"))); Assert.Equal(expected,Se.Settings.General.Language); if(preserve)Assert.Equal(json,File.ReadAllText(p)); }
        finally { Se.Settings=os; Se.Language=ol; Se.SettingsFilePathOverride=op; Directory.Delete(d,true); }
    }
}
