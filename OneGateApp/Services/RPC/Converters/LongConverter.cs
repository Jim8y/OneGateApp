using System.Text.Json;
using System.Text.Json.Serialization;

namespace NeoOrder.OneGate.Services.RPC.Converters;

class LongConverter : JsonConverter<long>
{
    const long JS_SAFE_MAX = 9007199254740991;

    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
            return long.Parse(reader.GetString()!);
        else
            return reader.GetInt64();
    }

    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
    {
        if (value > JS_SAFE_MAX)
            writer.WriteStringValue(value.ToString());
        else
            writer.WriteNumberValue(value);
    }
}
