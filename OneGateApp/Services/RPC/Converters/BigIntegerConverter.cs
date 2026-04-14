using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NeoOrder.OneGate.Services.RPC.Converters;

class BigIntegerConverter : JsonConverter<BigInteger>
{
    const long JS_SAFE_MAX = 9007199254740991;
    const long JS_SAFE_MIN = -9007199254740991;

    public override BigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
            return BigInteger.Parse(reader.GetString()!);
        else
            return reader.GetInt64();
    }

    public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options)
    {
        if (value > JS_SAFE_MAX || value < JS_SAFE_MIN)
        {
            writer.WriteStringValue(value.ToString());
        }
        else
        {
            writer.WriteNumberValue((long)value);
        }
    }
}
