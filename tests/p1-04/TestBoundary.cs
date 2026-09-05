// Standalone service tests retain the actual protocol and service. Only the
// platform storage, discovery, UI dispatch and page-construction edges are fake.
namespace NeoOrder.OneGate.Services.RemoteDebug
{
    sealed class MdnsRemoteDebuggerDiscovery : IAsyncDisposable
    {
        public event Action<string, string, int>? RemoteDebuggerDiscovered { add { } remove { } }
        public Task StartAsync() => Task.CompletedTask;
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}

namespace NeoOrder.OneGate.Pages
{
    public class LaunchDAppPage
    {
        public void ConfigureRemoteDebug(string id, Services.RemoteDebug.RemoteDebugService service) { }
        public void ApplyQueryAttributes(IDictionary<string, object> query) { }
    }
}

namespace NeoOrder.OneGate
{
    static class ServiceProviderExtensions
    {
        public static T GetServiceOrCreateInstance<T>(this IServiceProvider services) where T : new() => new();
    }
    static class MainThread
    {
        public static Task InvokeOnMainThreadAsync(Func<Task> action) => action();
    }
    sealed class SecureStorage
    {
        public static SecureStorage Default { get; } = new();
        public Task<string?> GetAsync(string key) => Task.FromResult<string?>(null);
        public Task SetAsync(string key, string value) => Task.CompletedTask;
    }
    static class DeviceInfo
    {
        public static string Name => "Regression harness";
        public static string Platform => "Test";
    }
    sealed class Application
    {
        public static Application Current { get; } = new();
        public void OpenWindow(Window window) { }
    }
    sealed class Window(NavigationPage page) { public NavigationPage Page { get; } = page; }
    sealed class NavigationPage(Pages.LaunchDAppPage page) { public Pages.LaunchDAppPage Page { get; } = page; }
}
