using Neo.Wallets;
using NeoOrder.OneGate.Models;
using NeoOrder.OneGate.Properties;
using NeoOrder.OneGate.Services;

namespace NeoOrder.OneGate.Pages;

public partial class WalletDetailsPage : ContentPage
{
    readonly IWalletProvider walletProvider;

    public LoadingService LoadingService { get; set { field = value; OnPropertyChanged(); } }
    public SettingEntryGroup[]? SettingEntries { get; set { field = value; OnPropertyChanged(); } }

    public WalletDetailsPage(IWalletProvider walletProvider)
    {
        this.walletProvider = walletProvider;
        this.LoadingService = new(LoadSettingsAsync);
        InitializeComponent();
        LoadingService.BeginLoad();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (this.ShouldRefresh())
            LoadingService.BeginLoad();
    }

    async Task LoadSettingsAsync()
    {
        SettingEntries = await GetAllSettingEntries()
            .GroupBy(p => p.GroupName, p => p.Entry)
            .Select(p => SettingEntryGroup.Create(p.Key, p))
            .ToArrayAsync();
    }

    async IAsyncEnumerable<(string GroupName, SettingEntry Entry)> GetAllSettingEntries()
    {
        yield return (Strings.General, new SettingEntry(Strings.WalletName)
        {
            CurrentValue = walletProvider.GetWallet()!.Name,
            Command = Commands.GotoPage,
            CommandParameter = "name"
        });
        yield return (Strings.Security, new SettingEntry(Strings.ChangePassword)
        {
            Command = Commands.GotoPage,
            CommandParameter = "password"
        });
        yield return (Strings.Security, new SettingEntry(Strings.ExportPrivateKey)
        {
            IsDanger = true,
            Command = Commands.GotoPage,
            CommandParameter = "export"
        });
        yield return (Strings.Security, new SettingEntry(Strings.DeleteWallet)
        {
            IsDanger = true,
            Command = Commands.GotoPage,
            CommandParameter = "delete"
        });
    }
}
