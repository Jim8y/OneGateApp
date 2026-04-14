using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NeoOrder.OneGate.Services;
using System.Text.Json;

namespace NeoOrder.OneGate.Data;

static class SettingsExtensions
{
    public static async Task<bool> ExistsAsync(this DbSet<Setting> settings, string key)
    {
        return await settings.AnyAsync(p => p.Key == key);
    }

    public static bool Delete(this DbSet<Setting> settings, string key)
    {
        return settings.Where(p => p.Key == key).ExecuteDelete() > 0;
    }

    public static async Task<bool> DeleteAsync(this DbSet<Setting> settings, string key)
    {
        return await settings.Where(p => p.Key == key).ExecuteDeleteAsync() > 0;
    }

    public static string? Get(this DbSet<Setting> settings, string key)
    {
        return settings.FirstOrDefault(p => p.Key == key)?.Value;
    }

    public static async Task<string?> GetAsync(this DbSet<Setting> settings, string key)
    {
        return (await settings.FirstOrDefaultAsync(p => p.Key == key))?.Value;
    }

    public static void Put(this DbSet<Setting> settings, string key, string? value)
    {
        DbContext dbContext = ((IInfrastructure<DbContext>)settings).Instance;
        dbContext.Database.ExecuteSql($"INSERT INTO [Settings]([Key], [Value]) VALUES({key}, {value}) ON CONFLICT([Key]) DO UPDATE SET [Value] = {value}");
    }

    public static async Task PutAsync(this DbSet<Setting> settings, string key, string? value)
    {
        DbContext dbContext = ((IInfrastructure<DbContext>)settings).Instance;
        await dbContext.Database.ExecuteSqlAsync($"INSERT INTO [Settings]([Key], [Value]) VALUES({key}, {value}) ON CONFLICT([Key]) DO UPDATE SET [Value] = {value}");
    }

    public static T? Get<T>(this DbSet<Setting> settings, string key) where T : notnull
    {
        string? value = settings.Get(key);
        if (value is null) return default;
        return JsonSerializer.Deserialize<T>(value, SharedOptions.JsonSerializerOptions);
    }

    public static async Task<T?> GetAsync<T>(this DbSet<Setting> settings, string key) where T : notnull
    {
        string? value = await settings.GetAsync(key);
        if (value is null) return default;
        return JsonSerializer.Deserialize<T>(value, SharedOptions.JsonSerializerOptions);
    }

    public static void Put<T>(this DbSet<Setting> settings, string key, T value) where T : notnull
    {
        settings.Put(key, JsonSerializer.Serialize(value, SharedOptions.JsonSerializerOptions));
    }

    public static async Task PutAsync<T>(this DbSet<Setting> settings, string key, T value) where T : notnull
    {
        await settings.PutAsync(key, JsonSerializer.Serialize(value, SharedOptions.JsonSerializerOptions));
    }
}
