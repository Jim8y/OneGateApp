namespace NeoOrder.OneGate.Models;

public partial class SettingEntryGroup(string name) : List<SettingEntry>
{
    public string Name => name;

    public static SettingEntryGroup Create(string name, IEnumerable<SettingEntry> entries)
    {
        var group = new SettingEntryGroup(name);
        group.AddRange(entries);
        return group;
    }
}
