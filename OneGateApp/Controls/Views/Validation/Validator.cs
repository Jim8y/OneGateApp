using NeoOrder.OneGate.Properties;

namespace NeoOrder.OneGate.Controls.Views.Validation;

public abstract class Validator : Element
{
    public static readonly BindableProperty IsEnabledProperty = BindableProperty.Create(nameof(IsEnabled), typeof(bool), typeof(Validator), true);

    public bool IsEnabled
    {
        get => (bool)GetValue(IsEnabledProperty);
        set => SetValue(IsEnabledProperty, value);
    }
    public string? ErrorMessage { get; set; }

    public abstract bool Check(object? value);

    public virtual string GetErrorMessage(string? title)
    {
        return string.Format(ErrorMessage ?? Strings.DefaultValidatorErrorMessage, title);
    }
}
