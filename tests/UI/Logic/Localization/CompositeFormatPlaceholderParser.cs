namespace UITests.Logic.Localization;

internal sealed record CompositePlaceholder(string Identifier, int? Alignment, string? Format);
internal sealed record CompositeFormatSignature(
    IReadOnlyList<CompositePlaceholder> Placeholders,
    int EscapedOpenBraceCount,
    int EscapedCloseBraceCount);

internal static class CompositeFormatPlaceholderParser
{
    internal static CompositeFormatSignature Parse(string value)
    {
        var placeholders = new List<CompositePlaceholder>();
        var escapedOpenBraceCount = 0;
        var escapedCloseBraceCount = 0;

        for (var position = 0; position < value.Length;)
        {
            if (value[position] == '{')
            {
                if (position + 1 < value.Length && value[position + 1] == '{')
                {
                    escapedOpenBraceCount++;
                    position += 2;
                    continue;
                }

                placeholders.Add(ParsePlaceholder(value, ref position));
                continue;
            }

            if (value[position] == '}')
            {
                if (position + 1 < value.Length && value[position + 1] == '}')
                {
                    escapedCloseBraceCount++;
                    position += 2;
                    continue;
                }

                throw new FormatException($"Unexpected closing brace at position {position}.");
            }

            position++;
        }

        return new CompositeFormatSignature(placeholders, escapedOpenBraceCount, escapedCloseBraceCount);
    }

    internal static IReadOnlyList<string> Compare(string expected, string actual)
    {
        var expectedSignature = Parse(expected);
        var actualSignature = Parse(actual);
        var errors = new List<string>();

        var expectedPlaceholders = Sort(expectedSignature.Placeholders);
        var actualPlaceholders = Sort(actualSignature.Placeholders);
        if (!expectedPlaceholders.SequenceEqual(actualPlaceholders))
        {
            errors.Add("Composite placeholders differ.");
        }

        if (expectedSignature.EscapedOpenBraceCount != actualSignature.EscapedOpenBraceCount)
        {
            errors.Add($"Expected {expectedSignature.EscapedOpenBraceCount} escaped opening braces, actual {actualSignature.EscapedOpenBraceCount}.");
        }

        if (expectedSignature.EscapedCloseBraceCount != actualSignature.EscapedCloseBraceCount)
        {
            errors.Add($"Expected {expectedSignature.EscapedCloseBraceCount} escaped closing braces, actual {actualSignature.EscapedCloseBraceCount}.");
        }

        return errors;
    }

    private static CompositePlaceholder ParsePlaceholder(string value, ref int position)
    {
        position++;
        var identifier = ParseIdentifier(value, ref position);
        SkipWhiteSpace(value, ref position);

        if (identifier == "language" && position < value.Length && value[position] != '}')
        {
            throw new FormatException("The {language} placeholder does not support alignment or format syntax.");
        }

        int? alignment = null;
        if (position < value.Length && value[position] == ',')
        {
            position++;
            SkipWhiteSpace(value, ref position);
            var sign = 1;
            if (position < value.Length && (value[position] == '-' || value[position] == '+'))
            {
                if (value[position] == '-')
                {
                    sign = -1;
                }
                position++;
            }
            alignment = sign * ParseUnsignedInteger(value, ref position, "alignment");
            SkipWhiteSpace(value, ref position);
        }

        string? format = null;
        if (position < value.Length && value[position] == ':')
        {
            var formatStart = ++position;
            while (position < value.Length && value[position] != '}')
            {
                if (value[position] == '{')
                {
                    throw new FormatException($"Unexpected opening brace at position {position}.");
                }
                position++;
            }
            format = value[formatStart..position];
        }

        if (position >= value.Length || value[position] != '}')
        {
            throw new FormatException("Input string was not in a correct composite format.");
        }

        position++;
        return new CompositePlaceholder(identifier, alignment, format);
    }

    private static string ParseIdentifier(string value, ref int position)
    {
        var start = position;
        if (position < value.Length && char.IsAsciiDigit(value[position]))
        {
            var index = ParseUnsignedInteger(value, ref position, "placeholder index");
            return index.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        const string languageToken = "language";
        if (value.AsSpan(position).StartsWith(languageToken, StringComparison.Ordinal))
        {
            position += languageToken.Length;
            return languageToken;
        }

        throw new FormatException($"Invalid placeholder identifier at position {start}.");
    }

    private static int ParseUnsignedInteger(string value, ref int position, string component)
    {
        var start = position;
        while (position < value.Length && char.IsAsciiDigit(value[position]))
        {
            position++;
        }

        if (position == start || !int.TryParse(value.AsSpan(start, position - start), out var result))
        {
            throw new FormatException($"Invalid {component} at position {start}.");
        }

        return result;
    }

    private static void SkipWhiteSpace(string value, ref int position)
    {
        while (position < value.Length && char.IsWhiteSpace(value[position]))
        {
            position++;
        }
    }

    private static IReadOnlyList<CompositePlaceholder> Sort(IEnumerable<CompositePlaceholder> placeholders) =>
        placeholders
            .OrderBy(p => p.Identifier, StringComparer.Ordinal)
            .ThenBy(p => p.Alignment)
            .ThenBy(p => p.Format, StringComparer.Ordinal)
            .ToArray();
}
