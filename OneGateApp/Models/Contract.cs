using System.Diagnostics.CodeAnalysis;

namespace NeoOrder.OneGate.Models;

record Contract
{
    public byte[]? Script { get; init; }
    public required Parameter[] Parameters { get; init; }
    public bool Deployed { get; init; }

    [return: NotNullIfNotNull(nameof(contract))]
    public static Contract? From(Neo.SmartContract.Contract? contract)
    {
        if (contract is null) return null;
        return new Contract
        {
            Script = contract.Script,
            Parameters = contract.ParameterList.Select(p => new Parameter { Type = p }).ToArray()
        };
    }
}
