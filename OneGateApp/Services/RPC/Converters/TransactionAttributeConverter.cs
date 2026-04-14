using Neo;
using Neo.Network.P2P.Payloads;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NeoOrder.OneGate.Services.RPC.Converters;

class TransactionAttributeConverter : JsonConverter<TransactionAttribute>
{
    public override TransactionAttribute? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonObject? obj = JsonSerializer.Deserialize<JsonObject>(ref reader, options);
        if (obj is null) return null;
        return TransactionAttributeConverter.ConvertToTransactionAttribute(obj, options);
    }

    static TransactionAttribute? ConvertToTransactionAttribute(JsonObject json, JsonSerializerOptions options)
    {
        TransactionAttributeType type = json["type"].Deserialize<TransactionAttributeType>(options);
        return type switch
        {
            TransactionAttributeType.HighPriority => new HighPriorityAttribute(),
            TransactionAttributeType.OracleResponse => new OracleResponse()
            {
                Id = json["id"].Deserialize<ulong>(options),
                Code = json["code"].Deserialize<OracleResponseCode>(options),
                Result = json["result"].Deserialize<byte[]>(options)
            },
            TransactionAttributeType.NotValidBefore => new NotValidBefore()
            {
                Height = json["height"].Deserialize<uint>(options)
            },
            TransactionAttributeType.Conflicts => new Conflicts()
            {
                Hash = json["hash"].Deserialize<UInt256>(options)!
            },
            TransactionAttributeType.NotaryAssisted => new NotaryAssisted()
            {
                NKeys = json["nkeys"]!.GetValue<byte>()
            },
            _ => throw new FormatException()
        };
    }

    public override void Write(Utf8JsonWriter writer, TransactionAttribute value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(value.ToJson().ToString());
    }
}
