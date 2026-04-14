using Neo;
using System.Numerics;

namespace NeoOrder.OneGate.Models;

public class AssetInfo
{
    public required TokenInfo Token { get; init; }
    public required BigInteger Balance { get; init; }

    public BigDecimal DecimalBalance => new(Balance, Token.Decimals);
    public string DisplayBalance => $"{DecimalBalance} {Token.Symbol}";
    public decimal? Valuation
    {
        get
        {
            if (!Token.Price.HasValue) return null;
            return (decimal)Balance / (decimal)BigInteger.Pow(10, Token.Decimals) * Token.Price;
        }
    }
    public string DisplayValuation => Token.Price.HasValue ? $"≈ $ {Valuation:N2}" : "N/A";
}
