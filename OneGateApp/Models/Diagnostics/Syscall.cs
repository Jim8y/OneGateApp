using Neo.VM.Types;
using System.Text.Json.Serialization;

namespace NeoOrder.OneGate.Models.Diagnostics;

public class Syscall : DiagnosticNode
{
    public required string Name { get; init; }
    [JsonPropertyName("args")]
    public required StackItem[] Arguments { get; init; }
    public StackItem? Result { get; init; }
}
