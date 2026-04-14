using System.ComponentModel.DataAnnotations;

namespace NeoOrder.OneGate.Data;

public class Setting
{
    [Key]
    public required string Key { get; set; }
    public string? Value { get; set; }
}
