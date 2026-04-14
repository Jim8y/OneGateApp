using NeoOrder.OneGate.Properties;

namespace NeoOrder.OneGate.Controls.Views.Validation;

public partial class Compare : Validator
{
    public static readonly BindableProperty ToProperty = BindableProperty.Create(nameof(To), typeof(object), typeof(Compare));

    public object? To
    {
        get => (object?)GetValue(ToProperty);
        set => SetValue(ToProperty, value);
    }

    public override bool Check(object? value)
    {
        if (value == null && To == null) return true;
        if (value == null || To == null) return false;
        return value.Equals(To);
    }

    public override string GetErrorMessage(string? title)
    {
        return string.Format(ErrorMessage ?? Strings.DefaultCompareErrorMessage, title);
    }
}
