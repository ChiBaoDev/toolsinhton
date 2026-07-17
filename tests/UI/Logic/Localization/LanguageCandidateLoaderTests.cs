using Avalonia.Headless.XUnit;
using Avalonia.Platform;
using Nikse.SubtitleEdit.Logic.Config;
using Nikse.SubtitleEdit.Logic.Config.Language;
namespace UITests.Logic.Localization;
public sealed class LanguageCandidateLoaderTests
{
    [AvaloniaFact] public void WritableValidFileWinsOverEmbedded() => WithFolder(f=>{File.WriteAllText(Path.Combine(f,"Vietnamese.json"),Embedded());var r=new LanguageCandidateLoader(f).TryLoad("Vietnamese");Assert.True(r.Success);Assert.Equal(LanguageSource.WritableFile,r.Source);});
    [AvaloniaTheory][InlineData(false)][InlineData(true)] public void MissingOrMalformedVietnameseUsesEmbedded(bool bad)=>WithFolder(f=>{if(bad)File.WriteAllText(Path.Combine(f,"Vietnamese.json"),"{");var r=new LanguageCandidateLoader(f).TryLoad("Vietnamese");Assert.True(r.Success);Assert.Equal(LanguageSource.EmbeddedResource,r.Source);});
    [AvaloniaTheory][InlineData(false,"Missing")][InlineData(true,"MalformedJson")] public void OtherFailureIsTyped(bool bad,string failure)=>WithFolder(f=>{if(bad)File.WriteAllText(Path.Combine(f,"Nope.json"),"{");var r=new LanguageCandidateLoader(f).TryLoad("Nope");Assert.False(r.Success);Assert.Equal(failure,r.Failure.ToString());});
    [AvaloniaFact] public void EnglishAlwaysLoadsWithoutMutation(){var s=Se.Settings;var l=Se.Language;var r=new LanguageCandidateLoader(Path.GetTempPath()).TryLoad("English");Assert.True(r.Success);Assert.Equal(LanguageSource.BuiltInEnglish,r.Source);Assert.Same(s,Se.Settings);Assert.Same(l,Se.Language);}
    [AvaloniaFact] public void InvalidShapeIsRejected()=>WithFolder(f=>{File.WriteAllText(Path.Combine(f,"Bad.json"),"{\"title\":\"x\"}");var r=new LanguageCandidateLoader(f).TryLoad("Bad");Assert.False(r.Success);Assert.Equal(LanguageLoadFailure.InvalidShape,r.Failure);});
    private static string Embedded(){using var s=AssetLoader.Open(new Uri("avares://SubtitleEdit/Assets/Languages/Vietnamese.json"));using var r=new StreamReader(s);return r.ReadToEnd();}
    private static void WithFolder(Action<string>a){var f=Path.Combine(Path.GetTempPath(),"SE12",Guid.NewGuid().ToString("N"));Directory.CreateDirectory(f);try{a(f);}finally{Directory.Delete(f,true);}}
}
