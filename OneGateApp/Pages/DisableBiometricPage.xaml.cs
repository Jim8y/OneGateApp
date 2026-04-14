using CommunityToolkit.Maui.Alerts;
using NeoOrder.OneGate.Controls;
using NeoOrder.OneGate.Data;
using NeoOrder.OneGate.Properties;
using NeoOrder.OneGate.Services;

namespace NeoOrder.OneGate.Pages;

public partial class DisableBiometricPage : ContentPage
{
    readonly ApplicationDbContext dbContext;

    public DisableBiometricPage(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
        InitializeComponent();
    }

    async void OnDisableClicked(object sender, EventArgs e)
    {
        await dbContext.Settings.DeleteAsync("biometric/credential");
        GlobalStates.Invalidate<SettingsPage>();
        await Shell.Current.GoToAsync("..");
        await Toast.Show(Strings.BiometricDisabledText);
    }
}
