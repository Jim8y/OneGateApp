using NeoOrder.OneGate.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace NeoOrder.OneGate.Services;

internal static class TokenPrices
{
    public static async Task RefreshAsync(HttpClient http, IReadOnlyList<AssetInfo> assets)
    {
        if (assets.Count == 0) return;
        var updates = assets.Select(asset => (Asset: asset, Version: asset.BeginPriceRefresh())).ToArray();
        var prices = new Dictionary<string, decimal>(StringComparer.Ordinal);
        try
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            string query = string.Join('&', assets.Select(p => $"symbol={Uri.EscapeDataString(p.Token.Symbol + "USDT")}").Distinct());
            Ticker[] tickers = await http.GetFromJsonAsync<Ticker[]>($"/api/ticker/price?{query}", timeout.Token) ?? [];
            foreach (Ticker ticker in tickers)
                if (ticker is not null && !string.IsNullOrEmpty(ticker.Symbol) && ticker.Price >= 0)
                    prices.TryAdd(ticker.Symbol, ticker.Price);
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException or OperationCanceledException)
        {
            // Prices are optional; chain balances and sending must remain usable.
        }
        foreach (var update in updates)
            update.Asset.SetPrice(prices.TryGetValue(update.Asset.Token.Symbol + "USDT", out decimal price) ? price : null, update.Version);
    }

    public static (decimal? Value, bool IsPartial) Total(IReadOnlyList<AssetInfo> assets)
    {
        AssetInfo[] held = assets.Where(p => p.Balance > 0).ToArray();
        if (held.Length == 0) return (0m, false);
        decimal[] values = held.Where(p => p.Valuation.HasValue).Select(p => p.Valuation!.Value).ToArray();
        return values.Length == 0 ? (null, false) : (values.Sum(), values.Length != held.Length);
    }
}
