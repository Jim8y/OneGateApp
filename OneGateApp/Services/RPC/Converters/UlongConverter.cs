using System.Text.Json;
using System.Text.Json.Serialization;

namespace NeoOrder.OneGate.Services.RPC.Converters;

class UlongConverter : JsonConverter<ulong>
{
    const ulong JS_SAFE_MAX = 9007199254740991;

    public override ulong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
            return ulong.Parse(reader.GetString()!);
        else
            return reader.GetUInt64();
    }

    public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
    {
        if (value > JS_SAFE_MAX)
            writer.WriteStringValue(value.ToString());
        else
            writer.WriteNumberValue(value);
    }
}
