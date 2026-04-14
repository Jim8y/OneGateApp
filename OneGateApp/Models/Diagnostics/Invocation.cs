using Neo;
using Neo.Extensions;
using Neo.VM.Types;
using NeoOrder.OneGate.Models.Intents;
using NeoOrder.OneGate.Services.RPC;
using System.Text.Json.Serialization;

namespace NeoOrder.OneGate.Models.Diagnostics;

public class Invocation : DiagnosticNode, ICanCall
{
    public required UInt160 Hash { get; init; }
    public required string Method { get; init; }
    [JsonPropertyName("args")]
    public required StackItem[] Arguments { get; init; }
    [JsonPropertyName("return")]
    public StackItem? ReturnValue { get; init; }
    public required bool IsNative { get; init; }
    public required DiagnosticNode[] Calls { get; init; }

    internal async Task<InvocationIntent> ToIntentAsync(RpcClient rpcClient)
    {
        return new InvocationIntent
        {
            Contract = await rpcClient.GetContractState(Hash),
            Method = Method,
            Arguments = Arguments.Select(p => p.ToParameter()).ToArray()
        };
    }
}
