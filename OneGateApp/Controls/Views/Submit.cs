using CommunityToolkit.Maui.Views;

namespace NeoOrder.OneGate.Controls.Views;

public partial class Submit : SpinnerButton
{
    public event EventHandler? Submitted;

    public Submit()
    {
        Clicked += Submit_Clicked;
    }

    private void Submit_Clicked(object? sender, EventArgs e)
    {
        Element container = Parent;
        while (true)
        {
            if (container is Page) break;
            if (container is Popup) break;
            if (container.Parent is null) break;
            container = container.Parent;
        }
        var rules = container
            .GetVisualTreeDescendants()
            .OfType<ValidationMessage>()
            .ToArray();
        bool isValid = true;
        foreach (var rule in rules)
            isValid &= rule.Validate();
        if (isValid)
            Submitted?.Invoke(this, EventArgs.Empty);
    }
}
