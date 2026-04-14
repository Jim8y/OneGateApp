using NeoOrder.OneGate.Services;

namespace NeoOrder.OneGate.Pages;

public partial class WelcomePage : ContentPage
{
    readonly IServiceProvider serviceProvider;

    public WelcomePage(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        InitializeComponent();
    }

    async void OnCreateWallet(object sender, EventArgs e)
    {
        Page page = serviceProvider.GetServiceOrCreateInstance<CreateWalletPage>();
        await Navigation.PushAsync(page);
    }

    async void OnImportWallet(object sender, EventArgs e)
    {
        Page page = serviceProvider.GetServiceOrCreateInstance<ImportWalletPage>();
        await Navigation.PushAsync(page);
    }
}
