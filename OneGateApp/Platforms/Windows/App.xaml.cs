using Microsoft.Windows.AppLifecycle;

namespace NeoOrder.OneGate.WinUI;

public partial class App : MauiWinUIApplication
{
    public App()
    {
        InitializeComponent();
    }

    protected override MauiApp CreateMauiApp()
    {
        MauiApp app = MauiProgram.CreateMauiApp();
        Workarounds.FixPickerHandler();
        var arguments = AppInstance.GetCurrent().GetActivatedEventArgs();
        if (arguments.Kind == ExtendedActivationKind.Protocol && arguments.Data is Windows.ApplicationModel.Activation.ProtocolActivatedEventArgs protocol)
        {
            var application = (OneGate.App)app.Services.GetRequiredService<IApplication>();
            if (!application.ProcessAppLinkUri(protocol.Uri))
                application.Quit();
        }
        return app;
    }
}
