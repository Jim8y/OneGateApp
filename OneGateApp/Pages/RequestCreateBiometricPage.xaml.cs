using NeoOrder.OneGate.Services;

namespace NeoOrder.OneGate.Pages;

public partial class RequestCreateBiometricPage : ContentPage
{
    readonly AppShell shell;

    public RequestCreateBiometricPage(IServiceProvider serviceProvider)
    {
        shell = serviceProvider.GetServiceOrCreateInstance<AppShell>();
        InitializeComponent();
    }

    async void OnCreateClicked(object sender, EventArgs e)
    {
        GlobalStates.SetReturnPage<CreateBiometricPage>("//home");
        Window.Page = shell;
        await shell.GoToAsync("//home/settings/biometric/create");
    }

    void OnSkipClicked(object sender, EventArgs e)
    {
        Window.Page = shell;
    }
}
