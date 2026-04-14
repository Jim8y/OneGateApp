using Neo;
using Neo.Extensions;
using Neo.SmartContract;
using Neo.VM;
using System.Text.Json.Serialization;

namespace NeoOrder.OneGate.Models;

class InvocationArguments
{
    public required UInt160 Hash { get; init; }
    public required string Operation { get; init; }
    [JsonPropertyName("args")]
    public ContractParameter[]? Arguments { get; init; }
    public bool? AbortOnFail { get; init; }

    public void EmitScript(ScriptBuilder builder)
    {
        builder.EmitDynamicCall(Hash, Operation, Arguments ?? []);
        if (AbortOnFail == true)
            builder.Emit(OpCode.ASSERT);
    }
}
