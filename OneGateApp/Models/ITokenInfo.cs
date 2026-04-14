using Neo;
using System.Numerics;

namespace NeoOrder.OneGate.Models;

public interface ITokenInfo
{
    public UInt160 Hash { get; }
    public string Name { get; }
    public string Symbol { get; }
    public BigInteger TotalSupply { get; }
    public string? Icon { get; set; }
}
