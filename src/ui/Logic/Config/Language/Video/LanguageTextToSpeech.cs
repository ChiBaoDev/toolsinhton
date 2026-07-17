using System;

namespace Nikse.SubtitleEdit.Logic.Config.Language;

public class LanguageTextToSpeech
{
    public string Title { get; set; }
    public string TextToSpeechEngine { get; set; }
    public string ReviewAudioSegments { get; set; }
    public string ReviewAudioSegmentsHistory { get; set; }
    public string Stability { get; set; }
    public string Similarity { get; set; }
    public string SpeakerBoost { get; set; }
    public string StyleExaggeration { get; set; }
    public string RegenerateAudioSelectedLine { get; set; }
    public string GenerateSpeechFromText { get; set; }
    public string TestVoice { get; set; }
    public string AddAudioToVideoFile { get; set; }
    public string EngineAndVoice { get; set; }
    public string Output { get; set; }
    public string XLinesFromY { get; set; }
    public string XLines { get; set; }
    public string XVoices { get; set; }
    public string ReviewAudioSegmentsHint { get; set; }
    public string AddAudioToVideoFileHint { get; set; }
    public string XElapsedYLeft { get; set; }
    public string XElapsed { get; set; }
    public string VoiceSettings { get; set; }
    public string VoiceSampleText { get; set; }
    public string RefreshVoices { get; set; }
    public string VideoEncodingSettings { get; set; }
    public string ElevenLabsSettings { get; set; }
    public string ElevenLabsSettingsResetHint { get; set; }
    public string RegenerateAudio { get; set; }
    public string AutoContinuePlaying { get; set; }
    public string AddingAudioToVideoFileDotDotDot { get; set; }
    public string PreparingMergeDotDotDot { get; set; }
    public string ImportVoiceDotDotDot { get; set; }
    public string VoiceImportSuccessTitle { get; set; }
    public string VoiceXImported { get; set; }
    public string DropAudioFileHereToImportVoice { get; set; }
    public string DropAudioFileHereHint { get; set; }
    public string VoiceCloneTranscriptTitle { get; set; }
    public string UseSpeechToTextDotDotDot { get; set; }
    public string AdvancedTtsSettings { get; set; }
    public string ProAudioPostProcessing { get; set; }
    public string ProAudioPostProcessingDescription { get; set; }
    public string AudioDucking { get; set; }
    public string AudioDuckingDescription { get; set; }
    public string OriginalVolumePercent { get; set; }
    public string VadSilenceCompression { get; set; }
    public string VadSilenceCompressionDescription { get; set; }
    public string MaxSilenceMs { get; set; }
    public string HighQualityTimeStretch { get; set; }
    public string HighQualityTimeStretchDescription { get; set; }
    public string SilencePaddingMs { get; set; }
    public string SilencePaddingMsDescription { get; set; }
    public string OutputSampleRate { get; set; }
    public string OutputSampleRateDescription { get; set; }
    public string EdgeTtsRate { get; set; }
    public string EdgeTtsRateDescription { get; set; }
    public string EdgeTtsPitch { get; set; }
    public string EdgeTtsPitchDescription { get; set; }
    public string EdgeTtsVolume { get; set; }
    public string EdgeTtsVolumeDescription { get; set; }
    public string DownloadPiperPrompt { get; set; }
    public string DownloadEngineTitle { get; set; }
    public string CosyVoice3CrispAsrSettings { get; set; }
    public string F5TtsCrispAsrSettings { get; set; }
    public string IndexTtsCrispAsrSettings { get; set; }
    public string Qwen3TtsCrispAsrSettings { get; set; }
    public string VibeVoiceCrispAsrSettings { get; set; }
    public string VoxCpm2CrispAsrSettings { get; set; }
    public string CrispAsrExecutableNotFound { get; set; }
    public string KokoroServerExecutableNotFound { get; set; }
    public string KokoroModelOrVoicesFileMissing { get; set; }
    public string OmniVoiceExecutableNotFound { get; set; }
    public string OmniVoiceTranscriptRequiredX { get; set; }
    public string OmniVoiceStartFailed { get; set; }
    public string ServerStartFailedX { get; set; }
    public string ServerHealthTimedOutXXX { get; set; }
    public string AllTalkServerNotReachable { get; set; }
    public string AllTalkRequestTimedOut { get; set; }
    public string AzureRegionRequiredForVoiceRefresh { get; set; }
    public string Qwen3ServerExecutableNotFound { get; set; }
    public string ChatterboxRequiresCrispAsrUpdate { get; set; }
    public string ChatterboxModelCacheStaleXX { get; set; }
    public string ChatterboxTurboTokenizerMismatchX { get; set; }
    public string ChatterboxTurboStartupCrashX { get; set; }
    public string ProcessTimedOutXXX { get; set; }
    public string ChatterboxExitedDuringStartupXX { get; set; }
    public string ChatterboxHealthTimedOutX { get; set; }

