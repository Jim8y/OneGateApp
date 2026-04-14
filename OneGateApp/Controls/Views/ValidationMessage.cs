using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using NeoOrder.OneGate.Controls.Views.Validation;
using NeoOrder.OneGate.Properties;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace NeoOrder.OneGate.Controls.Views;

[ContentProperty(nameof(Validators))]
public partial class ValidationMessage : Label
{
    public static readonly BindableProperty TargetProperty = BindableProperty.Create(nameof(Target), typeof(object), typeof(ValidationMessage), null, propertyChanged: OnTargetChanged);

    Func<object?>? GetValidatableValue;

    public string? Title { get; set; }
    public object? Target { get => (object?)GetValue(TargetProperty); set => SetValue(TargetProperty, value); }
    public ObservableCollection<Validator> Validators { get; } = [];

    public ValidationMessage()
    {
        IsVisible = false;
        FontSize = 12;
        this.SetAppThemeColor(TextColorProperty, (AppThemeColor)Application.Current!.Resources["Danger"]);
        Validators.CollectionChanged += Validators_CollectionChanged;
    }

    static void OnTargetChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ValidationMessage _this = (ValidationMessage)bindable;
        switch (_this.Target)
        {
            case CheckBox checkBox:
                _this.GetValidatableValue = () => checkBox.IsChecked;
                checkBox.CheckedChanged += (_, _) => _this.Validate();
                break;
            case CheckBoxWithLabel checkBox:
                _this.GetValidatableValue = () => checkBox.IsChecked;
                checkBox.CheckedChanged += (_, _) => _this.Validate();
                break;
            case Editor editor:
                _this.GetValidatableValue = () => editor.Text;
                editor.Completed += (_, _) => _this.Validate();
                break;
            case Entry entry:
                _this.GetValidatableValue = () => entry.Text;
                entry.Completed += (_, _) => _this.Validate();
                break;
            case Picker picker:
                _this.GetValidatableValue = () => picker.SelectedItem;
                picker.SelectedIndexChanged += (_, _) => _this.Validate();
                break;
            default:
                _this.GetValidatableValue = () => _this.Target;
                if (_this.IsVisible) _this.Validate();
                break;
        }
    }

    void Validators_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
            foreach (var item in e.OldItems)
                RemoveLogicalChild((Validator)item);
        if (e.NewItems != null)
            foreach (var item in e.NewItems)
                AddLogicalChild((Validator)item);
    }

    public bool Validate()
    {
        if (IsEnabled)
        {
            object? value = GetValidatableValue?.Invoke();
            foreach (var validator in Validators)
            {
                if (validator.IsEnabled && !validator.Check(value))
                {
                    SetError(validator.GetErrorMessage(Title));
                    return false;
                }
            }
        }
        IsVisible = false;
        return true;
    }

    public void SetError(string? message)
    {
        Text = message ?? Strings.UnknownError;
        IsVisible = true;
    }
}
