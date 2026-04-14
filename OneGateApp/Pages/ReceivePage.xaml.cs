using CommunityToolkit.Maui.Alerts;
using Neo;
using Neo.Wallets;
using NeoOrder.OneGate.Controls;
using NeoOrder.OneGate.Properties;
using NeoOrder.OneGate.Services;

namespace NeoOrder.OneGate.Pages;

public partial class ReceivePage : ContentPage, IQueryAttributable
{
    public Wallet Wallet { get; set { field = value; OnPropertyChanged(); } }
    public WalletAccount DefaultAccount => Wallet.GetDefaultAccount()!;
    public UInt160? Asset { get; set { field = value; OnPropertyChanged(); OnPropertyChanged(nameof(AddressUri)); } }
    public string AddressUri
    {
        get
        {
            string uri = $"neo:{DefaultAccount.Address}";
            if (Asset is not null)
                uri += $"?asset={Asset}";
            return uri;
        }
    }

    public ReceivePage(IWalletProvider walletProvider)
    {
        Wallet = walletProvider.GetWallet()!;
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("asset", out var asset))
            Asset = (string)asset;
    }

    async void OnSaveToPhotoLibrary(object sender, EventArgs e)
    {
        string fileName = $"{DateTime.Now:yyyy-MM-dd HHmmss}.png";
        var result = await qrCodeCard.CaptureAsync();
        if (result is null)
            await Toast.Show(Strings.ScreenshotFailed);
        else
            await PhotoLibraryService.SaveAsync(result, fileName);
    }
}
