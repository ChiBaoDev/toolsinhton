namespace UITests.Logic.Localization;

public class CompositeFormatPlaceholderParserTests
{
    [Theory]
    [InlineData("{0} {1}", "{1} {0}")]
    [InlineData("{{{0}}}", "{{{0}}}")]
    [InlineData("{0:0.00} / {0:0.00}", "{0:0.00} / {0:0.00}")]
    public void Compare_AcceptsSemanticMatch(string expected, string actual) =>
        Assert.Empty(CompositeFormatPlaceholderParser.Compare(expected, actual));

    [Theory]
    [InlineData("{0} {0}", "{0}")]
    [InlineData("{0,10}", "{0,-10}")]
    [InlineData("{0:0.00}", "{0:0}")]
    [InlineData("{{{0}}}", "{0}")]
    public void Compare_RejectsSemanticChange(string expected, string actual) =>
        Assert.NotEmpty(CompositeFormatPlaceholderParser.Compare(expected, actual));

    [Fact]
    public void Parse_RejectsMalformedFormat() =>
        Assert.Throws<FormatException>(() => CompositeFormatPlaceholderParser.Parse("Value {0"));
}
