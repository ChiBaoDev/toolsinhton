namespace UITests.Logic.Localization;

public class InnoPlaceholderParserTests
{
    [Fact]
    public void Parse_CountsSupportedTokensWithMultiplicity()
    {
        var signature = InnoPlaceholderParser.Parse("%n%1 {app} %n %2 {app} %1");

        Assert.Equal(2, signature["%n"]);
        Assert.Equal(2, signature["%1"]);
        Assert.Equal(1, signature["%2"]);
        Assert.Equal(2, signature["{app}"]);
        Assert.Equal(4, signature.Count);
    }

    [Fact]
    public void Parse_RecognizesDifferentInnoConstantsSeparately()
    {
        var signature = InnoPlaceholderParser.Parse("{app} {tmp} {app}");

        Assert.Equal(2, signature["{app}"]);
        Assert.Equal(1, signature["{tmp}"]);
        Assert.Equal(2, signature.Count);
    }

    [Theory]
    [InlineData("First%n%nSecond %1", "Thứ nhất%n%nThứ hai %1")]
    [InlineData("Copy %1 to {app}, then use %2", "Sao chép %1 vào {app}, rồi dùng %2")]
    public void Parse_ProducesMatchingSignaturesForLocalizedText(string english, string localized)
    {
        Assert.Equal(
            InnoPlaceholderParser.Parse(english).OrderBy(pair => pair.Key, StringComparer.Ordinal),
            InnoPlaceholderParser.Parse(localized).OrderBy(pair => pair.Key, StringComparer.Ordinal));
    }

    [Fact]
    public void Parse_IgnoresEscapedPercentAndOpeningBrace()
    {
        var signature = InnoPlaceholderParser.Parse("%%n %%1 {{app} %n %1 {app}");

        Assert.Equal(1, signature["%n"]);
        Assert.Equal(1, signature["%1"]);
        Assert.Equal(1, signature["{app}"]);
        Assert.Equal(3, signature.Count);
    }

    [Fact]
    public void Parse_ProducesDifferentSignaturesWhenMultiplicityChanges()
    {
        Assert.NotEqual(
            InnoPlaceholderParser.Parse("%n%n%1").OrderBy(pair => pair.Key, StringComparer.Ordinal),
            InnoPlaceholderParser.Parse("%n%1").OrderBy(pair => pair.Key, StringComparer.Ordinal));
    }
}
