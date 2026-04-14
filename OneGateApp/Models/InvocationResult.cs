using Neo.VM;
using Neo.VM.Types;
using NeoOrder.OneGate.Models.Diagnostics;
using System.Text.Json.Serialization;

namespace NeoOrder.OneGate.Models;

public class InvocationResult
{
    public required byte[] Script { get; init; }
    public required VMState State { get; init; }
    [JsonPropertyName("gasconsumed")]
    public required long GasConsumed { get; init; }
    public string? Exception { get; init; }
    public required Notification[] Notifications { get; init; }
    public required StackItem[] Stack { get; init; }
    public Diagnostic? Diagnostics { get; init; }

    public void EnsureSuccess()
    {
        if (State != VMState.HALT)
            throw new DapiException(10004, "Invocation failed", this);
    }
}
