using Neo.Wallets;
using NeoOrder.OneGate.Services;

namespace NeoOrder.OneGate.Pages;

public partial class ChangeWalletNamePage : ContentPage
{
    public Wallet Wallet { get; set { field = value; OnPropertyChanged(); } }

    public ChangeWalletNamePage(IWalletProvider walletProvider)
    {
        Wallet = walletProvider.GetWallet()!;
        InitializeComponent();
    }

    async void OnSubmitted(object sender, EventArgs e)
    {
        Wallet.Save();
        GlobalStates.Invalidate<WalletPage>();
        GlobalStates.Invalidate<WalletDetailsPage>();
        await Shell.Current.GoToAsync("..");
    }
}