    public string OmniVoiceTtsSettings { get; set; }
    public string ReDownloadOmniVoiceTts { get; set; }
    public string SelectTheBuildToDownload { get; set; }
    public string DownloadTheLatestOmniVoiceTtsPrompt { get; set; }
    public string VulkanRuntimeMayBeRequired { get; set; }
    public string VulkanRuntimeNotDetectedMessage { get; set; }
    public string Qwen3TtsSettings { get; set; }
    public string KokoroTtsSettings { get; set; }
    public string ChatterboxTtsSettings { get; set; }
    public string PiperSettings { get; set; }
    public string VoiceInstruction { get; set; }
    public string VoiceInstructionHint { get; set; }
    public string VoiceGender { get; set; }
    public string VoiceAge { get; set; }
    public string VoicePitch { get; set; }
    public string VoiceAccent { get; set; }
    public string VoiceInstructionClonedVoiceNote { get; set; }

    // Cast dialog
    public string ActorVoicesTitle { get; set; }
    public string ActorVoicesSubtitle { get; set; }
    public string ActorVoicesAssignedXOfY { get; set; }
    public string ActorOrVoice { get; set; }
    public string ApplyDefaultToAll { get; set; }
    public string ClearAllAssignmentsConfirm { get; set; }
    public string SetupCast { get; set; }
    public string SetupCastHint { get; set; }
    public string ActorVoicesRowSettingsTitle { get; set; }
    public string VoiceSettingsForX { get; set; }
    public string VoiceInstructionFreeTextHint { get; set; }
    public string VoiceDesign { get; set; }
    public string NoActorsFoundMessage { get; set; }
    public string NoWebVttVoicesFoundMessage { get; set; }
    public string MergeContinuationLinesPromptTitle { get; set; }
    public string MergeContinuationLinesPromptMessage { get; set; }

    // Applying the TTS window's changes (review text edits, merged lines) back to the subtitle
    public string SubtitleUpdatedFromReviewSingular { get; set; }
    public string SubtitleUpdatedFromReviewPlural { get; set; }
    public string SubtitleMergedLinesAppliedSingular { get; set; }
    public string SubtitleMergedLinesAppliedPlural { get; set; }

