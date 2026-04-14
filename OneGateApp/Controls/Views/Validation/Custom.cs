namespace NeoOrder.OneGate.Controls.Views.Validation;

public partial class Custom : Validator
{
    public event EventHandler<CustomValidationEventArgs>? Validate;

    public override bool Check(object? value)
    {
        var args = new CustomValidationEventArgs(value);
        Validate?.Invoke(this, args);
        if (!args.IsValid && args.ErrorMessage is not null)
            ErrorMessage = args.ErrorMessage;
        return args.IsValid;
    }
}
