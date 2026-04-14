using Neo;
using Neo.SmartContract;
using NeoOrder.OneGate.Properties;
using NeoOrder.OneGate.Services.RPC;

namespace NeoOrder.OneGate.Models.Intents;

class InvocationIntent : TransactionIntent
{
    public required ContractState Contract { get; init; }
    public required string Method { get; init; }
    public ContractParameter[]? Arguments { get; init; }

    public UInt160 Hash => Contract.Hash;
    public string ContractName => Contract.Manifest.Name;

    public string ArgumentsString
    {
        get
        {
            if (Arguments is null || Arguments.Length == 0)
                return Strings.NoParameters;
            return $"({string.Join(", ", Arguments)})";
        }
    }

    internal override async Task<TransactionIntent?> TryConvertToMoreSpecificIntentAsync(RpcClient rpcClient)
    {
        if (Method != "transfer") return null;
        if (Contract.Manifest.SupportedStandards.Contains("NEP-11"))
        {
            if (Arguments?.Length != 3) return null;
            byte[] tokenId = Arguments[1].GetByteArray();
            NFT nft = await rpcClient.GetNFTInfo(Contract.Hash, tokenId);
            nft.TokenInfo = await rpcClient.GetNep11TokenInfo(Contract.Hash);
            return new Nep11TransferIntent
            {
                Asset = nft,
                From = await rpcClient.OwnerOf(Contract.Hash, tokenId),
                To = Arguments[0].GetHash160(),
                Data = Arguments[2]
            };
        }
        else if (Contract.Manifest.SupportedStandards.Contains("NEP-17"))
        {
            if (Arguments?.Length != 4) return null;
            return new TransferIntent
            {
                Asset = await rpcClient.GetTokenInfo(Contract.Hash),
                From = Arguments[0].GetHash160(),
                To = Arguments[1].GetHash160(),
                Amount = Arguments[2].GetBigInteger(),
                Data = Arguments[3]
            };
        }
        return null;
    }
}
