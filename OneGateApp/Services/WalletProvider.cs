using Neo;
using Neo.Wallets;

namespace NeoOrder.OneGate.Services;

class WalletProvider(ProtocolSettings protocolSettings) : IWalletProvider
{
    public event EventHandler<Wallet?>? WalletChanged;

    Wallet? wallet;

    public Wallet? GetWallet()
    {
        if (wallet is null && File.Exists(SharedOptions.WalletPath))
        {
            wallet = Wallet.Open(SharedOptions.WalletPath, null, protocolSettings)!;
            WalletChanged?.Invoke(this, wallet);
        }
        return wallet;
    }

    public void DeleteWallet()
    {
        File.Delete(SharedOptions.WalletPath);
        wallet = null;
        WalletChanged?.Invoke(this, null);
    }
}
