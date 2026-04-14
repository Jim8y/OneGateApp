using Neo;
using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NeoOrder.OneGate.Services.RPC.Converters;

class ContractStateConverter : JsonConverter<ContractState>
{
    public override ContractState? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonObject? obj = JsonSerializer.Deserialize<JsonObject>(ref reader, options);
        if (obj is null) return null;
        return new ContractState
        {
            Id = obj["id"]!.GetValue<int>(),
            UpdateCounter = obj["updatecounter"]?.GetValue<ushort>() ?? 0,
            Hash = obj["hash"].Deserialize<UInt160>(options)!,
            Nef = obj["nef"].Deserialize<NefFile>(options)!,
            Manifest = ContractManifest.FromJson((JObject)JToken.Parse(obj["manifest"]!.ToJsonString())!)
        };
    }

    public override void Write(Utf8JsonWriter writer, ContractState value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(value.ToJson().ToString());
    }
}
