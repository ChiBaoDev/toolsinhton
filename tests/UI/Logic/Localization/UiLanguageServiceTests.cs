using Nikse.SubtitleEdit.Logic.Config;
using Nikse.SubtitleEdit.Logic.Config.Language;
using Microsoft.Extensions.DependencyInjection;
using Nikse.SubtitleEdit;

namespace UITests.Logic.Localization;

public sealed class UiLanguageServiceTests
{
    [Fact]
    public async Task InvalidSelectionChangesNothing() =>
        await Run(async (path, sink) =>
        {
            var oldSettings = Se.Settings;
            var oldLanguage = Se.Language;
            var before = File.ReadAllText(path);

            var result = await new UiLanguageService(new Loader(false), sink, path).TryApplyAsync("Bad");

            Assert.False(result.Success);
            Assert.Same(oldSettings, Se.Settings);
            Assert.Same(oldLanguage, Se.Language);
            Assert.Equal(before, File.ReadAllText(path));
            Assert.Equal(0, sink.Count);
        });

    [Fact]
    public async Task ValidSelectionPersistsCommitsAndCallsSinkOnce() =>
        await Run(async (path, sink) =>
        {
            var language = new SeLanguage();

            var result = await new UiLanguageService(new Loader(true, language), sink, path).TryApplyAsync("French");

            Assert.True(result.Success);
            Assert.Equal("French", Se.Settings.General.Language);
            Assert.Same(language, Se.Language);
            Assert.Contains("French", File.ReadAllText(path));
            Assert.Equal(1, sink.Count);
            Assert.Equal("English", sink.Change!.OldName);
        });

    [Fact]
    public void DependencyInjectionResolvesLanguageServiceFactory()
    {
        var services = new ServiceCollection();
        services.AddSubtitleEditServices();
        using var provider = services.BuildServiceProvider();

        var factory = provider.GetRequiredService<UiLanguageServiceFactory>();
        var service = factory.Create(new Sink());

        Assert.IsType<UiLanguageService>(service);
    }

    [Fact]
    public async Task SinkFailureDoesNotReportThatTheOldLanguageIsStillActive() =>
        await Run(async (path, _) =>
        {
            var language = new SeLanguage();
            var sink = new ThrowingSink();
            var service = new UiLanguageService(new Loader(true, language), sink, path);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.TryApplyAsync("French", TestContext.Current.CancellationToken));

            Assert.Equal("French", Se.Settings.General.Language);
            Assert.Same(language, Se.Language);
            Assert.Contains("French", File.ReadAllText(path));
            Assert.Equal(1, sink.Count);
        });

    private static async Task Run(Func<string, Sink, Task> action)
    {
        var oldSettings = Se.Settings;
        var oldLanguage = Se.Language;
        var oldPath = Se.SettingsFilePathOverride;
        var directory = Path.Combine(Path.GetTempPath(), "SE12", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "Settings.json");
        try
        {
            Se.Settings = new Se();
            Se.Settings.General.Language = "English";
            Se.Language = new SeLanguage();
            Se.SettingsFilePathOverride = path;
            Se.SaveSettings(path);
            await action(path, new Sink());
        }
        finally
        {
            Se.Settings = oldSettings;
            Se.Language = oldLanguage;
            Se.SettingsFilePathOverride = oldPath;
            Directory.Delete(directory, true);
        }
    }

    private sealed class Loader(bool ok, SeLanguage? language = null) : ILanguageCandidateLoader
    {
        public LanguageLoadResult TryLoad(string name) => ok
            ? new(true, name, language!, LanguageSource.WritableFile, LanguageLoadFailure.None, null)
            : new(false, name, null, LanguageSource.WritableFile, LanguageLoadFailure.InvalidShape, "invalid");
    }

    private sealed class Sink : IUiLanguageChangeSink
    {
        public int Count;
        public LanguageChange? Change;

        public Task ApplyAsync(LanguageChange change, CancellationToken cancellationToken = default)
        {
            Count++;
            Change = change;
            return Task.CompletedTask;
        }
    }

    private sealed class ThrowingSink : IUiLanguageChangeSink
    {
        public int Count;

        public Task ApplyAsync(LanguageChange change, CancellationToken cancellationToken = default)
        {
            Count++;
            throw new InvalidOperationException("UI rebuild failed");
        }
    }
}
