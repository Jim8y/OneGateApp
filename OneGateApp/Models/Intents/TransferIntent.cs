using Neo;
using Neo.SmartContract;
using System.Numerics;

namespace NeoOrder.OneGate.Models.Intents;

class TransferIntent : TransactionIntent
{
    public required TokenInfo Asset { get; init; }
    public required UInt160 From { get; init; }
    public required UInt160 To { get; init; }
    public required BigInteger Amount { get; init; }
    public ContractParameter? Data { get; init; }

    public BigDecimal DecimalAmount => new(Amount, Asset.Decimals);
    public string DisplayAmount => $"{DecimalAmount} {Asset.Symbol}";
}
