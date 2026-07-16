using System.Text.Json;

namespace UITests.Logic.Localization;

public class LocalizationJsonHelperTests
{
    [Fact]
    public void FlattenLeaves_UsesStableJsonPaths()
    {
        using var document = JsonDocument.Parse("""
        { "general": { "ok": "OK", "items": ["a", "b"] } }
        """);

        var leaves = LocalizationJsonHelper.FlattenLeaves(document.RootElement);

        Assert.Equal(["$.general.items[0]", "$.general.items[1]", "$.general.ok"],
            leaves.Keys.OrderBy(p => p));
    }

    [Fact]
    public void CompareNodeShape_ReportsMissingExtraAndWrongKind()
    {
        using var expected = JsonDocument.Parse("""{ "a": { "b": "x" }, "c": "y" }""");
        using var actual = JsonDocument.Parse("""{ "a": "x", "d": "z" }""");

        var errors = LocalizationJsonHelper.CompareNodeShape(expected.RootElement, actual.RootElement);

        Assert.Contains(errors, e => e.Contains("$.a") && e.Contains("kind"));
        Assert.Contains(errors, e => e.Contains("$.c") && e.Contains("missing"));
        Assert.Contains(errors, e => e.Contains("$.d") && e.Contains("extra"));
    }

    [Fact]
    public void RequireStringLeaves_RejectsNonStringLeaves()
    {
        using var document = JsonDocument.Parse("""{ "ok": "yes", "bad": 42 }""");

        var exception = Assert.Throws<InvalidDataException>(
            () => LocalizationJsonHelper.RequireStringLeaves(document.RootElement));

        Assert.Contains("$.bad", exception.Message);
    }
}
