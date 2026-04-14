using Neo.Extensions;
using Neo.VM;
using Neo.VM.Types;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Array = Neo.VM.Types.Array;
using Buffer = Neo.VM.Types.Buffer;
using Map = Neo.VM.Types.Map;

namespace NeoOrder.OneGate.Services.RPC.Converters;

class StackItemConverter : JsonConverter<StackItem>
{
    public override StackItem? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonObject? obj = JsonSerializer.Deserialize<JsonObject>(ref reader, options);
        if (obj is null) return null;
        return ConvertToStackItem(obj, options);
    }

    static StackItem ConvertToStackItem(JsonObject json, JsonSerializerOptions options)
    {
        StackItemType type = json["type"].Deserialize<StackItemType>(options);
        switch (type)
        {
            case StackItemType.Boolean:
                return json["value"]!.GetValue<bool>() ? StackItem.True : StackItem.False;
            case StackItemType.Buffer:
                return new Buffer(json["value"].Deserialize<byte[]>(options));
            case StackItemType.ByteString:
                return new ByteString(json["value"].Deserialize<byte[]>(options));
            case StackItemType.Integer:
                return json["value"].Deserialize<BigInteger>(options);
            case StackItemType.Array:
                Array array = new();
                foreach (var item in json["value"]!.AsArray().Cast<JsonObject>())
                    array.Add(ConvertToStackItem(item, options));
                return array;
            case StackItemType.Struct:
                Struct @struct = new();
                foreach (var item in json["value"]!.AsArray().Cast<JsonObject>())
                    @struct.Add(ConvertToStackItem(item, options));
                return @struct;
            case StackItemType.Map:
                Map map = new();
                foreach (var item in json["value"]!.AsArray().Cast<JsonObject>())
                {
                    PrimitiveType key = (PrimitiveType)ConvertToStackItem(item["key"]!.AsObject(), options);
                    map[key] = ConvertToStackItem(item["value"]!.AsObject(), options);
                }
                return map;
            case StackItemType.Pointer:
                return new Pointer(Script.Empty, json["value"]!.GetValue<int>());
            case StackItemType.InteropInterface:
                return new InteropInterface(json);
            default:
                return json["value"]?.GetValue<string>() ?? StackItem.Null;
        }
    }

    public override void Write(Utf8JsonWriter writer, StackItem value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(value.ToJson().ToString());
    }
}
