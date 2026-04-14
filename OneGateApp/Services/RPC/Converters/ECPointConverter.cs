using Neo.Cryptography.ECC;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NeoOrder.OneGate.Services.RPC.Converters;

class ECPointConverter : JsonConverter<ECPoint>
{
    public override ECPoint? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString();
        if (value is null) return null;
        return ECPoint.Parse(value, ECCurve.Secp256r1);
    }

    public override void Write(Utf8JsonWriter writer, ECPoint value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
