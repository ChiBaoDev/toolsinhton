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
            new VietnameseTranslationBatch("B1", ["$.title", "$.version", "$.translatedBy", "$.cultureName", "$.general", "$.file", "$.edit", "$.help", "$.about"], false, "", "", "VietnameseDraft/01-general-file-edit.json"),
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
