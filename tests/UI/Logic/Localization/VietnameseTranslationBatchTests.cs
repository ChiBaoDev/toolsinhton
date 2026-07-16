using System.Text.Json;

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
            new VietnameseTranslationBatch("B2", ["$.main.menu", "$.main.toolbar", "$.main.waveform"], false, "", "", "VietnameseDraft/02-main-navigation.json"),
            new VietnameseTranslationBatch("B3", ["$.main", "$.waveform", "$.sync"], false, "", "", "VietnameseDraft/03-main-sync-waveform.json"),
            new VietnameseTranslationBatch("B4", ["$.tools", "$.spellCheck", "$.options", "$.plugins"], false, "", "", "VietnameseDraft/04-tools-options.json"),
            new VietnameseTranslationBatch("B5", ["$.video", "$.ocr", "$.assa"], false, "", "", "VietnameseDraft/05-video-ocr-assa.json"),
            new VietnameseTranslationBatch("B6", ["$.translate"], false, "", "", "VietnameseDraft/06-translate-remaining.json"),
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
            var expectedPaths = englishLeaves.Keys
                .Where(path => string.Equals(ResolveOwner(path, batches), batch.Id, StringComparison.Ordinal))
                .ToHashSet(StringComparer.Ordinal);
            var actualPaths = shardLeaves.Keys.ToHashSet(StringComparer.Ordinal);

            foreach (var path in expectedPaths.Except(actualPaths, StringComparer.Ordinal))
                errors.Add($"{batch.Id}: missing owned leaf {path}.");
            foreach (var path in actualPaths.Except(expectedPaths, StringComparer.Ordinal))
                errors.Add($"{batch.Id}: extra or non-owned leaf {path}.");

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

        var reviewedBatchIds = batches
            .Where(batch => batch.Reviewed)
            .Select(batch => batch.Id)
            .ToHashSet(StringComparer.Ordinal);
        var relevantAllowlistKeys = allowlist.Keys
            .Where(path =>
            {
                var owner = ResolveOwner(path, batches);
                return owner is not null && reviewedBatchIds.Contains(owner);
            })
            .ToHashSet(StringComparer.Ordinal);

        foreach (var path in englishIdenticalReviewedPaths.Except(relevantAllowlistKeys, StringComparer.Ordinal))
            errors.Add($"{path} is identical to English without an exact allowlist entry.");
        foreach (var path in relevantAllowlistKeys.Except(englishIdenticalReviewedPaths, StringComparer.Ordinal))
            errors.Add($"{path} is a stale or extra allowlist entry; the reviewed value is not identical to English.");

        Assert.True(errors.Count == 0, string.Join(Environment.NewLine, errors));
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

    private static T LoadJson<T>(string fileName) =>
        JsonSerializer.Deserialize<T>(File.ReadAllText(Path.Combine(TestDataFolder(), fileName)), JsonOptions)
        ?? throw new InvalidDataException($"Could not deserialize '{fileName}'.");

    private static string TestDataFolder() => Path.Combine(AppContext.BaseDirectory, "TestData");

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
