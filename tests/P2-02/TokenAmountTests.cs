using System.Globalization;
using System.Numerics;
using NeoOrder.OneGate.Services;
using Xunit;

public class TokenAmountTests
{
    [Theory]
    [InlineData("en-US", "1.5")]
    [InlineData("de-DE", "1,5")]
    [InlineData("fr-FR", "1,5")]
    public void All_balance_formats_and_parses_without_changing_chain_units(string locale, string expected)
    {
        CultureInfo culture = new(locale);
        BigInteger units = 150_000_000;
        string formatted = TokenAmount.Format(units, 8, culture);
        Assert.Equal(expected, formatted);
        Assert.True(TokenAmount.TryParse(formatted, 8, out var parsed, culture));
        Assert.Equal(units, parsed);
    }

    [Theory]
    [InlineData("en-US", "0.00000001")]
    [InlineData("de-DE", "0,00000001")]
    [InlineData("fr-FR", "0,00000001")]
    public void Minimum_unit_is_preserved(string locale, string text)
    {
        Assert.True(TokenAmount.TryParse(text, 8, out var units, new CultureInfo(locale)));
        Assert.Equal(BigInteger.One, units);
    }

    [Theory]
    [InlineData("en-US", "0.000000001")]
    [InlineData("de-DE", "0,000000001")]
    [InlineData("fr-FR", "1 000,5")]
    [InlineData("en-US", "1,000.5")]
    [InlineData("en-US", "-1")]
    [InlineData("en-US", "1e3")]
    [InlineData("de-DE", "1.234")]
    public void Ambiguous_grouped_negative_and_overprecision_amounts_are_rejected(string locale, string text)
    {
        Assert.False(TokenAmount.TryParse(text, 8, out _, new CultureInfo(locale)));
    }

    [Fact]
    public void Large_balance_does_not_round_or_overflow_decimal()
    {
        var units = BigInteger.Parse("12345678901234567890123456789012345678");
        string text = TokenAmount.Format(units, 8, CultureInfo.InvariantCulture);
        Assert.Equal("123456789012345678901234567890.12345678", text);
        Assert.True(TokenAmount.TryParse(text, 8, out var parsed, CultureInfo.InvariantCulture));
        Assert.Equal(units, parsed);
    }

    [Theory]
    [InlineData("0", 8, "0")]
    [InlineData("1.000", 0, "1")]
    [InlineData(".5", 8, "50000000")]
    public void Zero_trailing_zeroes_and_fraction_input_have_exact_values(string text, byte decimals, string expected)
    {
        Assert.True(TokenAmount.TryParse(text, decimals, out var units, CultureInfo.InvariantCulture));
        Assert.Equal(BigInteger.Parse(expected), units);
    }
}
