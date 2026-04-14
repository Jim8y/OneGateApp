namespace NeoOrder.OneGate.Models;

class Ticker
{
    public required string Symbol { get; init; }
    public required decimal Price { get; init; }
}
