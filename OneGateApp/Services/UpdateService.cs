namespace NeoOrder.OneGate.Services;

public partial class UpdateService(HttpClient httpClient)
{
    public event EventHandler? UpdateAvailable;

    public bool IsUpdating { get; private set; }

    public async Task<bool> CheckForUpdatesAsync()
    {
        bool available = await CheckForUpdatesInternalAsync();
        if (available) UpdateAvailable?.Invoke(this, EventArgs.Empty);
        return available;
    }

    private partial Task<bool> CheckForUpdatesInternalAsync();

    async Task<bool> CheckForUpdatesFallbackAsync()
    {
        Version latest = new(await httpClient.GetStringAsync("/api/app/version"));
        return latest > AppInfo.Version;
    }

    public async Task UpdateAsync()
    {
        if (IsUpdating) return;
        IsUpdating = true;
        try
        {
            await UpdateInternalAsync();
        }
        catch
        {
            IsUpdating = false;
            throw;
        }
    }

    private partial Task UpdateInternalAsync();

    async Task UpdateFallbackAsync()
    {
        await Browser.OpenAsync($"https://{SharedOptions.OneGateDomain}/download", BrowserLaunchMode.External);
        IsUpdating = false;
    }
}
