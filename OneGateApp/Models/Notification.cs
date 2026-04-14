using Neo;
using Neo.VM.Types;
using System.Text.Json.Serialization;

namespace NeoOrder.OneGate.Models;

public class Notification
{
    public required UInt160 Contract { get; init; }
    [JsonPropertyName("eventname")]
    public required string EventName { get; init; }
    public required StackItem State { get; init; }
}
