// The production page and Neo wallet implementation are linked unchanged.
// Only MAUI navigation, rendering and the unrelated settings store are replaced.
using NeoOrder.OneGate.Controls.Views;

namespace NeoOrder.OneGate.Pages
{
    public class ContentPage
    {
        protected void OnPropertyChanged() { }
        protected virtual void OnAppearing() { }
        protected virtual void OnDisappearing() { }
    }
    public sealed class Shell
    {
        public static Shell Current { get; } = new();
        public Task GoToAsync(string route) => Task.CompletedTask;
    }
    public partial class ChangePasswordPage
    {
        readonly ErrorMessage errMsg = new();
        public string? ErrorForTest => errMsg.LastError;
        void InitializeComponent() { }
        public async Task SubmitForTestAsync()
        {
            CommunityToolkit.Maui.Alerts.Toast.Shown = new(TaskCreationOptions.RunContinuationsAsynchronously);
            OnSubmitted(new Submit(), EventArgs.Empty);
            await Task.WhenAny(CommunityToolkit.Maui.Alerts.Toast.Shown.Task, errMsg.Shown.Task).WaitAsync(TimeSpan.FromSeconds(30));
        }
    }
    public sealed class ErrorMessage
    {
        public string? LastError { get; private set; }
        public TaskCompletionSource Shown { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public void SetError(string message) { LastError = message; Shown.TrySetResult(); }
    }
}
namespace NeoOrder.OneGate.Controls { public sealed class Placeholder { } }
namespace NeoOrder.OneGate.Controls.Views
{
    public sealed class Submit
    {
        public IDisposable EnterBusyState() => new BusyState();
        sealed class BusyState : IDisposable { public void Dispose() { } }
    }
}
namespace NeoOrder.OneGate.Data
{
    public sealed class ApplicationDbContext { public SettingsStore Settings { get; } = new(); }
    public sealed class SettingsStore
    {
        public bool HasBiometricCredential { get; private set; } = true;
        public Task<bool> ExistsAsync(string key) => Task.FromResult(HasBiometricCredential);
        public Task DeleteAsync(string key) { HasBiometricCredential = false; return Task.CompletedTask; }
    }
}
namespace NeoOrder.OneGate.Properties
{
    public static class Strings
    {
        public const string BiometricResetText = "Biometric reset";
        public const string PasswordChanged = "Password changed";
        public const string ErrorMessageIncorrectPassword = "Incorrect password";
        public const string WalletPasswordSaveFailed = "Could not save the new password. Your current password is unchanged. Please try again.";
    }
}
namespace Plugin.Maui.ScreenSecurity
{
    public interface IScreenSecurity
    {
        void ActivateScreenSecurityProtection();
        void DeactivateScreenSecurityProtection();
    }
}
public sealed class ScreenSecurity : Plugin.Maui.ScreenSecurity.IScreenSecurity
{
    public void ActivateScreenSecurityProtection() { }
    public void DeactivateScreenSecurityProtection() { }
}
namespace CommunityToolkit.Maui.Alerts
{
    public static class Toast
    {
        public static TaskCompletionSource Shown { get; set; } = new();
        public static Task Show(string message) { Shown.TrySetResult(); return Task.CompletedTask; }
    }
}
