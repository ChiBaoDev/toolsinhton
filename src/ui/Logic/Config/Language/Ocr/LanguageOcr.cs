using System;

namespace Nikse.SubtitleEdit.Logic.Config.Language.Translate;

public class LanguageOcr
{
    public string LinesToDraw { get; set; }
    public string CurrentImage { get; set; }
    public string AutoDrawAgain { get; set; }
    public string StartOcr { get; set; }
    public string PauseOcr { get; set; }
    public string InspectLine { get; set; }
    public string OcrEngine { get; set; }
    public string TesseractEngineMode { get; set; }
    public string TesseractEngineModeLegacy { get; set; }
    public string TesseractEngineModeNeural { get; set; }
    public string TesseractEngineModeBoth { get; set; }
    public string TesseractEngineModeDefault { get; set; }
    public string Database { get; set; }
    public string MaxWrongPixels { get; set; }
    public string MaxErrorPct { get; set; }
    public string NumberOfPixelsIsSpace { get; set; }
    public string InspectImageMatches { get; set; }
    public string ResolutionXYAndTopmarginZ { get; set; }
    public string RunningOcrDotDotDotXY { get; set; }
    public string RunningOcrDotDotDot { get; set; }
    public string AutoSubmitFirstCharacter { get; set; }
    public string EditNOcrDatabase { get; set; }
    public string ZoomFactorX { get; set; }
    public string ExpandInfoX { get; set; }
    public string EditNOcrDatabaseXWithYItems { get; set; }
    public string NewNOcrDatabase { get; set; }
    public string RenameNOcrDatabase { get; set; }
    public string NOcrDatabase { get; set; }
    public string DrawMode { get; set; }
    public string AddNewCharcter { get; set; }
    public string LineIndexX { get; set; }
    public string InspectNOcrAdditions { get; set; }
    public string OcrSelectedLines { get; set; }
    public string FillSelectedLinesWithClipboard { get; set; }
    public string ShowImage { get; set; }
    public string FixOcrErrors { get; set; }
    public string PromptForUknownWords { get; set; }
    public string TryToGuessUnknownWords { get; set; }
    public string AutoBreakIfMoreThanXLines { get; set; }
    public string UnknownWords { get; set; }
    public string AllFixes { get; set; }
    public string GuessesUsed { get; set; }
    public string Ocr { get; set; }
    public string OcrX { get; set; }
    public string AddBetterMatch { get; set; }
    public string NOcrInspectImageMatches { get; set; }
    public string AddToOcrPair { get; set; }
    public string AddNameToOcrReplaceList { get; set; }
    public string WordToAdd { get; set; }
    public string NameToAdd { get; set; }
    public string ChangeWordFromTo { get; set; }
    public string ClearBackground { get; set; }
    public string ClearForeground { get; set; }
    public string NOcrDrawHelp { get; set; }
    public string EditWholeText { get; set; }
    public string EditWordOnly { get; set; }
    public string ImagePreProcessing { get; set; }
    public string PreProcessingTitle { get; set; }
    public string CropTransparent { get; set; }
    public string InverseColors { get; set; }
    public string RemoveBorders { get; set; }
    public string Binarize { get; set; }
    public string BorderSize { get; set; }
    public string CaptureTopAlign { get; set; }
    public string OcrImage { get; set; }
    public string OneColor { get; set; }
    public string DarknessThreshold { get; set; }
    public string EditExportDotDotDot { get; set; }
    public string EditBinaryOcrDatabase { get; set; }
    public string BinaryImageCompareDatabase { get; set; }
    public string RemoveXFromUnknownWordsList { get; set; }
    public string DownloadingPaddleOcrEngineDotDotDot { get; set; }
    public string DownloadingTesseractModel { get; set; }
    public string SelectTesseractDictionary { get; set; }
    public string Algorithm { get; set; }
    public string AutoDrawAlgorithmTooltip { get; set; }
    public string DownloadingPaddleOcrModelsDotDotDot { get; set; }
    public string PaddleOcr { get; set; }
    public string BinaryImageCompareInspectImageMatches { get; set; }
    public string SaveBlankTextTitle { get; set; }
    public string SaveBlankTextPrompt { get; set; }
    public string YesAndNeverAskAgain { get; set; }
    public string ImportTextFromSubtitleDotDotDot { get; set; }
    public string ImportTextFromSubtitleOverwritePrompt { get; set; }
    public string ImportTextFromSubtitleNoMatchesFound { get; set; }
    public string ImportTextFromSubtitleXLinesImported { get; set; }
    public string ExportTextAsSubtitleDotDotDot { get; set; }
    public string ExportTextAsSubtitleNoText { get; set; }
    public string VobSubColors { get; set; }
    public string VobSubColorsTitle { get; set; }
    public string VobSubColorsHeader { get; set; }
    public string VobSubColorsDescription { get; set; }
    public string VobSubColorBackground { get; set; }
    public string VobSubColorPattern { get; set; }
    public string VobSubColorEmphasis1 { get; set; }
    public string VobSubColorEmphasis2 { get; set; }
    public string VobSubColorsInvert { get; set; }
    public string VobSubIsolateColors { get; set; }
    public string VobSubIsolateColorsHint { get; set; }
    public string LlamaCppOcrSettingsTitle { get; set; }
    public string LlamaCppOcrPromptHint { get; set; }
    public string LlamaCppOcrPromptEmpty { get; set; }
    public string LlamaCppOcrPromptMissingLanguagePlaceholder { get; set; }
    public string LlamaCppOcrTimeoutMinutes { get; set; }
    public string NOcrBinaryOcrFallbackDatabase { get; set; }
    public string NOcrBinaryOcrFallbackNone { get; set; }
    public string BinaryOcrNOcrFallbackDatabase { get; set; }
    public string PickFallbackDatabase { get; set; }
    public string FallbackOcrDatabase { get; set; }
    public string ShowAllOllamaModels { get; set; }
    public string OllamaModelLikelyWrong { get; set; }
    public string LlamaCppNotDownloaded { get; set; }
    public string LlamaCppReturnedNoText { get; set; }
    public string InspectBinaryOcrAdditions { get; set; }
    public string NewRenameBinaryImageCompareDatabase { get; set; }
    public string DownloadingCrispEmbed { get; set; }
    public string DownloadingGoogleLensOcr { get; set; }
    public string CloudVisionApiKeyInvalid { get; set; }
    public string CloudVisionRequestForbidden { get; set; }
    public string CloudVisionApiStatusErrorX { get; set; }
    public string CloudVisionApiCallErrorX { get; set; }
    public string DownloadingPaddleOcr { get; set; }
    public string DownloadingTesseract { get; set; }
    public string NewRenameNOcrDatabase { get; set; }

