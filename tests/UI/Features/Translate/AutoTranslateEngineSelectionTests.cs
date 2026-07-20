using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Microsoft.Extensions.DependencyInjection;
using Nikse.SubtitleEdit;
using Nikse.SubtitleEdit.Core.AutoTranslate;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Features.Translate;
using Nikse.SubtitleEdit.Logic.Config;

namespace UITests.Features.Translate;

public class AutoTranslateEngineSelectionTests
{
    private static AutoTranslateViewModel ResolveViewModel()
    {
        var services = new ServiceCollection();
        services.AddSubtitleEditServices();
        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<AutoTranslateViewModel>();
    }

    [AvaloniaFact]
    public void AutoTranslatorChanged_UsesComboBoxSelectedEngine()
    {
        var vm = ResolveViewModel();
        var openAiCompatible = Assert.Single(vm.AutoTranslators.OfType<OpenAiCompatibleTranslate>());
        var engineCombo = new ComboBox
        {
            ItemsSource = vm.AutoTranslators,
            SelectedItem = openAiCompatible,
        };

        Assert.IsNotType<OpenAiCompatibleTranslate>(vm.SelectedAutoTranslator);

        vm.AutoTranslatorChanged(engineCombo);

        Assert.Same(openAiCompatible, vm.SelectedAutoTranslator);
        Assert.Equal(OpenAiCompatibleTranslate.StaticName, vm.AutoTranslatorLinkText);
    }

    [AvaloniaFact]
    public void SaveSettings_OpenAiCompatibleSelection_RoutesApiSettingsWithoutChangingChatGpt()
    {
        var oldSettings = Se.Settings;
        var oldChatGptUrl = Configuration.Settings.Tools.ChatGptUrl;
        var oldChatGptApiKey = Configuration.Settings.Tools.ChatGptApiKey;
        var oldChatGptModel = Configuration.Settings.Tools.ChatGptModel;
        var oldOpenAiCompatibleUrl = Configuration.Settings.Tools.OpenAiCompatibleTranslateUrl;
        var oldOpenAiCompatibleApiKey = Configuration.Settings.Tools.OpenAiCompatibleTranslateApiKey;
        var oldOpenAiCompatibleModel = Configuration.Settings.Tools.OpenAiCompatibleTranslateModel;
        try
        {
            Se.Settings = new Se();
            Se.Settings.AutoTranslate.ChatGptUrl = "https://chatgpt.example/v1/chat/completions";
            Configuration.Settings.Tools.ChatGptApiKey = "chatgpt-api-key";
            Configuration.Settings.Tools.ChatGptModel = "chatgpt-model";

            var vm = ResolveViewModel();
            var openAiCompatible = Assert.Single(vm.AutoTranslators.OfType<OpenAiCompatibleTranslate>());
            var engineCombo = new ComboBox { SelectedItem = openAiCompatible };
            const string url = "https://translator.example/v1/chat/completions";
            const string apiKey = "test-api-key";
            const string model = "test-model";

            vm.AutoTranslatorChanged(engineCombo);
            vm.ApiUrlText = url;
            vm.ApiKeyText = apiKey;
            vm.ModelText = model;

            vm.SaveSettings();

            Assert.Equal(OpenAiCompatibleTranslate.StaticName, Se.Settings.AutoTranslate.AutoTranslateLastName);
            Assert.Equal(url, Se.Settings.AutoTranslate.OpenAiCompatibleUrl);
            Assert.Equal(apiKey, Se.Settings.AutoTranslate.OpenAiCompatibleApiKey);
            Assert.Equal(model, Se.Settings.AutoTranslate.OpenAiCompatibleModel);
            Assert.Equal("https://chatgpt.example/v1/chat/completions", Se.Settings.AutoTranslate.ChatGptUrl);
            Assert.Equal("chatgpt-api-key", Se.Settings.AutoTranslate.ChatGptApiKey);
            Assert.Equal("chatgpt-model", Se.Settings.AutoTranslate.ChatGptModel);
        }
        finally
        {
            Se.Settings = oldSettings;
            Configuration.Settings.Tools.ChatGptUrl = oldChatGptUrl;
            Configuration.Settings.Tools.ChatGptApiKey = oldChatGptApiKey;
            Configuration.Settings.Tools.ChatGptModel = oldChatGptModel;
            Configuration.Settings.Tools.OpenAiCompatibleTranslateUrl = oldOpenAiCompatibleUrl;
            Configuration.Settings.Tools.OpenAiCompatibleTranslateApiKey = oldOpenAiCompatibleApiKey;
            Configuration.Settings.Tools.OpenAiCompatibleTranslateModel = oldOpenAiCompatibleModel;
            Se.UpdateLibSeSettingsAfterExternalCommit();
        }
    }
}
