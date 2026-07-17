namespace Nikse.SubtitleEdit.Logic.Config.Language.Options;

public class LanguageOptions
{
    public LanguageSettings Settings { get; set; } = new();
    public LanguageSettingsShortcuts Shortcuts { get; set; } = new();
    public LanguageSettingsWordLists WordLists { get; set; } = new();
    public LanguageChooseLanguage ChooseLanguage { get; set; } = new();

    public string FileAssociationError { get; set; }
    public string NoProfileSelectedForExport { get; set; }
    public string UnableToImportProfiles { get; set; }
    public string NoProfilesFoundInFile { get; set; }
    public string NoShortcutsFoundInFile { get; set; }
    public string FailedToImportShortcutsRNX { get; set; }
    public string FailedToImportSE4ShortcutsRN { get; set; }
    public string FailedToExportShortcutsRNX { get; set; }

    public LanguageOptions()
    {
        FileAssociationError = "File Association Error";
        NoProfileSelectedForExport = "No profile selected for export";
        UnableToImportProfiles = "Unable to import profiles: ";
        NoProfilesFoundInFile = "No profiles found in file";
        NoShortcutsFoundInFile = "No shortcuts found in file.";
        FailedToImportShortcutsRNX = "Failed to import shortcuts:\r\n{0}";
        FailedToImportSE4ShortcutsRN = "Failed to import SE 4 shortcuts:\r\n{0}";
        FailedToExportShortcutsRNX = "Failed to export shortcuts:\r\n{0}";

    }
}