using Neo.SmartContract;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NeoOrder.OneGate.Services.RPC.Converters;

class ContractParametersContextConverter : JsonConverter<ContractParametersContext>
{
    public override ContractParametersContext? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        using var doc = JsonDocument.ParseValue(ref reader);
        string text = doc.RootElement.GetRawText();
        return ContractParametersContext.Parse(text, null!);
    }

    public override void Write(Utf8JsonWriter writer, ContractParametersContext value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(value.ToString());
    }
}
