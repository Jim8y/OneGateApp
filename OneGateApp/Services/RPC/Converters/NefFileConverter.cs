using Neo.SmartContract;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NeoOrder.OneGate.Services.RPC.Converters;

class NefFileConverter : JsonConverter<NefFile>
{
    public override NefFile? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonObject? obj = JsonSerializer.Deserialize<JsonObject>(ref reader, options);
        if (obj is null) return null;
        return new NefFile
        {
            Compiler = obj["compiler"]!.GetValue<string>(),
            Source = obj["source"]!.GetValue<string>(),
            Tokens = obj["tokens"].Deserialize<MethodToken[]>(options)!,
            Script = obj["script"].Deserialize<byte[]>(options),
            CheckSum = obj["checksum"]!.GetValue<uint>()
        };
    }

    public override void Write(Utf8JsonWriter writer, NefFile value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(value.ToJson().ToString());
    }
}
