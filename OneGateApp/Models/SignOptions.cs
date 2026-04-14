namespace NeoOrder.OneGate.Models;

class SignOptions
{
    public bool? IsBase64Encoded { get; init; }
    public bool? IsTypedData { get; init; }
    public bool? IsLedgerCompatible { get; init; }
}