    public string DeleteBinaryOCRItem { get; set; }
    public string DoYouWantToDeleteTheCurrentBinary { get; set; }
    public string ValidationError { get; set; }
    public string ItemTextCannotBeEmpty { get; set; }
    public string BinaryOCR { get; set; }
    public string BinaryOCRCharacterUpdated { get; set; }
    public string DeleteBinaryImageCompareItem { get; set; }
    public string DoYouWantToDeleteTheCurrentBinary2 { get; set; }
    public string BinaryOCRCharacterSaved { get; set; }
    public string DeleteBinaryImageCompareDatabase { get; set; }
    public string DoYouWantToDeleteTheCurrentBinary3 { get; set; }
    public string DownloadPaddleOCR { get; set; }
    public string XPaddleOCRRequiresDownloadingPaddleOCRX { get; set; }
    public string DeleteNOCRItem { get; set; }
    public string DoYouWantToDeleteTheCurrentNOCR { get; set; }
    public string NOCR { get; set; }
    public string NOCRCharacterUpdated { get; set; }
    public string NOCRCharacterSaved { get; set; }
    public string DeleteNOCRDatabase { get; set; }
    public string DoYouWantToDeleteTheCurrentNOCR2 { get; set; }
    public string DownloadCrispEmbed { get; set; }
    public string XCrispEmbedRequiresDownloadingTheCrispEmbedEngineX { get; set; }
    public string DownloadModel { get; set; }
    public string ErrorDeletingFile { get; set; }
    public string CouldNotDeleteTheFileX { get; set; }
    public string MistralAPIKeyMissing { get; set; }
    public string YouMustEnterAValidMistralAPIKey { get; set; }
    public string DownloadGoogleLensOCR { get; set; }
    public string XGoogleLensOCRRequiresDownloadingGoogleLens { get; set; }
    public string TesseractOCRFailed { get; set; }
    public string CrispEmbedError { get; set; }
    public string TheCrispEmbedServerCouldNotBeStartedX { get; set; }
    public string DownloadTesseractOCR { get; set; }
    public string XTesseractRequiresDownloadingTesseractOCRXX { get; set; }
    public string PleaseInstallTesseract { get; set; }
    public string XTesseractWasNotDetectedPleaseInstallTesseract { get; set; }
    public string EGBrewInstallTesseract { get; set; }
    public string EGSudoAptInstallTesseractOcrOr { get; set; }
    public string UpdateCrispEmbed { get; set; }
    public string ANewerVersionOfCrispEmbedIsAvailableX { get; set; }
    public string DiscardOCRResult { get; set; }
    public string SomeItemsHaveOCRTextCloseAndDiscard { get; set; }

