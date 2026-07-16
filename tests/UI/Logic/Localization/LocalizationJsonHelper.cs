using System.Text.Json;

namespace UITests.Logic.Localization;

internal sealed record LocalizationLeaf(string Path, JsonValueKind Kind, string? StringValue);

internal static class LocalizationJsonHelper
{
    internal static JsonDocument LoadDocument(string filePath) =>
        JsonDocument.Parse(File.ReadAllText(filePath));

    internal static IReadOnlyDictionary<string, LocalizationLeaf> FlattenLeaves(JsonElement root)
    {
        var result = new SortedDictionary<string, LocalizationLeaf>(StringComparer.Ordinal);
        Visit(root, "$", result);
        return result;
    }

    internal static IReadOnlyList<string> CompareNodeShape(JsonElement expected, JsonElement actual)
    {
        var errors = new List<string>();
        Compare(expected, actual, "$", errors);
        return errors;
    }

    internal static void RequireStringLeaves(JsonElement root)
    {
        foreach (var leaf in FlattenLeaves(root).Values)
        {
            if (leaf.Kind != JsonValueKind.String)
            {
                throw new InvalidDataException($"Localization leaf '{leaf.Path}' must be a string, not {leaf.Kind}.");
            }
        }
    }

    internal static string NormalizeNewLines(string value) =>
        value.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');

    private static void Visit(JsonElement element, string path, IDictionary<string, LocalizationLeaf> result)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject().OrderBy(p => p.Name, StringComparer.Ordinal))
            {
                Visit(property.Value, $"{path}.{property.Name}", result);
            }
            return;
        }

        if (element.ValueKind == JsonValueKind.Array)
        {
            var index = 0;
            foreach (var item in element.EnumerateArray())
            {
                Visit(item, $"{path}[{index}]", result);
                index++;
            }
            return;
        }

        result[path] = new LocalizationLeaf(
            path,
            element.ValueKind,
            element.ValueKind == JsonValueKind.String ? element.GetString() : element.GetRawText());
    }

    private static void Compare(JsonElement expected, JsonElement actual, string path, ICollection<string> errors)
    {
        if (expected.ValueKind != actual.ValueKind)
        {
            errors.Add($"{path}: expected kind {expected.ValueKind}, actual {actual.ValueKind}.");
            return;
        }

        if (expected.ValueKind == JsonValueKind.Object)
        {
            var expectedProperties = expected.EnumerateObject().ToDictionary(p => p.Name, StringComparer.Ordinal);
            var actualProperties = actual.EnumerateObject().ToDictionary(p => p.Name, StringComparer.Ordinal);
            foreach (var name in expectedProperties.Keys.Except(actualProperties.Keys, StringComparer.Ordinal))
                errors.Add($"{path}.{name}: missing property.");
            foreach (var name in actualProperties.Keys.Except(expectedProperties.Keys, StringComparer.Ordinal))
                errors.Add($"{path}.{name}: extra property.");
            foreach (var name in expectedProperties.Keys.Intersect(actualProperties.Keys, StringComparer.Ordinal))
                Compare(expectedProperties[name].Value, actualProperties[name].Value, $"{path}.{name}", errors);
            return;
        }

        if (expected.ValueKind == JsonValueKind.Array)
        {
            var expectedItems = expected.EnumerateArray().ToArray();
            var actualItems = actual.EnumerateArray().ToArray();
            if (expectedItems.Length != actualItems.Length)
                errors.Add($"{path}: expected array length {expectedItems.Length}, actual {actualItems.Length}.");
            for (var i = 0; i < Math.Min(expectedItems.Length, actualItems.Length); i++)
                Compare(expectedItems[i], actualItems[i], $"{path}[{i}]", errors);
        }
    }
}
