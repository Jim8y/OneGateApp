using CommunityToolkit.Maui.Alerts;
using Neo.SmartContract.Native;
using NeoOrder.OneGate.Controls;
using NeoOrder.OneGate.Data;
using NeoOrder.OneGate.Models;
using NeoOrder.OneGate.Properties;
using NeoOrder.OneGate.Services;

namespace NeoOrder.OneGate.Pages;

public partial class AssetDetailsPage : ContentPage, IQueryAttributable
{
    readonly TokenManager tokenManager;

    public required AssetInfo Asset { get; set { field = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanHide)); } }
    public bool CanHide => Asset?.Token.Hash != NativeContract.NEO.Hash && Asset?.Token.Hash != NativeContract.GAS.Hash;
    public bool ShowBalance { get; set { field = value; OnPropertyChanged(); } }

    public AssetDetailsPage(ApplicationDbContext dbContext, TokenManager tokenManager)
    {
        this.tokenManager = tokenManager;
        ShowBalance = dbContext.Settings.Get<bool>("wallet/showBalance");
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Asset = (AssetInfo)query["asset"]!;
    }

    async void OnHideClicked(object sender, EventArgs e)
    {
        await tokenManager.HideTokenAsync(Asset.Token.Hash);
        GlobalStates.Invalidate<WalletPage>();
        await Shell.Current.GoToAsync("..");
        await Toast.Show(Strings.AssetHidden);
    }

    async void OnSendClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//wallet/asset/details/send", new Dictionary<string, object>
        {
            ["asset"] = Asset
        });
    }
}
