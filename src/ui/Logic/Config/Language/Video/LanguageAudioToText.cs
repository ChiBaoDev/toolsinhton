namespace Nikse.SubtitleEdit.Logic.Config.Language;

public class LanguageAudioToText
{
    public string Title { get; set; }
    public string Transcribe { get; set; }
    public string TranslateToEnglish { get; set; }
    public string Transcribing { get; set; }
    public string TranscribingXOfY { get; set; }
    public string InputLanguage { get; set; }
    public string AdvancedWhisperSettings { get; set; }
    public string DownloadingSpeechToTextEngine { get; set; }
    public string UnpackingSpeechToTextEngine { get; set; }
    public string EnableVad { get; set; }
    public string WhisperXxlStandard { get; set; }
    public string WhisperXxlStandardAsia { get; set; }
    public string WhisperXxlSentence { get; set; }
    public string WhisperXxlSingleWords { get; set; }
    public string WhisperXxlHighlightWord { get; set; }
    public string SelectModel { get; set; }
    public string AddCustomModelDotDotDot { get; set; }
    public string CustomModelHelp { get; set; }
    public string ViewToolsLogFile { get; set; }
    public string ReDownloadX { get; set; }
    public string DownloadX { get; set; }
    public string UpdateXTitle { get; set; }
    public string UpdateXMessage { get; set; }
    public string? DownloadingSpeechToTextModel { get; set; }
    public string WhisperPostProcessingTitle { get; set; }
    public string AdjustTimings { get; set; }
    public string MergeShortLines { get; set; }
    public string BreakSplitLongLines { get; set; }
    public string FixShortDuration { get; set; }
    public string FixCasing { get; set; }
    public string AddPeriods { get; set; }
    public string ChangeUnderlineToColor { get; set; }
    public string CueBuilding { get; set; }
    public string CueRebuild { get; set; }
    public string CueMaxChars { get; set; }
    public string CueMaxSeconds { get; set; }
    public string CueMaxCps { get; set; }
    public string VocabularyPrompt { get; set; }
    public string BeamSize { get; set; }
    public string BeamSizeNote { get; set; }

    public string EngineSettings { get; set; }
    public string EngineSettingsSubtitle { get; set; }
    public string BackendAndUpdateStatus { get; set; }
    public string ModelFileNotFound { get; set; }
    public string WhisperCppModelMustBeGgmlBin { get; set; }
    public string ModelFolderNotFoundX { get; set; }
    public string FasterWhisperModelFolderMustContainModelBin { get; set; }
    public string InputAudioFileNotFound { get; set; }
    public string AudioFileNotFound { get; set; }
    public string DashScopeTranscriptionTimedOut { get; set; }
    public string SttRequestTimedOut { get; set; }
    public string OpenRouterTranscriptionTimedOut { get; set; }
    public string DashScopeUploadPolicyParseFailedX { get; set; }
    public string DashScopeTaskIdMissingX { get; set; }
    public string DashScopeTranscriptionUrlMissingX { get; set; }
    public string DashScopeTaskFailedXX { get; set; }
    public string DashScopeUploadPolicyRequestFailedXX { get; set; }
    public string DashScopeOssUploadFailedXX { get; set; }
    public string DashScopeAsyncSubmitFailedXX { get; set; }
    public string DashScopeTaskPollFailedXX { get; set; }
    public string FfmpegChunkExtractionFailedXXXXX { get; set; }

