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

internal static class UiLanguageDirection
{
    private static readonly string[] RightToLeftLanguages =
    [
        "Arabic", "Dari", "Hebrew", "Kurdish", "Pashto",
        "Persian", "Sindhi", "Urdu", "Yiddish",
    ];

    internal static bool IsRightToLeft(string? name) =>
        Array.Exists(RightToLeftLanguages, language =>
            string.Equals(language, name, StringComparison.OrdinalIgnoreCase));
}

internal sealed class UiLanguageService : IUiLanguageService
{
    private readonly ILanguageCandidateLoader _loader;
    private readonly IUiLanguageChangeSink _sink;
    private readonly string _settingsPath;
    private readonly Action<string, Se> _persistSettings;
    private readonly Action _updateLibSeSettings;
    private static readonly SemaphoreSlim TransactionLock = new(1, 1);

    public UiLanguageService(ILanguageCandidateLoader loader, IUiLanguageChangeSink sink)
        : this(loader, sink, Se.GetSettingsFilePath())
    {
    }

    internal UiLanguageService(
        ILanguageCandidateLoader loader,
        IUiLanguageChangeSink sink,
        string settingsPath)
        : this(loader, sink, settingsPath, Se.SaveSettingsCandidate, Se.UpdateLibSeSettingsAfterExternalCommit)
    {
    }

    internal UiLanguageService(
        ILanguageCandidateLoader loader,
        IUiLanguageChangeSink sink,
        string settingsPath,
        Action<string, Se> persistSettings,
        Action updateLibSeSettings)
    {
        _loader = loader;
        _sink = sink;
        _settingsPath = settingsPath;
        _persistSettings = persistSettings;
        _updateLibSeSettings = updateLibSeSettings;
    }

    public async Task<LanguageApplyResult> TryApplyAsync(
        string languageName,
        CancellationToken cancellationToken = default)
    {
        await TransactionLock.WaitAsync(cancellationToken);
        try
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
                _persistSettings(_settingsPath, candidate);
            }
            catch (Exception ex)
            {
                return new(false, oldName, LanguageLoadFailure.ResourceUnavailable, ex.Message);
            }

            Se.Settings = candidate;
            Se.Language = loaded.Language;
            _updateLibSeSettings();

            var change = new LanguageChange(
                oldName,
                languageName,
                loaded.Language,
                UiLanguageDirection.IsRightToLeft(oldName) != UiLanguageDirection.IsRightToLeft(languageName));
            await _sink.ApplyAsync(change, cancellationToken);

            return new(true, languageName, LanguageLoadFailure.None, null);
        }
        finally
        {
            TransactionLock.Release();
        }
    }
}
