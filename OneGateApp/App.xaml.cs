using Neo;
using Neo.Wallets;
using NeoOrder.OneGate.Data;
using NeoOrder.OneGate.Models.AppLinks;
using NeoOrder.OneGate.Pages;
using NeoOrder.OneGate.Services;
using System.Globalization;

namespace NeoOrder.OneGate;

public partial class App : Application
{
    readonly IServiceProvider serviceProvider;
    readonly ApplicationDbContext dbContext;
    readonly ProtocolSettings protocolSettings;
    readonly IWalletProvider walletProvider;
    readonly HttpClient httpClient;

    AppLinkAction? appLinkAction;

    public App(IServiceProvider serviceProvider, ApplicationDbContext dbContext, ProtocolSettings protocolSettings, IWalletProvider walletProvider, HttpClient httpClient)
    {
        this.serviceProvider = serviceProvider;
        this.dbContext = dbContext;
        this.protocolSettings = protocolSettings;
        this.walletProvider = walletProvider;
        this.httpClient = httpClient;
        dbContext.Database.EnsureCreated();
        Version? version = dbContext.Settings.Get<Version>("system/version");
        if (version is null || version < AppInfo.Version)
        {
            dbContext.Settings.Put("system/version", AppInfo.Version);
            dbContext.Settings.Delete("system/updates");
        }
        InitializeComponent();
    }

    internal bool ProcessAppLinkUri(Uri uri)
    {
        appLinkAction = uri.Scheme switch
        {
            "neo" => ProcessNeoScheme(uri),
            "neoauth" => ProcessNeoAuthScheme(uri),
            "https" => ProcessHttpsScheme(uri),
            _ => null
        };
        if (appLinkAction is null) return false;
#if IOS
        bool supportMultiWindow = DeviceInfo.Idiom == DeviceIdiom.Desktop || DeviceInfo.Idiom == DeviceIdiom.Tablet;
        if (!supportMultiWindow && Windows.Count > 0 && Windows[0].Page is AppShell shell)
        {
            appLinkAction.GotoRoute(shell);
        }
#endif
        return true;
    }

    AppLinkAction? ProcessNeoScheme(Uri uri)
    {
        try
        {
            return new PaymentAction(uri, protocolSettings);
        }
        catch
        {
            return null;
        }
    }

    static AppLinkAction? ProcessNeoAuthScheme(Uri uri)
    {
        try
        {
            return new AuthenticationAction(uri);
        }
        catch
        {
            return null;
        }
    }

    static AppLinkAction? ProcessHttpsScheme(Uri uri)
    {
        try
        {
            return uri.Segments[1] switch
            {
                "app/" => new LaunchDAppAction(uri),
                "news/" => new ViewNewsAction(uri),
                _ => null,
            };
        }
        catch
        {
            return null;
        }
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        string? lang = dbContext.Settings.Get("preference/language");
        if (!string.IsNullOrEmpty(lang))
        {
            CultureInfo culture = new(lang);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
        httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
        httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.CurrentUICulture.Name);
        Page page;
        if (walletProvider.GetWallet() is null)
        {
            page = serviceProvider.GetServiceOrCreateInstance<WelcomePage>();
            page = new NavigationPage(page);
        }
        else
        {
            page = appLinkAction?.GetPage(serviceProvider)
                ?? serviceProvider.GetServiceOrCreateInstance<AppShell>();
        }
        return new Window(page) { Title = "OneGate" };
    }
}
