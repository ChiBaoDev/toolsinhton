namespace UITests.Logic.Localization;

public class CompositeFormatPlaceholderParserTests
{
    [Theory]
    [InlineData("{0} {1}", "{1} {0}")]
    [InlineData("{00}", "{0}")]
    [InlineData("{{{0}}}", "{{{0}}}")]
    [InlineData("{0:0.00} / {0:0.00}", "{0:0.00} / {0:0.00}")]
    [InlineData("Language: {language}", "Ngôn ngữ: {language}")]
    public void Compare_AcceptsSemanticMatch(string expected, string actual) =>
        Assert.Empty(CompositeFormatPlaceholderParser.Compare(expected, actual));

    [Theory]
    [InlineData("{0foo}")]
    [InlineData("{foo}")]
    [InlineData("{Language}")]
    [InlineData("{language,10}")]
    [InlineData("{language:x}")]
    [InlineData("{language }")]
    [InlineData("{language\t}")]
    [InlineData("{language\n}")]
    [InlineData("{999999999999999999999999999999999999999999}")]
    public void Parse_RejectsUnsupportedIdentifiersAndNamedTokenSyntax(string value) =>
        Assert.Throws<FormatException>(() => CompositeFormatPlaceholderParser.Parse(value));

    [Theory]
    [InlineData("{0} {0}", "{0}")]
    [InlineData("{0,10}", "{0,-10}")]
    [InlineData("{0:0.00}", "{0:0}")]
    [InlineData("{{{0}}}", "{0}")]
    public void Compare_RejectsSemanticChange(string expected, string actual) =>
        Assert.NotEmpty(CompositeFormatPlaceholderParser.Compare(expected, actual));

    [Fact]
    public void Compare_PreservesValidAssaOverrideTagSequenceAndContent()
    {
        Assert.Empty(CompositeFormatPlaceholderParser.Compare(@"{\an8}Hello {\pos(10,20)}", @"{\an8}Xin chào {\pos(10,20)}"));
        Assert.NotEmpty(CompositeFormatPlaceholderParser.Compare(@"{\an8}Hello", @"{\an7}Xin chào"));
        Assert.NotEmpty(CompositeFormatPlaceholderParser.Compare(@"{\an8}Hello", "Hello"));
    }

    [Theory]
    [InlineData(@"{\arbitrary}")]
    [InlineData(@"{\pos(10,)}")]
    [InlineData(@"{\an}")]
    [InlineData(@"{\pos(10,20)")]
    public void Parse_RejectsMalformedOrUnsupportedAssaOverrideTags(string value) =>
        Assert.Throws<FormatException>(() => CompositeFormatPlaceholderParser.Parse(value));

    [Fact]
    public void Compare_PreservesMixedLanguageAndAssaTags()
    {
        Assert.Empty(CompositeFormatPlaceholderParser.Compare(@"{language} {\an8}", @"{language} {\an8}"));
        Assert.NotEmpty(CompositeFormatPlaceholderParser.Compare(@"{language} {\an8}", @"{language} {\an7}"));
    }

    [Fact]
    public void Parse_RejectsMalformedFormat() =>
        Assert.Throws<FormatException>(() => CompositeFormatPlaceholderParser.Parse("Value {0"));
}
