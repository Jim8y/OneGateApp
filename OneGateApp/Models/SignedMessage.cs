using Neo;
using Neo.Cryptography.ECC;
using System.Text.Json.Serialization;

namespace NeoOrder.OneGate.Models;

class SignedMessage
{
    public required byte[] Payload { get; init; }
    public required byte[] Signature { get; init; }
    public required UInt160 Account { get; init; }
    [JsonPropertyName("pubkey")]
    public required ECPoint PublicKey { get; init; }
}
