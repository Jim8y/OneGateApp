using NeoOrder.OneGate.Properties;

namespace NeoOrder.OneGate.Controls.Views.Validation;

public partial class Required : Validator
{
    public bool AllowWhiteSpace { get; set; }

    public override bool Check(object? value)
    {
        return value switch
        {
            null => false,
            string s => AllowWhiteSpace ? !string.IsNullOrEmpty(s) : !string.IsNullOrWhiteSpace(s),
            _ => true
        };
    }

    public override string GetErrorMessage(string? title)
    {
        return string.Format(ErrorMessage ?? Strings.DefaultRequiredErrorMessage, title ?? Strings.Field);
    }
}
