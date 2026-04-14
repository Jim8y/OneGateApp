using CommunityToolkit.Maui.Alerts;
using Neo;
using Neo.Wallets;
using NeoOrder.OneGate.Controls;
using NeoOrder.OneGate.Models;
using NeoOrder.OneGate.Properties;
using NeoOrder.OneGate.Services;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace NeoOrder.OneGate.Pages;

public partial class AuthenticatePage : ContentPage, IQueryAttributable
{
    readonly ProtocolSettings protocolSettings;
    readonly WalletAuthorizationService walletAuthorizationService;
    readonly Wallet wallet;
    readonly HttpClient httpClient;

    public string? DAppIdentifier { get; set { field = value; OnPropertyChanged(); } }
    public required AuthenticationChallengePayload Payload { get; set { field = value; OnPropertyChanged(); } }

    public AuthenticatePage(ProtocolSettings protocolSettings, WalletAuthorizationService walletAuthorizationService, IWalletProvider walletProvider, HttpClient httpClient)
    {
        this.protocolSettings = protocolSettings;
        this.walletAuthorizationService = walletAuthorizationService;
        this.wallet = walletProvider.GetWallet()!;
        this.httpClient = httpClient;
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("dapp", out var obj_dapp))
            DAppIdentifier = (string)obj_dapp;
        Payload = (AuthenticationChallengePayload)query["payload"];
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Authenticate();
    }

    async void Authenticate()
    {
        try
        {
            AuthenticationResponsePayload response = await AuthenticateAsync(Payload);
            if (DAppIdentifier is null)
            {
                if (Payload.Callback is null || !Payload.Callback.IsAbsoluteUri || Payload.Callback.Scheme != "https" || Payload.Callback.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException(Strings.InvalidCallbackURLFormat);
                var message = await httpClient.PostAsJsonAsync(Payload.Callback, response, SharedOptions.JsonSerializerOptions);
                message.EnsureSuccessStatusCode();
            }
            else
            {
                string result = WebUtility.UrlEncode(JsonSerializer.Serialize(response, SharedOptions.JsonSerializerOptions));
                string uri = $"dapp://{DAppIdentifier}/auth?result={result}";
                if (!await Launcher.TryOpenAsync(uri))
                    await Toast.Show(Strings.OpenDAppFailedText);
            }
        }
        catch (Exception ex)
        {
            if (DAppIdentifier is null)
            {
                await Toast.Show(ex.Message);
            }
            else
            {
                var error = new JsonObject
                {
                    ["code"] = ex.HResult,
                    ["message"] = ex.Message
                };
                string text = WebUtility.UrlEncode(JsonSerializer.Serialize(error, SharedOptions.JsonSerializerOptions));
                string uri = $"dapp://{DAppIdentifier}/auth?error={text}";
                if (!await Launcher.TryOpenAsync(uri))
                    await Toast.Show(ex.Message);
            }
        }
        await this.GoBackOrCloseAsync();
    }

    async Task<AuthenticationResponsePayload> AuthenticateAsync(AuthenticationChallengePayload payload)
    {
        payload.Validate(protocolSettings);
        if (!await walletAuthorizationService.RequestAuthorizationAsync(this, Strings.LoginRequest, Strings.LoginRequestText, payload.Domain))
            throw new OperationCanceledException();
        WalletAccount account = wallet.GetDefaultAccount()!;
        return payload.CreateResponse(account, protocolSettings);
    }
}
