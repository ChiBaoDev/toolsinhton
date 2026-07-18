namespace UITests.Logic.Localization;

internal static class InnoPlaceholderParser
{
    internal static IReadOnlyDictionary<string, int> Parse(string value)
    {
        var tokens = new Dictionary<string, int>(StringComparer.Ordinal);
        for (var index = 0; index < value.Length; index++)
        {
            string? token = null;
            if (value[index] == '%' && index + 1 < value.Length && value[index + 1] == '%')
            {
                index++;
                continue;
            }
            if (value[index] == '%' && index + 1 < value.Length && value[index + 1] is 'n' or '1' or '2')
            {
                token = value.Substring(index, 2);
                index++;
            }
            else if (value[index] == '{' && index + 1 < value.Length && value[index + 1] == '{')
            {
                index++;
                continue;
            }
            else if (value[index] == '{')
            {
                var closingBrace = value.IndexOf('}', index + 1);
                if (closingBrace > index + 1)
                {
                    token = value[index..(closingBrace + 1)];
                    index = closingBrace;
                }
            }

            if (token is not null)
                tokens[token] = tokens.GetValueOrDefault(token) + 1;
        }
        return tokens;
    }
}
