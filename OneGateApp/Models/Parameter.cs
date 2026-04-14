using Neo.SmartContract;

namespace NeoOrder.OneGate.Models;

record Parameter
{
    public string? Name { get; init; }
    public required ContractParameterType Type { get; init; }
}
