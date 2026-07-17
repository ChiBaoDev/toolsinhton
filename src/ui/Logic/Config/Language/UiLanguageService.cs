using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nikse.SubtitleEdit.Logic.Config.Language;

internal sealed record LanguageChange(string OldName, string NewName, SeLanguage Language, bool DirectionChanged);
internal interface IUiLanguageChangeSink { Task ApplyAsync(LanguageChange change, CancellationToken cancellationToken = default); }
internal sealed record LanguageApplyResult(bool Success, string ActiveLanguageName, LanguageLoadFailure Failure, string? Diagnostic);
internal interface IUiLanguageService { Task<LanguageApplyResult> TryApplyAsync(string languageName, CancellationToken cancellationToken = default); }

internal sealed class UiLanguageServiceFactory
{
    private readonly ILanguageCandidateLoader _loader;

    internal UiLanguageServiceFactory(ILanguageCandidateLoader loader)
    {
        _loader = loader;
    }

    internal IUiLanguageService Create(IUiLanguageChangeSink sink) =>
        new UiLanguageService(_loader, sink);
}

internal sealed class UiLanguageService : IUiLanguageService
{
    private readonly ILanguageCandidateLoader _loader;
    private readonly IUiLanguageChangeSink _sink;
    private readonly string _settingsPath;

    public UiLanguageService(ILanguageCandidateLoader loader, IUiLanguageChangeSink sink)
        : this(loader, sink, Se.GetSettingsFilePath())
    {
    }

    internal UiLanguageService(
        ILanguageCandidateLoader loader,
        IUiLanguageChangeSink sink,
        string settingsPath)
    {
        _loader = loader;
        _sink = sink;
        _settingsPath = settingsPath;
    }

    public async Task<LanguageApplyResult> TryApplyAsync(
        string languageName,
        CancellationToken cancellationToken = default)
    {
        var loaded = _loader.TryLoad(languageName);
        var oldName = Se.Settings.General.Language ?? "English";
        if (!loaded.Success || loaded.Language == null)
        {
            return new(false, oldName, loaded.Failure, loaded.Diagnostic);
        }

        Se candidate;
        try
        {
            candidate = Se.CloneSettings(Se.Settings);
            candidate.General.Language = languageName;
            Se.SaveSettings(_settingsPath, candidate);
        }
        catch (Exception ex)
        {
            return new(false, oldName, LanguageLoadFailure.ResourceUnavailable, ex.Message);
        }

        Se.Settings = candidate;
        Se.Language = loaded.Language;
        Se.UpdateLibSeSettingsAfterExternalCommit();

        var change = new LanguageChange(
            oldName,
            languageName,
            loaded.Language,
            IsRightToLeft(oldName) != IsRightToLeft(languageName));
        await _sink.ApplyAsync(change, cancellationToken);

        return new(true, languageName, LanguageLoadFailure.None, null);
    }

    private static bool IsRightToLeft(string name) =>
        name is "Arabic" or "Dari" or "Hebrew" or "Kurdish" or "Pashto" or
            "Persian" or "Sindhi" or "Urdu" or "Yiddish";
}
