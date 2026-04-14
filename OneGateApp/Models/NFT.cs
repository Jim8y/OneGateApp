using Neo;
using System.Text.Json.Serialization;

namespace NeoOrder.OneGate.Models;

public class NFT
{
    public required UInt160 CollectionId { get; init; }
    public required byte[] TokenId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? Image { get; init; }
    public string? TokenURI { get; init; }

    [JsonIgnore]
    public Nep11TokenInfo? TokenInfo { get; set; }
}
