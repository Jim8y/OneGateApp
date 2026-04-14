using Neo.Cryptography.ECC;
using System.Text.Json.Serialization;

namespace NeoOrder.OneGate.Models;

public class AuthenticationResponsePayload
{
    public required string Algorithm { get; init; }
    public required uint Network { get; init; }
    [JsonPropertyName("pubkey")]
    public required ECPoint PublicKey { get; init; }
    public required string Address { get; init; }
    public required ulong Nonce { get; init; }
    public required uint Timestamp { get; init; }
    public required byte[] Signature { get; init; }
}
