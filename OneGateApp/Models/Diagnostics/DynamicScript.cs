using Neo;
using Neo.VM.Types;
using System.Text.Json.Serialization;

namespace NeoOrder.OneGate.Models.Diagnostics;

public class DynamicScript : DiagnosticNode, ICanCall
{
    public required UInt160 Hash { get; init; }
    [JsonPropertyName("args")]
    public required StackItem[] Arguments { get; init; }
    [JsonPropertyName("return")]
    public StackItem? ReturnValue { get; init; }
    public required DiagnosticNode[] Calls { get; init; }
}
