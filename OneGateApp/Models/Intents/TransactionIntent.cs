using NeoOrder.OneGate.Services.RPC;

namespace NeoOrder.OneGate.Models.Intents;

public abstract class TransactionIntent
{
    internal virtual Task<TransactionIntent?> TryConvertToMoreSpecificIntentAsync(RpcClient rpcClient)
    {
        return Task.FromResult<TransactionIntent?>(null);
    }
}
