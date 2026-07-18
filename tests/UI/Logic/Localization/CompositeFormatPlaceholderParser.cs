namespace UITests.Logic.Localization;

internal sealed record CompositePlaceholder(string Identifier, int? Alignment, string? Format);
internal sealed record CompositeFormatSignature(
    IReadOnlyList<CompositePlaceholder> Placeholders,
    int EscapedOpenBraceCount,
    int EscapedCloseBraceCount,
    IReadOnlyList<string> AssaOverrideTags,
    IReadOnlyList<string> StructuralTokens);

internal static class CompositeFormatPlaceholderParser
{
    internal static CompositeFormatSignature Parse(string value)
    {
        var placeholders = new List<CompositePlaceholder>();
        var escapedOpenBraceCount = 0;
        var escapedCloseBraceCount = 0;
        var assaOverrideTags = new List<string>();
        var structuralTokens = new List<string>();

        for (var position = 0; position < value.Length;)
        {
            if (value[position] == '{')
            {
                if (position + 1 < value.Length && value[position + 1] == '{')
                {
                    escapedOpenBraceCount++;
                    structuralTokens.Add("E:{");
                    position += 2;
                    continue;
                }

                if (position + 1 < value.Length && value[position + 1] == '\\')
                {
                    var closingBrace = value.IndexOf('}', position + 2);
                    if (closingBrace < 0)
                        throw new FormatException("Unterminated ASS override tag.");
                    var tag = value[(position + 1)..closingBrace];
                    foreach (var atom in ParseAssaBlock(tag))
                    {
                        assaOverrideTags.Add(atom);
                        structuralTokens.Add("T:" + atom);
                    }
                    position = closingBrace + 1;
                    continue;
                }

                placeholders.Add(ParsePlaceholder(value, ref position));
                structuralTokens.Add("P");
                continue;
            }

            if (value[position] == '}')
            {
                if (position + 1 < value.Length && value[position + 1] == '}')
                {
                    escapedCloseBraceCount++;
                    structuralTokens.Add("E:}");
                    position += 2;
                    continue;
                }

                throw new FormatException($"Unexpected closing brace at position {position}.");
            }

            var textStart = position;
            while (position < value.Length && value[position] is not ('{' or '}'))
                position++;
            structuralTokens.Add("X");
        }

        return new CompositeFormatSignature(placeholders, escapedOpenBraceCount, escapedCloseBraceCount, assaOverrideTags, structuralTokens);
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

        if (!expectedSignature.AssaOverrideTags.SequenceEqual(actualSignature.AssaOverrideTags, StringComparer.Ordinal) ||
            (expectedSignature.AssaOverrideTags.Count > 0 &&
             !expectedSignature.StructuralTokens.SequenceEqual(actualSignature.StructuralTokens, StringComparer.Ordinal)))
        {
            errors.Add("ASS override tags or positions differ.");
        }

        return errors;
    }

    private static IReadOnlyList<string> ParseAssaBlock(string block)
    {
        var atoms = new List<string>();
        for (var p = 0; p < block.Length;)
        {
            if (block[p++] != '\\') throw new FormatException("Invalid ASS override tag.");
            var start = p - 1;
            while (p < block.Length && char.IsLetter(block[p])) p++;
            var name = block[start..p];
            if (name is "\\N" or "\\n") { atoms.Add(name); continue; }
            if (name == "\\u")
            {
                if (p >= block.Length || block[p] is not ('0' or '1')) throw new FormatException("Invalid ASS underline tag.");
                atoms.Add(name + block[p++]); continue;
            }
            if (name == "\\an")
            {
                if (p >= block.Length || block[p] is < '1' or > '9') throw new FormatException("Invalid ASS alignment tag.");
                atoms.Add(name + block[p++]); continue;
            }
            if (name == "\\pos")
            {
                if (p >= block.Length || block[p++] != '(') throw new FormatException("Invalid ASS position tag.");
                var x = ReadAssNumber(block, ref p);
                if (p >= block.Length || block[p++] != ',') throw new FormatException("Invalid ASS position tag.");
                var y = ReadAssNumber(block, ref p);
                if (p >= block.Length || block[p++] != ')') throw new FormatException("Invalid ASS position tag.");
                atoms.Add(name + "(" + x + "," + y + ")"); continue;
            }
            if (name == "\\fsp") { atoms.Add(name + ReadAssNumber(block, ref p)); continue; }
            throw new FormatException("Invalid ASS override tag.");
        }
        return atoms;
    }

    private static string ReadAssNumber(string value, ref int position)
    {
        var start = position;
        if (position < value.Length && (value[position] == '+' || value[position] == '-')) position++;
        var hasDigits = false;
        while (position < value.Length && char.IsAsciiDigit(value[position])) { hasDigits = true; position++; }
        if (position < value.Length && value[position] == '.') { position++; while (position < value.Length && char.IsAsciiDigit(value[position])) { hasDigits = true; position++; } }
        if (!hasDigits) throw new FormatException("Invalid ASS numeric value.");
        var text = value[start..position];
        if (!double.TryParse(text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var number) || !double.IsFinite(number)) throw new FormatException("Invalid ASS numeric value.");
        return text;
    }

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
