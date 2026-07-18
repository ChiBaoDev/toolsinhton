using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Features.Shared;
using Nikse.SubtitleEdit.Logic.Config;

namespace Nikse.SubtitleEdit.Features.Video.TextToSpeech.ElevenLabsSettings;

public partial class ElevenLabsSettingsViewModel : ObservableObject
{
    [ObservableProperty] private double _stability;
    [ObservableProperty] private double _similarity;
    [ObservableProperty] private double _speakerBoost;
    [ObservableProperty] private double _speed;
    [ObservableProperty] private double _styleExaggeration;

    public Window? Window { get; set; }

    public bool OkPressed { get; private set; }

    public ElevenLabsSettingsViewModel()
    {
        Stability = 0.5;
        Similarity = 0.5;
        SpeakerBoost = 0;
        Speed = 1.0;
        StyleExaggeration = 0;

        LoadSettings();
    }

    private void LoadSettings()
    {
        Stability = Se.Settings.Video.TextToSpeech.ElevenLabsStability;
        Similarity = Se.Settings.Video.TextToSpeech.ElevenLabsSimilarity;
        SpeakerBoost = Se.Settings.Video.TextToSpeech.ElevenLabsSpeakerBoost;
        Speed = Se.Settings.Video.TextToSpeech.ElevenLabsSpeed;
        StyleExaggeration = Se.Settings.Video.TextToSpeech.ElevenLabsStyleeExaggeration;
    }

    public void SaveSettings()
    {
        Se.Settings.Video.TextToSpeech.ElevenLabsStability = Stability;
        Se.Settings.Video.TextToSpeech.ElevenLabsSimilarity = Similarity;
        Se.Settings.Video.TextToSpeech.ElevenLabsSpeakerBoost = SpeakerBoost;
        Se.Settings.Video.TextToSpeech.ElevenLabsSpeed = Speed;
        Se.Settings.Video.TextToSpeech.ElevenLabsStyleeExaggeration = StyleExaggeration;
        Se.SaveSettings();
    }

    [RelayCommand]
    private async Task ShowStabilityHelp()
    {
        await ShowStabilityHelp(Window!);
    }

    [RelayCommand]
    private async Task ShowSimilarityHelp()
    {
        await ShowSimilarityHelp(Window!);
    }

    [RelayCommand]
    private async Task ShowSpeakerBoostHelp()
    {
        await ShowSpeakerBoostHelp(Window!);
    }

    [RelayCommand]
    private async Task ShowSpeedHelp()
    {
        await ShowSpeedHelp(Window!);
    }

    [RelayCommand]
    private async Task ShowStyleExaggerationHelp()
    {
        await ShowStyleExaggerationHelp(Window!);
    }

    [RelayCommand]
    private async Task ShowMoreOnWeb()
    {
        await Window!.Launcher.LaunchUriAsync(new Uri("https://elevenlabs.io/docs/capabilities/text-to-speech"));
    }

    [RelayCommand]
    private void Reset()
    {
        var settings = new SeVideoTextToSpeech();
        Stability = settings.ElevenLabsStability;
        Similarity = settings.ElevenLabsSimilarity;
        SpeakerBoost = settings.ElevenLabsSpeakerBoost;
        Speed = settings.ElevenLabsSpeed;
        StyleExaggeration = settings.ElevenLabsStyleeExaggeration;
    }

    [RelayCommand]
    private void Ok()
    {
        OkPressed = true;
        SaveSettings();
        Window?.Close();
    }

    [RelayCommand]
    private void Cancel()
    {
        Window?.Close();
    }

    public static async Task ShowStabilityHelp(Window window)
    {
        await MessageBox.Show(window, Se.Language.Video.TextToSpeech.Info, Se.Language.Video.TextToSpeech.TheStabilitySliderDeterminesHowStableTheVoice, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public static async Task ShowSimilarityHelp(Window window)
    {
        await MessageBox.Show(window, Se.Language.Video.TextToSpeech.Info, Se.Language.Video.TextToSpeech.TheSimilaritySliderDictatesHowCloselyTheAI, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public static async Task ShowSpeakerBoostHelp(Window window)
    {
        await MessageBox.Show(window, Se.Language.Video.TextToSpeech.Info, Se.Language.Video.TextToSpeech.BoostsTheSimilarityToTheOriginalSpeakerHowever, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    public static async Task ShowSpeedHelp(Window window)
    {
        await MessageBox.Show(window, Se.Language.Video.TextToSpeech.Info, Se.Language.Video.TextToSpeech.AdjustsTheSpeedOfTheGeneratedSpeechA, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    public static async Task ShowStyleExaggerationHelp(Window window)
    {
        await MessageBox.Show(window, Se.Language.Video.TextToSpeech.Info, Se.Language.Video.TextToSpeech.DeterminesTheStyleExaggerationOfTheVoiceThis, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    internal void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            e.Handled = true;
            Window?.Close();
        }
    }
}