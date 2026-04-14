using Neo;
using Neo.Wallets;

namespace NeoOrder.OneGate.Models;

record Account
{
    public required UInt160 Hash { get; init; }
    public required string Address { get; init; }
    public string? Label { get; init; }
    public Contract? Contract { get; init; }
    public object? Extra { get; init; }

    public static Account From(WalletAccount account)
    {
        return new Account
        {
            Hash = account.ScriptHash,
            Address = account.Address,
            Label = account.Label,
            Contract = Contract.From(account.Contract)
        };
    }
}
