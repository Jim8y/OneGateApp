using Neo.Json;
using Neo.SmartContract;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NeoOrder.OneGate.Services.RPC.Converters;

class ContractParameterConverter : JsonConverter<ContractParameter>
{
    public override ContractParameter? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        using var doc = JsonDocument.ParseValue(ref reader);
        string text = doc.RootElement.GetRawText();
        return ContractParameter.FromJson((JObject)JToken.Parse(text)!);
    }

    public override void Write(Utf8JsonWriter writer, ContractParameter value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(value.ToJson().ToString());
    }
}
