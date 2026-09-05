using System.Net;
using Neo;
using Neo.SmartContract.Native;
using NeoOrder.OneGate.Data;
using NeoOrder.OneGate.Models;
using NeoOrder.OneGate.Services;
using NeoOrder.OneGate.Services.RPC;
using Xunit;

public class TokenPricesTests
{
    sealed class Handler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => send(request, cancellationToken);
    }

    static AssetInfo Asset(string symbol, int balance = 2) => new()
    {
        Token = new TokenInfo { Hash = UInt160.Zero, Name = symbol, Symbol = symbol, Decimals = 0 },
        Balance = balance
    };

    [Fact]
    public async Task BalanceLoadsDoNotRequestPrices()
    {
        int requests = 0;
        using var http = new HttpClient(new Handler((_, _) => { requests++; throw new HttpRequestException("price offline"); })) { BaseAddress = new Uri("https://price.test") };
        var manager = new TokenManager(new ApplicationDbContext(), new TestWalletProvider(), http, new RpcClient());
        var assets = await manager.LoadAssetsAsync();
        var single = await manager.LoadAssetAsync(NativeContract.NEO.Hash);
        Assert.Equal(2, assets.Count);
        Assert.Equal(1, (int)single.Balance);
        Assert.Equal(0, requests);
    }

    [Theory]
    [InlineData("[]", null, null)]
    [InlineData("[{\"symbol\":\"NEOUSDT\",\"price\":3}]", 3, null)]
    [InlineData("[{\"symbol\":\"X\",\"price\":99},{\"symbol\":\"BOGUSUSDT\",\"price\":99},{\"symbol\":\"NEOUSDT\",\"price\":3},{\"symbol\":\"GASUSDT\",\"price\":1}]", 3, 1)]
    public async Task MissingAndUnexpectedTickersAreSafe(string json, int? neoPrice, int? gasPrice)
    {
        using var http = new HttpClient(new Handler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) }))) { BaseAddress = new Uri("https://price.test") };
        AssetInfo[] assets = [Asset("NEO"), Asset("GAS")];
        await TokenPrices.RefreshAsync(http, assets);
        Assert.Equal(neoPrice, assets[0].Token.Price);
        Assert.Equal(gasPrice, assets[1].Token.Price);
        var total = TokenPrices.Total(assets);
        Assert.Equal(neoPrice.HasValue ? 2m * (neoPrice.Value + (gasPrice ?? 0)) : null, total.Value);
        Assert.Equal(neoPrice.HasValue && !gasPrice.HasValue, total.IsPartial);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task FailureAndTimeoutLeaveUnknownValuation(bool timeout)
    {
        using var http = new HttpClient(new Handler((_, _) => Task.FromException<HttpResponseMessage>(timeout
            ? new TaskCanceledException("timeout") : new HttpRequestException("502")))) { BaseAddress = new Uri("https://price.test") };
        AssetInfo[] assets = [Asset("NEO")];
        await TokenPrices.RefreshAsync(http, assets);
        Assert.Null(TokenPrices.Total(assets).Value);
    }

    [Fact]
    public void EmptyBalancesHaveKnownZeroValue() => Assert.Equal(0m, TokenPrices.Total([Asset("NEO", 0)]).Value);

    [Fact]
    public async Task PriceRefreshNotifiesAlreadyVisibleAsset()
    {
        using var http = new HttpClient(new Handler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("[{\"symbol\":\"NEOUSDT\",\"price\":3}]") }))) { BaseAddress = new Uri("https://price.test") };
        AssetInfo asset = Asset("NEO");
        var changed = new List<string?>();
        asset.PropertyChanged += (_, e) => changed.Add(e.PropertyName);
        await TokenPrices.RefreshAsync(http, [asset]);
        Assert.Contains(nameof(AssetInfo.DisplayValuation), changed);
        Assert.Contains(nameof(AssetInfo.Token), changed);
    }

    [Fact]
    public async Task OlderRefreshDoesNotOverwriteNewerQuote()
    {
        var older = new TaskCompletionSource<HttpResponseMessage>();
        int requests = 0;
        using var http = new HttpClient(new Handler((_, _) => ++requests == 1 ? older.Task : Task.FromResult(
            new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("[{\"symbol\":\"NEOUSDT\",\"price\":3}]") }))) { BaseAddress = new Uri("https://price.test") };
        AssetInfo asset = Asset("NEO");
        Task first = TokenPrices.RefreshAsync(http, [asset]);
        await TokenPrices.RefreshAsync(http, [asset]);
        older.SetResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("[{\"symbol\":\"NEOUSDT\",\"price\":1}]") });
        await first;
        Assert.Equal(3m, asset.Token.Price);
    }
}
