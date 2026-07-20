using Avalonia.Platform;
using System;
using System.IO;
using System.Text.Json;

namespace Nikse.SubtitleEdit.Logic.Config.Language;

internal enum LanguageSource { BuiltInEnglish, WritableFile, EmbeddedResource }
internal enum LanguageLoadFailure { None, Missing, MalformedJson, InvalidShape, ResourceUnavailable }
internal sealed record LanguageLoadResult(bool Success, string RequestedName, SeLanguage? Language, LanguageSource Source, LanguageLoadFailure Failure, string? Diagnostic);
internal interface ILanguageCandidateLoader { LanguageLoadResult TryLoad(string languageName); }

internal sealed class LanguageCandidateLoader : ILanguageCandidateLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly string _translationFolder;
    public LanguageCandidateLoader() : this(Se.TranslationFolder) { }
    internal LanguageCandidateLoader(string translationFolder) => _translationFolder = translationFolder;

    public LanguageLoadResult TryLoad(string languageName)
    {
        if (string.Equals(languageName, "English", StringComparison.OrdinalIgnoreCase))
            return Success(languageName, new SeLanguage(), LanguageSource.BuiltInEnglish);
        var file = Path.Combine(_translationFolder, languageName + ".json");
        LanguageLoadFailure diskFailure = LanguageLoadFailure.Missing; string? diagnostic = null;
        if (System.IO.File.Exists(file))
        {
            var disk = TryDeserialize(languageName, () => System.IO.File.OpenRead(file), LanguageSource.WritableFile);
            if (disk.Success) return disk;
            diskFailure = disk.Failure; diagnostic = disk.Diagnostic;
        }
        if (string.Equals(languageName, "Vietnamese", StringComparison.OrdinalIgnoreCase))
        {
            var embedded = TryDeserialize(languageName, () => AssetLoader.Open(new Uri("avares://SubtitleEdit/Assets/Languages/Vietnamese.json")), LanguageSource.EmbeddedResource);
            if (embedded.Success) return embedded;
            return embedded;
        }
        return new(false, languageName, null, LanguageSource.WritableFile, diskFailure, diagnostic ?? $"Language file '{file}' was not found.");
    }

    private static LanguageLoadResult TryDeserialize(string name, Func<Stream> open, LanguageSource source)
    {
        try
        {
            using var stream = open();
            using var document = JsonDocument.Parse(stream);
            if (!HasValidShape(document.RootElement))
                return new(false, name, null, source, LanguageLoadFailure.InvalidShape, "Language JSON does not match the required runtime shape.");
            var language = document.RootElement.Deserialize<SeLanguage>(JsonOptions);
            return language == null ? new(false, name, null, source, LanguageLoadFailure.InvalidShape, "Language JSON deserialized to null.") : Success(name, language, source);
        }
        catch (JsonException ex) { return new(false, name, null, source, LanguageLoadFailure.MalformedJson, ex.Message); }
        catch (Exception ex) { return new(false, name, null, source, LanguageLoadFailure.ResourceUnavailable, ex.Message); }
    }

    private static bool HasValidShape(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object) return false;
        foreach (var property in typeof(SeLanguage).GetProperties())
            if (!TryGetProperty(root, property.Name, out var value) || value.ValueKind == JsonValueKind.Null) return false;
        return true;
    }
    private static bool TryGetProperty(JsonElement element, string name, out JsonElement value)
    {
        foreach (var property in element.EnumerateObject()) if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase)) { value = property.Value; return true; }
        value = default; return false;
    }
    private static LanguageLoadResult Success(string name, SeLanguage language, LanguageSource source) => new(true, name, language, source, LanguageLoadFailure.None, null);
}
