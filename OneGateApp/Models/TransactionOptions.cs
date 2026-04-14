namespace NeoOrder.OneGate.Models;

public class TransactionOptions
{
    public long? SuggestedSystemFee { get; init; }
    public long? ExtraSystemFee { get; init; }
    public uint? ValidUntilBlock { get; init; }
}
