using System.Diagnostics.CodeAnalysis;

namespace NeoOrder.OneGate.Services;

[SuppressMessage("Style", "IDE0060")]
public static partial class DataProtectionService
{
    public static partial Task<bool> CheckAvailabilityAsync();
    public static partial Task<bool> AuthenticateAsync(string? title = null, string? message = null);
    public static partial Task<byte[]> ProtectAsync(string plainText);
    public static partial Task<string> UnprotectAsync(byte[] protectedData, string? title = null, string? message = null);
}
