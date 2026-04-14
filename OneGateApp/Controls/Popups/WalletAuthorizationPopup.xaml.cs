using Neo.Wallets;
using NeoOrder.OneGate.Controls.Views;
using NeoOrder.OneGate.Properties;

namespace NeoOrder.OneGate.Controls.Popups;

public partial class WalletAuthorizationPopup : MyPopup<bool>
{
    readonly IWalletProvider walletProvider;

    public required string Title { get; set { field = value; OnPropertyChanged(); } }
    public string? Message { get; set { field = value; OnPropertyChanged(); } }
    public string? Domain { get; set { field = value; OnPropertyChanged(); } }
    public string? Password { get; set; }
    public string AuthorizeText { get; set { field = value; OnPropertyChanged(); } } = Strings.Authorize;
    public string CancelText { get; set { field = value; OnPropertyChanged(); } } = Strings.Cancel;

    public WalletAuthorizationPopup(IWalletProvider walletProvider)
    {
        this.walletProvider = walletProvider;
        InitializeComponent();
    }

    async void OnAuthorize(object sender, EventArgs e)
    {
        Submit submit = (Submit)sender;
        using (submit.EnterBusyState())
        {
            Wallet wallet = walletProvider.GetWallet()!;
            bool isPasswordCorrect = await Task.Run(() => wallet.VerifyPassword(Password!));
            if (isPasswordCorrect)
                await CloseAsync(true);
            else
                errMsg.SetError(Strings.ErrorMessageIncorrectPassword);
        }
    }

    async void OnCancel(object sender, EventArgs e)
    {
        await CloseAsync(false);
    }
}
