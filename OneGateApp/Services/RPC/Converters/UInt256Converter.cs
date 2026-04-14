using Neo;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NeoOrder.OneGate.Services.RPC.Converters;

class UInt256Converter : JsonConverter<UInt256>
{
    public override UInt256? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString();
        if (value is null) return null;
        return UInt256.Parse(value);
    }

    public override void Write(Utf8JsonWriter writer, UInt256 value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
