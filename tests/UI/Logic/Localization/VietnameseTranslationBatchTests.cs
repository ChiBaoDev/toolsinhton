using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace UITests.Logic.Localization;

internal sealed record VietnameseTranslationBatch(
    string Id,
    IReadOnlyList<string> OwnedRoots,
    bool Reviewed,
    string Reviewer,
    string ReviewNote,
    string ShardFile);

internal sealed record UntranslatedAllowlistEntry(string Key, string Value, string Reason);

public class VietnameseTranslationBatchTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    [Fact]
    public void Manifest_MatchesTheStableBatchContract()
    {
        var batches = LoadBatches();
        var expected = new[]
        {
            new VietnameseTranslationBatch("B1", ["$.title", "$.version", "$.translatedBy", "$.cultureName", "$.general", "$.file", "$.edit", "$.help", "$.about"], true, "ChiBaoDev", "Reviewed metadata, general actions, file/edit operations, help, and about terminology against the Vietnamese glossary.", "VietnameseDraft/01-general-file-edit.json"),
            new VietnameseTranslationBatch("B2", ["$.main.menu", "$.main.toolbar", "$.main.waveform"], true, "ChiBaoDev", "Reviewed main menu mnemonics, toolbar labels/tooltips, and waveform navigation in context.", "VietnameseDraft/02-main-navigation.json"),
            new VietnameseTranslationBatch("B3", ["$.main", "$.waveform", "$.sync"], true, "ChiBaoDev", "Reviewed core editing statuses, waveform actions, timing, and synchronization terminology.", "VietnameseDraft/03-main-sync-waveform.json"),
            new VietnameseTranslationBatch("B4", ["$.tools", "$.spellCheck", "$.options", "$.plugins"], true, "ChiBaoDev", "Reviewed tools, spell-check, settings, and plugin terminology; technical engines and formats are explicitly classified.", "VietnameseDraft/04-tools-options.json"),
            new VietnameseTranslationBatch("B5", ["$.video", "$.ocr", "$.assa"], true, "ChiBaoDev", "Reviewed video, media processing, OCR, and Advanced SubStation Alpha terminology.", "VietnameseDraft/05-video-ocr-assa.json"),
            new VietnameseTranslationBatch("B6", ["$.translate"], true, "ChiBaoDev", "Reviewed translation workflow, API terminology, placeholders, and Vietnamese technical wording in context.", "VietnameseDraft/06-translate-remaining.json"),
        };

        Assert.Equal(expected.Length, batches.Count);
        for (var i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i].Id, batches[i].Id);
            Assert.Equal(expected[i].OwnedRoots, batches[i].OwnedRoots);
            Assert.Equal(expected[i].Reviewed, batches[i].Reviewed);
            Assert.Equal(expected[i].Reviewer, batches[i].Reviewer);
            Assert.Equal(expected[i].ReviewNote, batches[i].ReviewNote);
            Assert.Equal(expected[i].ShardFile, batches[i].ShardFile);
        }
    }

    [Fact]
    public void B1_HasExactMetadata()
    {
        var b1 = LoadBatches().Single(batch => batch.Id == "B1");

        using var english = LocalizationJsonHelper.LoadDocument(LocalizationTestPaths.EnglishLanguageFile());
        using var shard = LocalizationJsonHelper.LoadDocument(Path.Combine(TestDataFolder(), b1.ShardFile));

        Assert.Equal("Subtitle Edit", shard.RootElement.GetProperty("title").GetString());
        Assert.Equal(
            english.RootElement.GetProperty("version").GetString(),
            shard.RootElement.GetProperty("version").GetString());
        Assert.Equal("ChiBaoDev", shard.RootElement.GetProperty("translatedBy").GetString());
        Assert.Equal("vi-VN", shard.RootElement.GetProperty("cultureName").GetString());
    }

    [Fact]
    public void Manifest_OwnedRootsAreUniqueKnownAndUnambiguous()
    {
        var batches = LoadBatches();
        var leaves = LoadEnglishLeaves();
        var roots = batches.SelectMany(batch => batch.OwnedRoots.Select(root => (batch.Id, Root: root))).ToArray();

        var duplicateRoots = roots.GroupBy(item => item.Root, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToArray();
        Assert.True(duplicateRoots.Length == 0, $"Duplicate owned roots: {string.Join(", ", duplicateRoots)}");

        var unknownRoots = roots.Where(item => !leaves.Keys.Any(path => IsOwnedPath(path, item.Root)))
            .Select(item => item.Root)
            .ToArray();
        Assert.True(unknownRoots.Length == 0, $"Owned roots matching no English leaf: {string.Join(", ", unknownRoots)}");

        var errors = new List<string>();
        foreach (var leafPath in leaves.Keys)
        {
            var matches = roots.Where(item => IsOwnedPath(leafPath, item.Root)).ToArray();
            if (matches.Length == 0)
            {
                errors.Add($"{leafPath}: no owner.");
                continue;
            }

            var longestLength = matches.Max(item => item.Root.Length);
            var owners = matches.Where(item => item.Root.Length == longestLength)
                .Select(item => item.Id)
                .Distinct(StringComparer.Ordinal)
                .ToArray();
            if (owners.Length != 1)
                errors.Add($"{leafPath}: equal-specificity owners {string.Join(", ", owners)}.");
        }

        Assert.True(errors.Count == 0, string.Join(Environment.NewLine, errors));
    }

    [Fact]
    public void Manifest_UsesPathSegmentsAndLongestRootForNestedMainAreas()
    {
        var batches = LoadBatches();

        Assert.Equal("B2", ResolveOwner("$.main.menu.file.open", batches));
        Assert.Equal("B2", ResolveOwner("$.main.toolbar.open", batches));
        Assert.Equal("B2", ResolveOwner("$.main.waveform[0]", batches));
        Assert.Equal("B3", ResolveOwner("$.main.videoControls.play", batches));
        Assert.Null(ResolveOwner("$.mainland.menu", batches));
    }

    [Fact]
    public void Allowlist_ContainsOnlyExactEnglishLeavesWithReasons()
    {
        var entries = LoadJson<UntranslatedAllowlistEntry[]>("VietnameseUntranslatedAllowlist.json");
        var leaves = LoadEnglishLeaves();
        var errors = new List<string>();

        foreach (var duplicate in entries
                     .GroupBy(entry => entry.Key, StringComparer.Ordinal)
                     .Where(group => group.Count() > 1))
        {
            errors.Add($"{duplicate.Key}: duplicate allowlist key.");
        }

        foreach (var entry in entries)
        {
            if (entry.Key.Contains('*', StringComparison.Ordinal))
                errors.Add($"{entry.Key}: wildcards are forbidden.");
            if (!entry.Key.StartsWith("$.", StringComparison.Ordinal))
                errors.Add($"{entry.Key}: key must be an exact $. path.");
            if (!leaves.TryGetValue(entry.Key, out var leaf))
                errors.Add($"{entry.Key}: key is not an English leaf.");
            else if (leaf.Kind != JsonValueKind.String || !string.Equals(leaf.StringValue, entry.Value, StringComparison.Ordinal))
                errors.Add($"{entry.Key}: value does not exactly match English.");
            if (string.IsNullOrWhiteSpace(entry.Reason))
                errors.Add($"{entry.Key}: reason is required.");
        }

        Assert.True(errors.Count == 0, string.Join(Environment.NewLine, errors));
    }

    [Fact]
    public void ReviewedBatches_HaveExactOwnedStringLeavesValidPlaceholdersAndAllowlistedEnglishValues()
    {
        var batches = LoadBatches();
        var englishLeaves = LoadEnglishLeaves();
        var englishLeafPathOrder = LoadEnglishLeavesInSourceOrder().Select(leaf => leaf.Path).ToArray();
        var reviewedLeafCounts = new Dictionary<string, int>(StringComparer.Ordinal);
        var allowlist = LoadJson<UntranslatedAllowlistEntry[]>("VietnameseUntranslatedAllowlist.json")
            .ToDictionary(entry => entry.Key, StringComparer.Ordinal);
        var errors = new List<string>();
        var englishIdenticalReviewedPaths = new HashSet<string>(StringComparer.Ordinal);

        foreach (var batch in batches.Where(batch => batch.Reviewed))
        {
            var shardPath = Path.Combine(TestDataFolder(), batch.ShardFile);
            if (!File.Exists(shardPath))
                continue;

            using var shard = LocalizationJsonHelper.LoadDocument(shardPath);
            var shardLeaves = LocalizationJsonHelper.FlattenLeaves(shard.RootElement);
            var shardLeavesInSourceOrder = LocalizationJsonHelper.FlattenLeavesInSourceOrder(shard.RootElement);
            var expectedPathSequence = englishLeafPathOrder
                .Where(path => string.Equals(ResolveOwner(path, batches), batch.Id, StringComparison.Ordinal))
                .ToArray();
            var expectedPaths = expectedPathSequence.ToHashSet(StringComparer.Ordinal);
            var actualPathSequence = shardLeavesInSourceOrder.Select(leaf => leaf.Path).ToArray();
            var actualPaths = actualPathSequence.ToHashSet(StringComparer.Ordinal);
            reviewedLeafCounts[batch.Id] = actualPathSequence.Length;

            foreach (var path in expectedPaths.Except(actualPaths, StringComparer.Ordinal))
                errors.Add($"{batch.Id}: missing owned leaf {path}.");
            foreach (var path in actualPaths.Except(expectedPaths, StringComparer.Ordinal))
                errors.Add($"{batch.Id}: extra or non-owned leaf {path}.");

            if (expectedPaths.SetEquals(actualPathSequence))
            {
                var orderError = LocalizationJsonHelper.CompareLeafPathOrder(expectedPathSequence, actualPathSequence);
                if (orderError is not null)
                    errors.Add($"{batch.Id}: {orderError}");
            }

            foreach (var path in expectedPaths.Intersect(actualPaths, StringComparer.Ordinal))
            {
                var english = englishLeaves[path];
                var translated = shardLeaves[path];
                if (translated.Kind != JsonValueKind.String)
                {
                    errors.Add($"{batch.Id}: {path} must be a string leaf.");
                    continue;
                }

                if (english.Kind != JsonValueKind.String)
                {
                    errors.Add($"{batch.Id}: English leaf {path} is not a string.");
                    continue;
                }

                try
                {
                    foreach (var error in CompositeFormatPlaceholderParser.Compare(english.StringValue!, translated.StringValue!))
                        errors.Add($"{batch.Id}: {path}: {error}");
                }
                catch (FormatException exception)
                {
                    errors.Add($"{batch.Id}: {path}: invalid composite format: {exception.Message}");
                }

                if (string.Equals(english.StringValue, translated.StringValue, StringComparison.Ordinal))
                    englishIdenticalReviewedPaths.Add(path);
            }
        }

        var relevantAllowlistKeys = allowlist.Keys.ToHashSet(StringComparer.Ordinal);

        foreach (var path in englishIdenticalReviewedPaths.Except(relevantAllowlistKeys, StringComparer.Ordinal))
            errors.Add($"{path} is identical to English without an exact allowlist entry.");
        foreach (var path in relevantAllowlistKeys.Except(englishIdenticalReviewedPaths, StringComparer.Ordinal))
            errors.Add($"{path} is a stale or extra allowlist entry; the reviewed value is not identical to English.");

        Assert.Equal(1_076, reviewedLeafCounts["B1"]);
        Assert.Equal(171, reviewedLeafCounts["B2"]);
        Assert.Equal(203, reviewedLeafCounts["B3"]);
        Assert.Equal(1_242, reviewedLeafCounts["B4"]);
        Assert.Equal(681, reviewedLeafCounts["B5"]);
        Assert.Equal(27, reviewedLeafCounts["B6"]);
        Assert.Equal(3_400, reviewedLeafCounts.Values.Sum());
        Assert.True(errors.Count == 0, string.Join(Environment.NewLine, errors));
    }

    [Fact]
    public void B6_PreservesSourceSignificantNewlinesAndPlaceholders()
    {
        var b6 = LoadBatches().Single(batch => batch.Id == "B6");
        using var english = LocalizationJsonHelper.LoadDocument(LocalizationTestPaths.EnglishLanguageFile());
        using var shard = LocalizationJsonHelper.LoadDocument(Path.Combine(TestDataFolder(), b6.ShardFile));
        var englishLeaves = LocalizationJsonHelper.FlattenLeaves(english.RootElement);
        var shardLeaves = LocalizationJsonHelper.FlattenLeaves(shard.RootElement);

        Assert.Equal(
            englishLeaves["$.translate.translationFailedHint"].StringValue!.Count(character => character == '\n'),
            shardLeaves["$.translate.translationFailedHint"].StringValue!.Count(character => character == '\n'));
        Assert.Empty(CompositeFormatPlaceholderParser.Compare(
            englishLeaves["$.translate.blockXOfY"].StringValue!,
            shardLeaves["$.translate.blockXOfY"].StringValue!));
        Assert.Empty(CompositeFormatPlaceholderParser.Compare(
            englishLeaves["$.translate.xIsAlreadyDownloadedReDownload"].StringValue!,
            shardLeaves["$.translate.xIsAlreadyDownloadedReDownload"].StringValue!));
    }

    [Fact]
    public void B5_PreservesDecodedAssaSyntaxAndRejectsControlCharacterCorruption()
    {
        var batches = LoadBatches();
        var b5 = batches.Single(batch => batch.Id == "B5");
        using var english = LocalizationJsonHelper.LoadDocument(LocalizationTestPaths.EnglishLanguageFile());
        using var shard = LocalizationJsonHelper.LoadDocument(Path.Combine(TestDataFolder(), b5.ShardFile));
        var englishLeaves = LocalizationJsonHelper.FlattenLeaves(english.RootElement);
        var shardLeaves = LocalizationJsonHelper.FlattenLeaves(shard.RootElement);
        var syntaxPaths = englishLeaves
            .Where(pair => pair.Value.Kind == JsonValueKind.String && pair.Value.StringValue!.Contains('\\'))
            .Select(pair => pair.Key)
            .Where(path => IsOwnedPath(path, "$.video") || IsOwnedPath(path, "$.ocr") || IsOwnedPath(path, "$.assa"))
            .ToArray();

        foreach (var path in syntaxPaths)
        {
            var source = englishLeaves[path].StringValue!;
            var translated = shardLeaves[path].StringValue!;
            Assert.DoesNotContain(translated, character => character < (char)32 && character is not (char)13 and not (char)10 and not (char)9);
            var tagPattern = @"\\(?:N|n|u[01]|an[1-9]|fsp[-+]?\d+(?:\.\d+)?|pos\([-+]?\d+(?:\.\d+)?,[-+]?\d+(?:\.\d+)?\))";
            var sourceTags = Regex.Matches(source, tagPattern).Select(match => match.Value).ToArray();
            var translatedTags = Regex.Matches(translated, tagPattern).Select(match => match.Value).ToArray();
            Assert.Equal(sourceTags, translatedTags);
        }

        Assert.Equal(@"Thêm thẻ vị trí ASSA (ví dụ: {\an8})", shardLeaves["$.video.videoOcr.addAssaPositionTag"].StringValue);
        Assert.Equal(@"Tăng khoảng cách giữa các từ bằng thẻ \fsp để văn bản dễ đọc hơn", shardLeaves["$.assa.advancedEffectWordSpacingDescription"].StringValue);
    }

    [Fact]
    public void B2_MenuMnemonics_ArePreservedAndUniqueWithinDisplayedSiblingGroups()
    {
        var batches = LoadBatches();
        var b2 = batches.Single(batch => batch.Id == "B2");
        using var english = LocalizationJsonHelper.LoadDocument(LocalizationTestPaths.EnglishLanguageFile());
        using var shard = LocalizationJsonHelper.LoadDocument(Path.Combine(TestDataFolder(), b2.ShardFile));
        var englishMenu = english.RootElement.GetProperty("main").GetProperty("menu");
        var translatedMenu = shard.RootElement.GetProperty("main").GetProperty("menu");
        var reviewedLeaves = LoadReviewedShardLeaves(batches);
        var errors = new List<string>();

        foreach (var property in englishMenu.EnumerateObject())
        {
            var englishValue = property.Value.GetString()!;
            var translatedValue = translatedMenu.GetProperty(property.Name).GetString()!;
            Assert.Equal(englishValue.Count(character => character == '_'), translatedValue.Count(character => character == '_'));
        }

        // These groups mirror the MenuItem siblings built by InitMenu.Make. Paths outside
        // $.main.menu are included when their Vietnamese value is available in a reviewed shard.
        var siblingGroups = new Dictionary<string, string[]>
        {
            ["top-level base + ASSA (InitMenu.Make menu.Items when ASSA is visible)"] = [.. TopLevelBaseMenuPaths(), "$.main.menu.assaTools"],
            ["top-level base + SSA (InitMenu.Make menu.Items when SSA is visible)"] = [.. TopLevelBaseMenuPaths(), "$.main.menu.ssaTools"],
            ["file (File.Items)"] = MenuPaths("new", "newKeepVideo", "newWindow", "open", "openKeepVideo", "openOriginal", "closeOriginal", "closeTranslation", "reopen", "restoreAutoBackup", "save", "saveAs", "openContainingFolder", "compare", "statistics", "import", "export", "exit"),
            ["file import (Import.Items)"] = ["$.file.import.subtitleWithManuallyChosenEncodingDotDotDot", "$.file.import.imageBasedSubtitleForOcrDotDotDot", "$.file.import.imageBasedSubtitleForEditDotDotDot", "$.file.import.imagesForOcrDotDotDot", "$.file.import.plainTextDotDotDot", "$.file.import.csvXlsxCustomColumnsDotDotDot", "$.file.import.timeCodesDotDotDot", "$.file.import.formattingDotDotDot"],
            ["file export (Export.Items)"] = ["$.general.bluRaySup", "$.general.bdnXml", "$.file.export.titleExportDCinemaInteropPng", "$.file.export.titleExportDCinemaSmpte2014Png", "$.file.export.titleExportDvdSup", "$.general.imagesWithTimeCode", "$.file.export.titleExportVobSub", "$.file.export.customTextFormatsDotDotDot", "$.file.export.plainTextDotDotDot"],
            ["edit (Edit.Items)"] = [.. MenuPaths("undo", "redo", "showHistory", "find", "findNext", "replace", "multipleReplace", "goToLineNumber", "rightToLeftMode", "fixRightToLeftViaUnicodeControlCharacters", "removeUnicodeControlCharacters", "reverseRightToLeftStartEnd", "modifySelectionDotDotDot"), "$.general.invertSelection", "$.general.selectAll"],
            ["tools (menuItemTools.Items)"] = MenuPaths("adjustDurations", "applyDurationLimits", "batchConvert", "beautifyTimeCodes", "bridgeGaps", "applyMinGap", "changeCasing", "changeFormatting", "fixCommonErrors", "checkAndFixNetflixErrors", "aiReview", "makeEmptyTranslationFromCurrentSubtitle", "mergeLinesWithSameText", "mergeLinesWithSameTimeCodes", "splitBreakLongLines", "mergeShortLines", "mergeContinuationLines", "snapAllTimesToFrames", "mergeTwoSubtitles", "sortSubtitles", "renumber", "removeTextForHearingImpaired", "convertActors", "joinSubtitles", "splitSubtitle"),
            ["spell-check (SpellCheckTitle.Items)"] = MenuPaths("spellCheck", "findDoubleWords", "findDoubleLines", "addNameToNamesList", "getDictionaries"),
            ["video (Video.Items)"] = [.. MenuPaths("openVideo", "openVideoFromUrl", "closeVideoFile", "audioTracks", "speechToText", "textToSpeech", "videoOcr", "generateBurnIn", "generateTransparent", "generateImportShotChanges", "listShotChanges", "undockVideoControls", "toggleSelectSubtitleWhilePlayingCurrentlyOn", "toggleSelectSubtitleWhilePlayingCurrentlyOff", "dockVideoControls"), "$.general.more"],
            ["synchronization (Synchronization.Items)"] = MenuPaths("adjustAllTimes", "visualSync", "pointSync", "pointSyncViaOther", "changeFrameRate", "changeSpeed"),
            ["translate (Translate.Items)"] = MenuPaths("autoTranslate", "translateViaCopyPaste"),
            ["options (Options.Items)"] = MenuPaths("settings", "shortcuts", "wordLists", "chooseLanguage"),
            ["help (HelpTitle.Items)"] = MenuPaths("checkForUpdates", "help", "about"),
            ["ASSA tools (menuItemAssaTools.Items)"] = MenuPaths("assaProgressBar", "assaChangeResolution", "assaGenerateBackground", "assaImageColorPicker", "assaSetPosition", "assaApplyAdvancedEffects", "assaApplyCustomOverrideTags", "assaDraw", "assaProperties", "assaAttachments", "assaStyles", "filterLayersForDisplayDotDotDot"),
            ["SSA tools (menuItemSsaTools.Items)"] = MenuPaths("assaStyles", "assaProperties", "assaAttachments"),
        };

        foreach (var (groupName, paths) in siblingGroups)
        {
            var mnemonics = paths
                .Where(reviewedLeaves.ContainsKey)
                .Select(path => (Path: path, Mnemonic: ExtractMnemonic(reviewedLeaves[path], path, errors)))
                .Where(item => item.Mnemonic is not null)
                .ToArray();
            foreach (var duplicate in mnemonics.GroupBy(item => item.Mnemonic, StringComparer.Ordinal).Where(group => group.Count() > 1))
                errors.Add($"{groupName}: mnemonic '{duplicate.Key}' is duplicated by {string.Join(", ", duplicate.Select(item => item.Path))}.");
        }

        Assert.True(errors.Count == 0, string.Join(Environment.NewLine, errors));
    }

    [Fact]
    public void MnemonicExtraction_AcceptsUnicodeLettersAndDigitsAndRejectsInvalidTextElements()
    {
        var errors = new List<string>();

        Assert.Equal("Ế", ExtractMnemonic("T_ế", "composed", errors));
        Assert.Equal("Ế", ExtractMnemonic("T_ế", "decomposed", errors));
        Assert.Equal("7", ExtractMnemonic("M_7", "digit", errors));
        Assert.Null(ExtractMnemonic("M_ ", "whitespace", errors));
        Assert.Null(ExtractMnemonic("M_-", "punctuation", errors));
        Assert.Null(ExtractMnemonic("M__", "underscore", errors));
        Assert.Null(ExtractMnemonic("M_😀", "emoji", errors));
        Assert.Null(ExtractMnemonic("M_a‍", "invalid-grapheme", errors));
        Assert.Equal(5, errors.Count);
    }

    [Fact]
    public void ReviewedBatches_HaveReviewMetadataAndExistingShardFiles()
    {
        var errors = new List<string>();
        foreach (var batch in LoadBatches().Where(batch => batch.Reviewed))
        {
            if (string.IsNullOrWhiteSpace(batch.Reviewer))
                errors.Add($"{batch.Id}: reviewer is required.");
            if (string.IsNullOrWhiteSpace(batch.ReviewNote))
                errors.Add($"{batch.Id}: review note is required.");
            if (!File.Exists(Path.Combine(TestDataFolder(), batch.ShardFile)))
                errors.Add($"{batch.Id}: shard file '{batch.ShardFile}' does not exist.");
        }

        Assert.True(errors.Count == 0, string.Join(Environment.NewLine, errors));
    }

    private static IReadOnlyList<VietnameseTranslationBatch> LoadBatches() =>
        LoadJson<VietnameseTranslationBatch[]>("VietnameseTranslationBatches.json");

    private static IReadOnlyDictionary<string, LocalizationLeaf> LoadEnglishLeaves()
    {
        using var english = LocalizationJsonHelper.LoadDocument(LocalizationTestPaths.EnglishLanguageFile());
        return LocalizationJsonHelper.FlattenLeaves(english.RootElement);
    }

    private static IReadOnlyList<LocalizationLeaf> LoadEnglishLeavesInSourceOrder()
    {
        using var english = LocalizationJsonHelper.LoadDocument(LocalizationTestPaths.EnglishLanguageFile());
        return LocalizationJsonHelper.FlattenLeavesInSourceOrder(english.RootElement);
    }

    private static T LoadJson<T>(string fileName) =>
        JsonSerializer.Deserialize<T>(File.ReadAllText(Path.Combine(TestDataFolder(), fileName)), JsonOptions)
        ?? throw new InvalidDataException($"Could not deserialize '{fileName}'.");

    private static string TestDataFolder() => Path.Combine(AppContext.BaseDirectory, "TestData");

    private static IReadOnlyDictionary<string, string> LoadReviewedShardLeaves(IReadOnlyList<VietnameseTranslationBatch> batches)
    {
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var batch in batches.Where(batch => batch.Reviewed))
        {
            using var shard = LocalizationJsonHelper.LoadDocument(Path.Combine(TestDataFolder(), batch.ShardFile));
            foreach (var (path, leaf) in LocalizationJsonHelper.FlattenLeaves(shard.RootElement))
            {
                if (leaf.Kind == JsonValueKind.String)
                    values[path] = leaf.StringValue!;
            }
        }

        return values;
    }

    private static string[] MenuPaths(params string[] keys) =>
        keys.Select(key => $"$.main.menu.{key}").ToArray();

    private static string[] TopLevelBaseMenuPaths() =>
    [
        "$.main.menu.file",
        "$.main.menu.edit",
        "$.main.menu.tools",
        "$.plugins.title",
        "$.main.menu.spellCheckTitle",
        "$.main.menu.video",
        "$.main.menu.synchronization",
        "$.main.menu.translate",
        "$.main.menu.options",
        "$.main.menu.helpTitle",
    ];

    private static string? ExtractMnemonic(string value, string path, ICollection<string> errors)
    {
        var underscore = value.IndexOf('_');
        if (underscore < 0)
            return null;

        if (underscore + 1 >= value.Length)
        {
            errors.Add($"{path}: mnemonic underscore has no following text element.");
            return null;
        }

        var element = StringInfo.GetNextTextElement(value, underscore + 1);
        var runes = element.EnumerateRunes().ToArray();
        var isUsable = runes.Length > 0 && Rune.IsLetterOrDigit(runes[0]) &&
                       runes.Skip(1).All(rune => Rune.GetUnicodeCategory(rune) is
                           UnicodeCategory.NonSpacingMark or UnicodeCategory.SpacingCombiningMark or UnicodeCategory.EnclosingMark);
        if (!isUsable)
        {
            errors.Add($"{path}: mnemonic '{element}' is not a letter or digit text element.");
            return null;
        }

        return element.Normalize(NormalizationForm.FormC).ToUpperInvariant();
    }

    private static bool IsOwnedPath(string path, string root) =>
        string.Equals(path, root, StringComparison.Ordinal) ||
        path.StartsWith(root + ".", StringComparison.Ordinal) ||
        path.StartsWith(root + "[", StringComparison.Ordinal);

    private static string? ResolveOwner(string path, IReadOnlyList<VietnameseTranslationBatch> batches)
    {
        var matches = batches.SelectMany(batch => batch.OwnedRoots.Select(root => (batch.Id, Root: root)))
            .Where(item => IsOwnedPath(path, item.Root))
            .ToArray();
        if (matches.Length == 0)
            return null;

        var longestLength = matches.Max(item => item.Root.Length);
        var owners = matches.Where(item => item.Root.Length == longestLength)
            .Select(item => item.Id)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        return owners.Length == 1 ? owners[0] : throw new InvalidDataException($"Ambiguous owner for '{path}'.");
    }
}
