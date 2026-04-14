using Neo;
using Neo.SmartContract;

namespace NeoOrder.OneGate.Models.Intents;

class Nep11TransferIntent : TransactionIntent
{
    public required NFT Asset { get; init; }
    public required UInt160 From { get; init; }
    public required UInt160 To { get; init; }
    public ContractParameter? Data { get; init; }
}
