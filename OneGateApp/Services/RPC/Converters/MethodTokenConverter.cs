using Neo;
using Neo.SmartContract;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NeoOrder.OneGate.Services.RPC.Converters;

class MethodTokenConverter : JsonConverter<MethodToken>
{
    public override MethodToken? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonObject? obj = JsonSerializer.Deserialize<JsonObject>(ref reader, options);
        if (obj is null) return null;
        return new MethodToken
        {
            Hash = obj["hash"].Deserialize<UInt160>(options)!,
            Method = obj["method"]!.GetValue<string>(),
            ParametersCount = obj["paramcount"]!.GetValue<ushort>(),
            HasReturnValue = obj["hasreturnvalue"]!.GetValue<bool>(),
            CallFlags = obj["callflags"].Deserialize<CallFlags>(options)
        };
    }

    public override void Write(Utf8JsonWriter writer, MethodToken value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(value.ToJson().ToString());
    }
}