    public LanguageOcr()
    {
        DeleteBinaryOCRItem = "Delete Binary OCR item?";
        DoYouWantToDeleteTheCurrentBinary = "Do you want to delete the current Binary OCR item?";
        ValidationError = "Validation Error";
        ItemTextCannotBeEmpty = "Item text cannot be empty.";
        BinaryOCR = "Binary OCR";
        BinaryOCRCharacterUpdated = "Binary OCR character updated.";
        DeleteBinaryImageCompareItem = "Delete \"Binary image compare\" item?";
        DoYouWantToDeleteTheCurrentBinary2 = "Do you want to delete the current \"Binary image compare\" item?";
        BinaryOCRCharacterSaved = "Binary OCR character saved";
        DeleteBinaryImageCompareDatabase = "Delete Binary Image Compare database?";
        DoYouWantToDeleteTheCurrentBinary3 = "Do you want to delete the current \"Binary image compare\" database \"{0}\" with {1:#,###,##0} items?";
        DownloadPaddleOCR = "Download Paddle OCR?";
        XPaddleOCRRequiresDownloadingPaddleOCRX = "{0}\"Paddle OCR\" requires downloading Paddle OCR.{1}{2}Download and use Paddle OCR?";
        DeleteNOCRItem = "Delete nOCR item?";
        DoYouWantToDeleteTheCurrentNOCR = "Do you want to delete the current nOCR item?";
        NOCR = "nOCR";
        NOCRCharacterUpdated = "nOCR character updated.";
        NOCRCharacterSaved = "nOCR character saved";
        DeleteNOCRDatabase = "Delete nOCR database?";
        DoYouWantToDeleteTheCurrentNOCR2 = "Do you want to delete the current nOCR database \"{0}\" with {1:#,###,##0} items?";
        DownloadCrispEmbed = "Download CrispEmbed?";
        XCrispEmbedRequiresDownloadingTheCrispEmbedEngineX = "{0}\"CrispEmbed\" requires downloading the CrispEmbed engine.{1}{2}Download and use CrispEmbed?";
        DownloadModel = "Download model?";
        ErrorDeletingFile = "Error deleting file";
        CouldNotDeleteTheFileX = "Could not delete the file {0}.";
        MistralAPIKeyMissing = "Mistral API key missing";
        YouMustEnterAValidMistralAPIKey = "You must enter a valid Mistral API key.{0}{1}Get your API key from https://mistral.ai/";
        DownloadGoogleLensOCR = "Download Google Lens OCR?";
        XGoogleLensOCRRequiresDownloadingGoogleLens = "{0}\"Google Lens OCR\" requires downloading Google Lens OCR standalone.{1}{2}Download and use Google Lens OCR?";
        TesseractOCRFailed = "Tesseract OCR failed:";
        CrispEmbedError = "CrispEmbed error";
        TheCrispEmbedServerCouldNotBeStartedX = "The CrispEmbed server could not be started:{0}{1}{2}";
        DownloadTesseractOCR = "Download Tesseract OCR?";
        XTesseractRequiresDownloadingTesseractOCRXX = "{0}\"Tesseract\" requires downloading Tesseract OCR.{1}{2}Download and use Tesseract OCR?";
        PleaseInstallTesseract = "Please install Tesseract";
        XTesseractWasNotDetectedPleaseInstallTesseract = "{0}\"Tesseract\" was not detected. Please install Tesseract.";
        EGBrewInstallTesseract = "E.g. ´brew install tesseract´.";
        EGSudoAptInstallTesseractOcrOr = "E.g. ´sudo apt install tesseract-ocr´ or ´sudo pacman -S tesseract´.";
        UpdateCrispEmbed = "Update CrispEmbed?";
        ANewerVersionOfCrispEmbedIsAvailableX = "A newer version of CrispEmbed is available.{0}{1}Download it now?";
        DiscardOCRResult = "Discard OCR result?";
        SomeItemsHaveOCRTextCloseAndDiscard = "Some items have OCR text. Close and discard the OCR result?";

        LinesToDraw = "Lines to draw";
        CurrentImage = "Current image";
        AutoDrawAgain = "Auto draw again";
        StartOcr = "Start OCR";
        PauseOcr = "Pause OCR";
        InspectLine = "Inspect line...";
        OcrEngine = "OCR Engine";
        TesseractEngineMode = "Engine mode";
        TesseractEngineModeLegacy = "Original Tesseract only (can detect italic)";
        TesseractEngineModeNeural = "Neural nets LSTM only";
        TesseractEngineModeBoth = "Tesseract + LSTM";
        TesseractEngineModeDefault = "Default, based on what is available";
        Database = "Database";
        MaxWrongPixels = "Max wrong pixels";
        MaxErrorPct = "Max error %";
        NumberOfPixelsIsSpace = "Number of pixels is space";
        InspectImageMatches = "Inspect image matches";
        ResolutionXYAndTopmarginZ = "Resolution {0}x{1}, top margin {2}";
        RunningOcrDotDotDotXY = "Running OCR... {0}/{1}";
        RunningOcrDotDotDot = "Running OCR...";
        AutoSubmitFirstCharacter = "Auto submit first character";
        EditNOcrDatabase = "Edit nOCR database";
        ZoomFactorX = "Zoom factor: {0}x";
        ExpandInfoX = "Expand count: {0}";
        EditNOcrDatabaseXWithYItems = "Edit nOCR database {0} with {1:#,###,##0} items";
        NewNOcrDatabase = "New nOCR database";
        RenameNOcrDatabase = "Rename nOCR database";
        NOcrDatabase = "nOCR database";
        DrawMode = "Draw mode:";
        AddNewCharcter = "Add new character";
        LineIndexX = "Line {0}";
        InspectNOcrAdditions = "Inspect new nOCR additions";
        OcrSelectedLines = "OCR selected lines";
        FillSelectedLinesWithClipboard = "Fill selected lines with clipboard text";
        ShowImage = "Show image";
        FixOcrErrors = "Fix OCR errors";
        PromptForUknownWords = "Prompt for unknown words";
        TryToGuessUnknownWords = "Try to guess unknown words";
        AutoBreakIfMoreThanXLines = "Auto break if more than {0} lines";
        UnknownWords = "Unknown words";
        AllFixes = "All fixes";
        GuessesUsed = "Guesses used";
        Ocr = "OCR";
        OcrX = "OCR - {0}";
        AddBetterMatch = "Add better match";
        NOcrInspectImageMatches = "nOCR - Inspect image matches";
        AddToOcrPair = "Add to OCR replace pairs";
        AddNameToOcrReplaceList = "Add name to OCR replace list";
        WordToAdd = "Word to add";
        NameToAdd = "Name to add";
        ChangeWordFromTo = "Change word from/to";
        ClearBackground = "Clear background";
        ClearForeground = "Clear foreground";
        NOcrDrawHelp = "Tips for drawing\r\n────────────────────\r\n• Hold Ctrl down to continue line\r\n• Ctrl+z=undo, Ctrl+y=redo";
        EditWholeText = "Edit whole text";
        EditWordOnly = "Edit word only";
        ImagePreProcessing = "Image pre-processing";
        PreProcessingTitle = "Pre-processing";
        CropTransparent = "Crop transparent colors";
        InverseColors = "Inverse colors";
        RemoveBorders = "Remove borders";
        Binarize = "Binarize";
        BorderSize = "Border size";
        CaptureTopAlign = "Auto-detect ASSA alignment";
        OcrImage = "OCR image";
        OneColor = "One color (white)";
        DarknessThreshold = "Darkness threshold";
        EditExportDotDotDot = "Edit/export...";
        EditBinaryOcrDatabase = "Edit \"Binary image compare\" database";
        BinaryImageCompareDatabase = "\"Binary image compare\" database";
        RemoveXFromUnknownWordsList = "Remove \"{0}\" from unknown words list";
        DownloadingPaddleOcrEngineDotDotDot = "Downloading Paddle OCR engine...";
        DownloadingTesseractModel = "Downloading Tesseract model";
        SelectTesseractDictionary = "Select Tesseract dictionary:";
        Algorithm = "Algorithm";
        AutoDrawAlgorithmTooltip = "Algorithm used by Auto-draw to generate foreground/background lines";
        DownloadingPaddleOcrModelsDotDotDot = "Downloading Paddle OCR models...";
        PaddleOcr = "Paddle OCR";
        BinaryImageCompareInspectImageMatches = "\"Binary image compare\" - Inspect image matches";
        SaveBlankTextTitle = "Save blank text?";
        SaveBlankTextPrompt = "Save blank text for image?";
        YesAndNeverAskAgain = "Yes and never ask again";
        ImportTextFromSubtitleDotDotDot = "Import text from subtitle...";
        ImportTextFromSubtitleOverwritePrompt = "Some lines already have OCR text. Overwrite existing text?";
        ImportTextFromSubtitleNoMatchesFound = "No matching lines found in subtitle file.";
        ImportTextFromSubtitleXLinesImported = "Imported text for {0} line(s).";
        ExportTextAsSubtitleDotDotDot = "Export text as subtitle...";
        ExportTextAsSubtitleNoText = "No OCR text to export. Run OCR first or import text.";
        VobSubColors = "VobSub/DVD colors...";
        VobSubColorsTitle = "VobSub/DVD colors";
        VobSubColorsHeader = "Customize the four VobSub colors";
        VobSubColorsDescription = "VobSub/DVD subtitles use four indexed colors: background, pattern, emphasis 1 and emphasis 2. Pick each color below to override the embedded palette - useful when the original colors give poor OCR contrast.";
        VobSubColorBackground = "Background";
        VobSubColorPattern = "Pattern";
        VobSubColorEmphasis1 = "Emphasis 1";
        VobSubColorEmphasis2 = "Emphasis 2";
        VobSubColorsInvert = "Invert background / emphasis 1";
        VobSubIsolateColors = "Isolate VobSub colors for OCR";
        VobSubIsolateColorsHint = "Rebuild each VobSub image as crisp black-on-white before OCR by keeping the most frequent color (text) and dropping the outline/anti-alias colors. Improves recognition on discs where gray outlines merge characters together.";

        LlamaCppOcrSettingsTitle = "llama.cpp OCR settings";
        LlamaCppOcrPromptHint = "Use {language} to insert the selected OCR language.";
        LlamaCppOcrPromptEmpty = "The prompt cannot be empty.";
        LlamaCppOcrPromptMissingLanguagePlaceholder = "The prompt must contain the {language} placeholder.";
        LlamaCppOcrTimeoutMinutes = "Request timeout (minutes)";

        NOcrBinaryOcrFallbackDatabase = "BinaryOCR fallback database";
        NOcrBinaryOcrFallbackNone = "(none)";
        BinaryOcrNOcrFallbackDatabase = "nOCR fallback database";
        PickFallbackDatabase = "Pick fallback OCR database";
        FallbackOcrDatabase = "Fallback OCR database";

        ShowAllOllamaModels = "Show all models (not just vision-capable)";
        OllamaModelLikelyWrong = "Ollama returned no text - the selected model may not support OCR / vision";
        LlamaCppNotDownloaded = "llama.cpp engine/model not downloaded - download via batch convert settings";
        LlamaCppReturnedNoText = "llama.cpp returned no text - check the server and model";
        InspectBinaryOcrAdditions = "Inspect Binary OCR Additions";
        NewRenameBinaryImageCompareDatabase = "New/rename Binary Image Compare database";
        DownloadingCrispEmbed = "Downloading CrispEmbed";
        DownloadingGoogleLensOcr = "Downloading Google Lens OCR";
        CloudVisionApiKeyInvalid = "The Cloud Vision API key is invalid, or billing/API access is not enabled.";
        CloudVisionRequestForbidden = "Cloud Vision rejected the request. Check billing, API status, and the API key.";
        CloudVisionApiStatusErrorX = "The Cloud Vision API call failed with status code {0}.";
        CloudVisionApiCallErrorX = "Error calling the Cloud Vision API: {0}";
        DownloadingPaddleOcr = "Downloading Paddle OCR";
        DownloadingTesseract = "Downloading Tesseract";
        NewRenameNOcrDatabase = "New/rename nOCR database";
    }
}