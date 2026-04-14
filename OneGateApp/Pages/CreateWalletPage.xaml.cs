using NeoOrder.OneGate.Models;
using NeoOrder.OneGate.Services;

namespace NeoOrder.OneGate.Pages;

public partial class CreateWalletPage : ContentPage
{
    readonly IServiceProvider serviceProvider;

    public CreateWalletPage(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        this.serviceProvider = serviceProvider;
        BindingContext = new WalletCreationContext();
    }

    async void OnSubmitted(object sender, EventArgs e)
    {
        Page page = serviceProvider.GetServiceOrCreateInstance<GenerateMnemonicPage>();
        page.BindingContext = BindingContext;
        await Navigation.PushAsync(page);
    }
}
