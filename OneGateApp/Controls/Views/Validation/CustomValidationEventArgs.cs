namespace NeoOrder.OneGate.Controls.Views.Validation;

public class CustomValidationEventArgs(object? value) : EventArgs
{
    public object? Value => value;
    public bool IsValid { get; set; } = true;
    public string? ErrorMessage { get; set; }
}
