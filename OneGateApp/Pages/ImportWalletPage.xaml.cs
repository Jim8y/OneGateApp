using NeoOrder.OneGate.Models;
using NeoOrder.OneGate.Services;

namespace NeoOrder.OneGate.Pages;

public partial class ImportWalletPage : ContentPage
{
    readonly IServiceProvider serviceProvider;

    public WalletCreationContext CreationContext { get; set { field = value; OnPropertyChanged(); } }

    public ImportWalletPage(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        this.CreationContext = new WalletCreationContext();
        InitializeComponent();
    }

    async void OnSubmitted(object sender, EventArgs e)
    {
        SelectImportTypePage page = serviceProvider.GetServiceOrCreateInstance<SelectImportTypePage>();
        page.CreationContext = CreationContext;
        await Navigation.PushAsync(page);
    }
}
