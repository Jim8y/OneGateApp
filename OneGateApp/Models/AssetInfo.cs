using Neo;
using System.Numerics;
using System.ComponentModel;

namespace NeoOrder.OneGate.Models;

public class AssetInfo : INotifyPropertyChanged
{
    readonly object priceSync = new();
    int priceVersion;
    public event PropertyChangedEventHandler? PropertyChanged;
    internal int BeginPriceRefresh()
    {
        lock (priceSync) return ++priceVersion;
    }
    internal void SetPrice(decimal? price, int version)
    {
        lock (priceSync)
        {
            if (version != priceVersion) return;
            Token.Price = price;
        }
        PropertyChanged?.Invoke(this, new(nameof(Token)));
        PropertyChanged?.Invoke(this, new(nameof(Valuation)));
        PropertyChanged?.Invoke(this, new(nameof(DisplayValuation)));
    }
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
