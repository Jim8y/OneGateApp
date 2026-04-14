using CommunityToolkit.Maui.Alerts;
using NeoOrder.OneGate.Controls;
using NeoOrder.OneGate.Models;
using NeoOrder.OneGate.Properties;
using NeoOrder.OneGate.Services;

namespace NeoOrder.OneGate.Pages;

public partial class NFTDetailsPage : ContentPage, IQueryAttributable
{
    readonly TokenManager tokenManager;

    public required NFT NFT { get; set { field = value; OnPropertyChanged(); } }

    public NFTDetailsPage(TokenManager tokenManager)
    {
        this.tokenManager = tokenManager;
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        NFT = (NFT)query["nft"];
    }

    async void OnHideClicked(object sender, EventArgs e)
    {
        await tokenManager.HideTokenAsync(NFT.TokenInfo!.Hash);
        GlobalStates.Invalidate<WalletPage>();
        await Shell.Current.GoToAsync("..");
        await Toast.Show(Strings.AssetHidden);
    }

    async void OnSendClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//wallet/nft/details/send", new Dictionary<string, object> { ["nft"] = NFT });
    }
}
