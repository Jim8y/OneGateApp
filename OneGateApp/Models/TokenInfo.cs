using Neo;
using System.Numerics;
using System.Text.Json.Serialization;

namespace NeoOrder.OneGate.Models;

public class TokenInfo : ITokenInfo
{
    public required UInt160 Hash { get; init; }
    public required string Name { get; init; }
    public required string Symbol { get; init; }
    public required byte Decimals { get; init; }
    public BigInteger TotalSupply { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWriting)]
    public string? Icon { get; set; }
    [JsonIgnore]
    public decimal? Price { get; set; }
}
