namespace UITests.Logic.Localization;

internal sealed record CompositePlaceholder(string Identifier, int? Alignment, string? Format);
internal sealed record CompositeFormatSignature(
    IReadOnlyList<CompositePlaceholder> Placeholders,
    int EscapedOpenBraceCount,
    int EscapedCloseBraceCount,
    IReadOnlyList<string> AssaOverrideTags);

internal static class CompositeFormatPlaceholderParser
{
    internal static CompositeFormatSignature Parse(string value)
    {
        var placeholders = new List<CompositePlaceholder>();
        var escapedOpenBraceCount = 0;
        var escapedCloseBraceCount = 0;
        var assaOverrideTags = new List<string>();

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

                if (position + 1 < value.Length && value[position + 1] == '\\')
                {
                    var closingBrace = value.IndexOf('}', position + 2);
                    if (closingBrace < 0)
                        throw new FormatException("Unterminated ASS override tag.");
                    var tag = value[(position + 1)..closingBrace];
                    if (!IsValidAssaOverrideTag(tag))
                        throw new FormatException("Invalid ASS override tag.");
                    assaOverrideTags.Add(tag);
                    position = closingBrace + 1;
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

        return new CompositeFormatSignature(placeholders, escapedOpenBraceCount, escapedCloseBraceCount, assaOverrideTags);
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

        if (!expectedSignature.AssaOverrideTags.SequenceEqual(actualSignature.AssaOverrideTags, StringComparer.Ordinal))
        {
            errors.Add("ASS override tags differ.");
        }

        return errors;
    }

    private static bool IsValidAssaOverrideTag(string tag)
    {
        var content = tag[1..];
        if (content.StartsWith("an", StringComparison.Ordinal))
            return content.Length == 3 && content[2] is >= '1' and <= '9';
        if (content.StartsWith("pos(", StringComparison.Ordinal) && content.EndsWith(")"))
        {
            var parts = content[4..^1].Split(',');
            return parts.Length == 2 && parts.All(IsSignedNumber);
        }
        return content is "N" or "u1" or "u0";
    }

    private static bool IsSignedNumber(string value) =>
        double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out _);

    private static CompositePlaceholder ParsePlaceholder(string value, ref int position)
    {
        position++;
        var identifier = ParseIdentifier(value, ref position);

        if (identifier == "language")
        {
            if (position >= value.Length || value[position] != '}')
            {
                throw new FormatException("The {language} placeholder must be the exact bare token.");
            }

            position++;
            return new CompositePlaceholder(identifier, null, null);
        }

        SkipWhiteSpace(value, ref position);

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
