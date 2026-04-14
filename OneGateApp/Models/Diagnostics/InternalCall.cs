using Neo.VM.Types;
using System.Text.Json.Serialization;

namespace NeoOrder.OneGate.Models.Diagnostics;

public class InternalCall : DiagnosticNode, ICanCall
{
    public required int Position { get; init; }
    [JsonPropertyName("args")]
    public required StackItem[] Arguments { get; init; }
    public required DiagnosticNode[] Calls { get; init; }
}
