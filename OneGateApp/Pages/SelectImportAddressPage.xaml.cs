using Neo.Wallets;
using NeoOrder.OneGate.Models;
using NeoOrder.OneGate.Services;

namespace NeoOrder.OneGate.Pages;

public partial class SelectImportAddressPage : ContentPage
{
    readonly IServiceProvider serviceProvider;

    public required WalletCreationContext CreationContext { get; set { field = value; OnPropertyChanged(); } }
    public KeyPair? SelectedKey { get; set { field = value; OnPropertyChanged(); } }

    public SelectImportAddressPage(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        InitializeComponent();
    }

    async void OnSubmit(object sender, EventArgs e)
    {
        CreationContext.PrivateKey = SelectedKey!.PrivateKey;
        Page page = serviceProvider.GetServiceOrCreateInstance<CreatePasswordPage>();
        page.BindingContext = CreationContext;
        await Navigation.PushAsync(page);
    }
}
