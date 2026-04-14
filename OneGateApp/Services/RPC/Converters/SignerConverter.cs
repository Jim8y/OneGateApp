using Neo.Json;
using Neo.Network.P2P.Payloads;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NeoOrder.OneGate.Services.RPC.Converters;

class SignerConverter : JsonConverter<Signer>
{
    public override Signer? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        using var doc = JsonDocument.ParseValue(ref reader);
        string text = doc.RootElement.GetRawText();
        return Signer.FromJson((JObject)JToken.Parse(text)!);
    }

    public override void Write(Utf8JsonWriter writer, Signer value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(value.ToJson().ToString());
    }
}
