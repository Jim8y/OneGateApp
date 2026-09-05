using Neo;
using Neo.SmartContract.Native;
using Neo.Wallets;
using Neo.Wallets.NEP6;
using NeoOrder.OneGate.Models;
using System.Numerics;

namespace NeoOrder.OneGate.Data
{
    public class ApplicationDbContext { public Settings Settings { get; } = new(); }
    public class Settings
    {
        public Task<T?> GetAsync<T>(string key) => Task.FromResult(default(T));
        public Task PutAsync<T>(string key, T value) => Task.CompletedTask;
    }
}
namespace NeoOrder.OneGate.Resources
{
    static class EmbeddedResource
    {
        public static T LoadJson<T>(string name) => (T)(object)new List<TokenInfo>
        {
            new() { Hash = NativeContract.NEO.Hash, Name = "Neo", Symbol = "NEO", Decimals = 0 },
            new() { Hash = NativeContract.GAS.Hash, Name = "Gas", Symbol = "GAS", Decimals = 8 }
        };
    }
}
namespace NeoOrder.OneGate.Services.RPC
{
    public class RpcClient
    {
        public Task<BigInteger[]> BalanceOf(UInt160 account, UInt160[] assets) => Task.FromResult(assets.Select(_ => new BigInteger(1)).ToArray());
        public Task<BigInteger> BalanceOf(UInt160 asset, UInt160 account) => Task.FromResult(new BigInteger(1));
        public Task<TokenInfo> GetTokenInfo(UInt160 asset) => throw new NotSupportedException();
        public Task<NFT[]> GetNFTs(UInt160 account, UInt160[] assets) => Task.FromResult(Array.Empty<NFT>());
    }
}
public class TestWalletProvider : IWalletProvider
{
    readonly NEP6Wallet wallet = new(Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json"), "test-only", ProtocolSettings.Default, "price-tests");
    public TestWalletProvider() => wallet.CreateAccount(UInt160.Zero).IsDefault = true;
    public event EventHandler<Wallet>? WalletChanged { add { } remove { } }
    public Wallet GetWallet() => wallet;
}
