using NeoOrder.OneGate.Controls.Views;
using NeoOrder.OneGate.Properties;
using NeoOrder.OneGate.Services;

namespace NeoOrder.OneGate.Pages;

public partial class ExportWalletPage : ContentPage
{
    readonly WalletAuthorizationService walletAuthorizationService;

    public string? ExportFormat { get; set { field = value; OnPropertyChanged(); } }

    public ExportWalletPage(WalletAuthorizationService walletAuthorizationService)
    {
        this.walletAuthorizationService = walletAuthorizationService;
        InitializeComponent();
    }

    async void OnSubmit(object sender, EventArgs e)
    {
        Submit submit = (Submit)sender;
        using (submit.EnterBusyState())
        {
            if (!await walletAuthorizationService.RequestAuthorizationAsync(this, Strings.ExportPrivateKey))
                return;
            switch (ExportFormat)
            {
                case "WIF":
                    await Shell.Current.GoToAsync("wif");
                    break;
                case "NEP-2":
                    await Shell.Current.GoToAsync("nep2");
                    break;
            }
        }
    }
}
