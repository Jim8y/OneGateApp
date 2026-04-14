using NeoOrder.OneGate.Services;
using System.Reflection;
using System.Text.Json;

namespace NeoOrder.OneGate.Resources;

static class EmbeddedResource
{
    static readonly string ns = typeof(EmbeddedResource).Namespace!;

    public static Stream Open(string fileName)
    {
        return Assembly.GetExecutingAssembly().GetManifestResourceStream($"{ns}.Raw.{fileName}")!;
    }

    public static T LoadJson<T>(string fileName)
    {
        using var stream = Open(fileName);
        return JsonSerializer.Deserialize<T>(stream, SharedOptions.JsonSerializerOptions)!;
    }
}