    public LanguageAudioToText()
    {
        Title = "Speech to text";
        Transcribe = "Transcribe";
        TranslateToEnglish = "Translate to English";
        Transcribing = "Transcribing...";
        TranscribingXOfY = "Transcribing {0} of {1}...";
        InputLanguage = "Input language";
        AdvancedWhisperSettings = "Advanced speech-to-text parameters";
        DownloadingSpeechToTextEngine = "Downloading speech-to-text engine";
        UnpackingSpeechToTextEngine = "Unpacking speech-to-text engine";
        EnableVad = "Enable VAD";
        WhisperXxlStandard = "Standard";
        WhisperXxlStandardAsia = "Standard Asia";
        WhisperXxlSentence = "Sentence-level";
        WhisperXxlSingleWords = "Single words";
        WhisperXxlHighlightWord = "Highlight word";
        SelectModel = "Select model";
        AddCustomModelDotDotDot = "Add custom model...";
        CustomModelHelp = "You can use your own model: pick the model file (whisper.cpp ggml '.bin') or model folder (faster-whisper folder with a 'model.bin' inside). It is copied to the models folder and added to the list above.";
        ViewToolsLogFile = "View tools log file";
        ReDownloadX = "Re-download {0}";
        DownloadX = "Download {0}";
        UpdateXTitle = "Update {0}?";
        UpdateXMessage = "A newer version of {0} is available.{1}{1}Download and install the update now?";
        DownloadingSpeechToTextModel = "Downloading speech-to-text model";
        WhisperPostProcessingTitle = "Whisper post-processing";
        AdjustTimings = "Adjust timings";
        MergeShortLines = "Merge short lines";
        BreakSplitLongLines = "Break/split long lines";
        FixShortDuration = "Fix short duration";
        FixCasing = "Fix casing";
        AddPeriods = "Add periods";
        ChangeUnderlineToColor = "Change underline to color";
        CueBuilding = "Cue building (MLX Whisper)";
        CueRebuild = "Rebuild cues from word timestamps";
        CueMaxChars = "Max characters per cue";
        CueMaxSeconds = "Max seconds per cue";
        CueMaxCps = "Max characters per second";
        VocabularyPrompt = "Vocabulary prompt (names, places, terms)";
        BeamSize = "Beam size";
        BeamSizeNote = "0 decodes fastest. 5 gives the best accuracy (like Faster Whisper) at roughly 20-30% more time. Values above 5 rarely help.";

        EngineSettings = "Speech-to-text engine settings";
        EngineSettingsSubtitle = "Speech-to-text engine";
        BackendAndUpdateStatus = "Backend and update status";
        ModelFileNotFound = "Model file not found.";
        WhisperCppModelMustBeGgmlBin = "A whisper.cpp model must be a ggml '.bin' file.";
        ModelFolderNotFoundX = "Model folder not found: {0}";
        FasterWhisperModelFolderMustContainModelBin = "A faster-whisper model folder must contain a 'model.bin' file.";
        InputAudioFileNotFound = "Input audio file not found";
        AudioFileNotFound = "Audio file not found";
        DashScopeTranscriptionTimedOut = "DashScope transcription timed out after {0} seconds.";
        SttRequestTimedOut = "STT request timed out after {0} seconds.";
        OpenRouterTranscriptionTimedOut = "OpenRouter transcription timed out after {0} seconds.";
        DashScopeUploadPolicyParseFailedX = "The DashScope upload-policy response could not be parsed. Response: {0}";
        DashScopeTaskIdMissingX = "The DashScope async submit response did not contain task_id. Response: {0}";
        DashScopeTranscriptionUrlMissingX = "The completed DashScope task did not contain transcription_url. Response: {0}";
        DashScopeTaskFailedXX = "The DashScope transcription task ended with status {0}. Response: {1}";
        DashScopeUploadPolicyRequestFailedXX = "The DashScope upload-policy request failed ({0}). Response: {1}";
        DashScopeOssUploadFailedXX = "The DashScope OSS upload failed ({0}). Response: {1}";
        DashScopeAsyncSubmitFailedXX = "The DashScope async submit failed ({0}). Response: {1}";
        DashScopeTaskPollFailedXX = "The DashScope task poll failed ({0}). Response: {1}";
        FfmpegChunkExtractionFailedXXXXX = "ffmpeg failed to extract chunk {0}/{1} ({2} seconds → {3} seconds) from {4}";
    }
}