    public LanguageTextToSpeech()
    {
        Title = "Text to speech";
        DownloadEngineTitle = "TTS - Download engine";
        CosyVoice3CrispAsrSettings = "CosyVoice3 (CrispASR) settings";
        F5TtsCrispAsrSettings = "F5-TTS (CrispASR) settings";
        IndexTtsCrispAsrSettings = "IndexTTS (CrispASR) settings";
        Qwen3TtsCrispAsrSettings = "Qwen3 TTS (CrispASR) settings";
        VibeVoiceCrispAsrSettings = "VibeVoice (CrispASR) settings";
        VoxCpm2CrispAsrSettings = "VoxCPM2 (CrispASR) settings";
        CrispAsrExecutableNotFound = "CrispASR executable not found. Install CrispASR via Video → Audio to text first.";
        KokoroServerExecutableNotFound = "Kokoro TTS server executable not found.";
        KokoroModelOrVoicesFileMissing = "Kokoro TTS model or voices file missing.";
        OmniVoiceExecutableNotFound = "omnivoice-tts executable not found.";
        OmniVoiceTranscriptRequiredX = "OmniVoice TTS voice cloning requires a transcript file at {0}. Re-import the voice to provide its transcript.";
        OmniVoiceStartFailed = "Failed to start omnivoice-tts.";
        ServerStartFailedX = "Failed to start {0}.";
        ServerHealthTimedOutXXX = "The {0} server did not report healthy within {1} seconds. Last output: {2}";
        AllTalkServerNotReachable = "The AllTalk TTS server is not reachable. Check that the server is running.";
        AllTalkRequestTimedOut = "The request to the AllTalk TTS server timed out. Check that the server is running.";
        AzureRegionRequiredForVoiceRefresh = "Set the Azure region in the TTS engine settings before refreshing voices.";
        Qwen3ServerExecutableNotFound = "Qwen3 TTS server executable not found.";
        ChatterboxRequiresCrispAsrUpdate = "Chatterbox requires CrispASR v0.6.0 or newer. Re-download CrispASR via Video → Audio to text → Engine settings → Re-download, then try again.";
        ChatterboxModelCacheStaleXX = "Chatterbox failed to load its model — the GGUFs in {0} are likely stale or partially downloaded. Delete them and try again so they re-download. Original output: {1}";
        ChatterboxTurboTokenizerMismatchX = "Chatterbox TTS \"Turbo\" does not load with CrispASR 0.8.0. The turbo model is fine — 0.8.0's tokenizer/vocab check was overly strict and rejected its benign embedding superset (50257-token tokenizer, text vocab size 50276). This is fixed upstream (CrispStrobe/CrispASR#181): a newer CrispASR loads Turbo normally, with no re-download. Until then, switch to the \"Base\" Chatterbox model, which works.\n\n{0}";
        ChatterboxTurboStartupCrashX = "Chatterbox TTS \"Turbo\" model crashed CrispASR during startup. This is a known upstream issue in the chatterbox-turbo backend (especially on macOS/CPU). Try the \"Base\" model instead, or file an issue at https://github.com/CrispStrobe/CrispASR/issues with the log below.\n\n{0}";
        ProcessTimedOutXXX = "\"{0} {1}\" did not finish within {2} seconds and was killed.";
        ChatterboxExitedDuringStartupXX = "crispasr (chatterbox) exited during startup (code {0}). Output: {1}";
        ChatterboxHealthTimedOutX = "crispasr (chatterbox) did not report healthy within 15 minutes. Last output: {0}";
        TextToSpeechEngine = "Text to speech engine";
        ReviewAudioSegments = "TTS - Review audio segments";
        ReviewAudioSegmentsHistory = "TTS - Review audio history";
        Stability = "Stability";
        Similarity = "Similarity";
        SpeakerBoost = "Speaker boost";
        StyleExaggeration = "Style exaggeration";
        RegenerateAudioSelectedLine = "Regenerate audio for selected line";
        GenerateSpeechFromText = "Generate speech from text";
        TestVoice = "Test voice";
        AddAudioToVideoFile = "Add audio to video file";
        EngineAndVoice = "Engine & voice";
        Output = "Output";
        XLinesFromY = "{0} lines from {1}";
        XLines = "{0} lines";
        XVoices = "{0} voices";
        ReviewAudioSegmentsHint = "Check each line before the final mix";
        AddAudioToVideoFileHint = "Mux the result into a new video file";
        XElapsedYLeft = "{0} elapsed \u00b7 ~{1} left";
        XElapsed = "{0} elapsed";
        VoiceSettings = "TTS - Voice settings";
        VoiceSampleText = "Voice sample text";
        RefreshVoices = "Refresh voices";
        VideoEncodingSettings = "TTS - Video encoding settings";
        ElevenLabsSettings = "TTS - ElevenLabs settings";
        ElevenLabsSettingsResetHint = "Reset ElevenLabs settings to default values";
        RegenerateAudio = "Regenerate audio";
        AutoContinuePlaying = "Auto-continue playing";
        AddingAudioToVideoFileDotDotDot = "Adding audio to video file...";
        PreparingMergeDotDotDot = "Preparing merge...";
        ImportVoiceDotDotDot = "Import voice...";
        VoiceImportSuccessTitle = "Voice imported";
        VoiceXImported = "Voice '{0}' imported successfully";
        DropAudioFileHereToImportVoice = "Drop audio file here to import voice";
        DropAudioFileHereHint = ".wav or .mp3";
        VoiceCloneTranscriptTitle = "Enter transcript of the audio (required for voice cloning)";
        UseSpeechToTextDotDotDot = "Use speech-to-text...";
        AdvancedTtsSettings = "Advanced TTS settings";
        ProAudioPostProcessing = "Pro audio post-processing";
        ProAudioPostProcessingDescription = "Applies EQ warmth, noise gate, compression, loudness normalization (-16 LUFS), and fade in/out to each segment.";
        AudioDucking = "Audio ducking";
        AudioDuckingDescription = "Reduces the original video audio volume and mixes it with the TTS audio, so the original soundtrack is still faintly audible.";
        OriginalVolumePercent = "Original volume %";
        VadSilenceCompression = "VAD silence compression";
        VadSilenceCompressionDescription = "Shortens pauses between words before changing tempo. Uses Voice Activity Detection to compress only silence gaps while keeping speech untouched. This is the preferred first step — it reduces duration without any quality loss.";
        MaxSilenceMs = "Max silence (ms)";
        HighQualityTimeStretch = "High-quality time-stretch (WSOLA/rubberband)";
        HighQualityTimeStretchDescription = "Uses the rubberband algorithm (WSOLA) instead of the default atempo filter for pitch-preserving speed changes. Produces more natural-sounding speech, especially at higher speed factors. Requires librubberband in your FFmpeg build — falls back to atempo automatically if unavailable.";
        SilencePaddingMs = "Silence padding (ms)";
        SilencePaddingMsDescription = "Adds a short silence at the end of each segment. Useful for breathing room between sentences.";
        OutputSampleRate = "Output sample rate (0 = default)";
        OutputSampleRateDescription = "Resamples all segments to the specified sample rate (e.g. 44100, 48000). Set to 0 to keep the original rate.";
        EdgeTtsRate = "Edge-TTS rate";
        EdgeTtsRateDescription = "Speech rate for Edge-TTS, e.g. \"+50%\", \"-30%\", or \"+0%\" for default.";
        EdgeTtsPitch = "Edge-TTS pitch";
        EdgeTtsPitchDescription = "Pitch adjustment for Edge-TTS, e.g. \"+10Hz\", \"-5Hz\", or \"+0Hz\" for default.";
        EdgeTtsVolume = "Edge-TTS volume";
        EdgeTtsVolumeDescription = "Volume adjustment for Edge-TTS, e.g. \"+20%\", \"-10%\", or \"+0%\" for default.";
        DownloadPiperPrompt = $"\"Text to speech\" requires Piper.{Environment.NewLine}{Environment.NewLine}Download and use Piper?";

        OmniVoiceTtsSettings = "OmniVoice TTS settings";
        ReDownloadOmniVoiceTts = "Re-download OmniVoice TTS";
        SelectTheBuildToDownload = $"{Environment.NewLine}Select the build to download:";
        DownloadTheLatestOmniVoiceTtsPrompt = $"{Environment.NewLine}Download the latest OmniVoice TTS now?";
        VulkanRuntimeMayBeRequired = "Vulkan runtime may be required";
        VulkanRuntimeNotDetectedMessage = $"The Vulkan build needs the Vulkan runtime (vulkan-1.dll). It usually ships with current GPU drivers but was not detected.{Environment.NewLine}{Environment.NewLine}Install it from:{Environment.NewLine}{{0}}{Environment.NewLine}{Environment.NewLine}Continue with Vulkan anyway?";
        Qwen3TtsSettings = "Qwen3 TTS settings";
        KokoroTtsSettings = "Kokoro TTS settings";
        ChatterboxTtsSettings = "Chatterbox TTS settings";
        PiperSettings = "Piper settings";
        VoiceInstruction = "Voice design";
        VoiceInstructionHint = "Optional - e.g. \"Speak in a calm and friendly tone\"";
        VoiceGender = "Gender";
        VoiceAge = "Age";
        VoicePitch = "Pitch";
        VoiceAccent = "Accent";
        VoiceInstructionClonedVoiceNote = "Voice design only affects the \"Default\" voice - a cloned voice keeps its own characteristics.";

        ActorVoicesTitle = "TTS - Cast";
        ActorVoicesSubtitle = "Assign a TTS voice (and optional voice-design instruction) to each actor or voice.";
        ActorVoicesAssignedXOfY = "{0} of {1} assigned";
        ActorOrVoice = "Actor / Voice";
        ApplyDefaultToAll = "Apply default to all";
        ClearAllAssignmentsConfirm = "Clear all voice assignments?";
        SetupCast = "Cast...";
        SetupCastHint = "Assign a TTS voice to each actor (ASSA) or voice (WebVTT).";
        ActorVoicesRowSettingsTitle = "TTS - Voice settings";
        VoiceSettingsForX = "Voice settings for \"{0}\"";
        VoiceInstructionFreeTextHint = "Free text used by the engine to shape the voice's tone.";
        VoiceDesign = "Voice design";
        NoActorsFoundMessage = "No actors found. Set the Actor field on subtitle lines first.";
        NoWebVttVoicesFoundMessage = "No <v Name> voices found in the WebVTT file.";
        MergeContinuationLinesPromptTitle = "Merge continuation lines?";
        MergeContinuationLinesPromptMessage = "Some lines appear to be a single sentence split across multiple subtitles." + Environment.NewLine + Environment.NewLine +
                                              "Merging them before generation lets the TTS engine speak each thought as one breath group, which usually sounds more natural." + Environment.NewLine + Environment.NewLine +
                                              "Review and apply merges now?";

        SubtitleUpdatedFromReviewSingular = "Updated one line from the speech review";
        SubtitleUpdatedFromReviewPlural = "Updated {0} lines from the speech review";
        SubtitleMergedLinesAppliedSingular = "Applied one line merge from text to speech";
        SubtitleMergedLinesAppliedPlural = "Applied {0} line merges from text to speech";
    }
}