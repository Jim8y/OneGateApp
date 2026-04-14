using System.Windows.Input;

namespace NeoOrder.OneGate.Models;

public class SettingEntry(string name)
{
    public string Name => name;
    public string? CurrentValue { get; init; }
    public bool IsDanger { get; init; }
    public ICommand? Command { get; init; }
    public object? CommandParameter { get; init; }
}
