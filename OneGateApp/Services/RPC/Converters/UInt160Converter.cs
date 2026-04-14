using Neo;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NeoOrder.OneGate.Services.RPC.Converters;

class UInt160Converter : JsonConverter<UInt160>
{
    public override UInt160? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString();
        if (value is null) return null;
        return UInt160.Parse(value);
    }

    public override void Write(Utf8JsonWriter writer, UInt160 value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
