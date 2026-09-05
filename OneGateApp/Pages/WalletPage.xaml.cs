using CommunityToolkit.Maui.Alerts;
using Neo;
using Neo.Wallets;
using NeoOrder.OneGate.Controls;
using NeoOrder.OneGate.Data;
using NeoOrder.OneGate.Models;
using NeoOrder.OneGate.Properties;
using NeoOrder.OneGate.Services;
using System.ComponentModel;

namespace NeoOrder.OneGate.Pages;

public partial class WalletPage : ContentPage
{
    readonly ApplicationDbContext dbContext;
    readonly TokenManager tokenManager;

    public LoadingService LoadingService { get; set { field = value; OnPropertyChanged(); } }
    public Wallet Wallet { get; set { field = value; OnPropertyChanged(); } }
    public bool ShowBalance { get; set { field = value; OnPropertyChanged(); } }
    public WalletAccount DefaultAccount => Wallet.GetDefaultAccount()!;
    public UInt160 ScriptHash => DefaultAccount.ScriptHash;
    public IReadOnlyList<AssetInfo>? Assets
    {
        get;
        set
        {
            if (field is not null)
                foreach (AssetInfo asset in field) asset.PropertyChanged -= OnAssetPriceChanged;
            field = value;
            if (field is not null)
                foreach (AssetInfo asset in field) asset.PropertyChanged += OnAssetPriceChanged;
            OnPropertyChanged();
        }
    }
    public IReadOnlyList<NFT>? NFTs { get; set { field = value; OnPropertyChanged(); } }
    public string TotalValuation { get; set { field = value; OnPropertyChanged(); } } = "N/A";

    public WalletPage(ApplicationDbContext dbContext, IWalletProvider walletProvider, TokenManager tokenManager)
    {
        this.LoadingService = new(RefreshWallet, LoadAssetsAsync, LoadNFTsAsync);
        this.dbContext = dbContext;
        this.tokenManager = tokenManager;
        Wallet = walletProvider.GetWallet()!;
        ShowBalance = dbContext.Settings.Get<bool>("wallet/showBalance");
        InitializeComponent();
        LoadingService.BeginLoad();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (this.ShouldRefresh())
            LoadingService.BeginLoad();
    }

    async void OnToggleShowBalance(object sender, EventArgs e)
    {
        ShowBalance = !ShowBalance;
        await dbContext.Settings.PutAsync("wallet/showBalance", ShowBalance);
    }

    Task RefreshWallet()
    {
        OnPropertyChanged(nameof(Wallet));
        return Task.CompletedTask;
    }

    async Task LoadAssetsAsync()
    {
        IReadOnlyList<AssetInfo> assets = await tokenManager.LoadAssetsAsync();
        Assets = assets;
        UpdateTotalValuation(assets);
        await tokenManager.RefreshPricesAsync(assets);
        if (ReferenceEquals(Assets, assets)) UpdateTotalValuation(assets);
    }

    void UpdateTotalValuation(IReadOnlyList<AssetInfo> assets)
    {
        var total = TokenPrices.Total(assets);
        TotalValuation = total.Value is null ? Strings.Unavailable
            : total.IsPartial ? string.Format(Strings.PartialAssetValuation, total.Value.Value)
            : $"$ {total.Value.Value:N2}";
    }

    void OnAssetPriceChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AssetInfo.Token) && Assets is not null)
            UpdateTotalValuation(Assets);
    }

    async Task LoadNFTsAsync()
    {
        NFTs = await tokenManager.LoadNFTsAsync();
    }

    async void OnSendClicked(object sender, EventArgs e)
    {
        if (Assets is null)
        {
            LoadingService.BeginLoad();
            await Toast.Show(Strings.LoadingWalletData + "…");
            return;
        }
        await Shell.Current.GoToAsync("//wallet/send", new Dictionary<string, object>
        {
            ["assets"] = Assets
        });
    }

    async void OnAssetTapped(object sender, TappedEventArgs e)
    {
        AssetInfo asset = (AssetInfo)e.Parameter!;
        await Shell.Current.GoToAsync("//wallet/asset/details", new Dictionary<string, object> { ["asset"] = asset });
    }

    async void OnNFTTapped(object sender, TappedEventArgs e)
    {
        NFT nft = (NFT)e.Parameter!;
        await Shell.Current.GoToAsync("//wallet/nft/details", new Dictionary<string, object> { ["nft"] = nft });
    }
}
