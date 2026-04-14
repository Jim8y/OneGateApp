using NeoOrder.OneGate.Properties;
using System.Numerics;

namespace NeoOrder.OneGate.Controls.Views.Validation;

public partial class Range<T> : Validator where T : IComparable<T>, IMinMaxValue<T>, IParsable<T>
{
    public static readonly BindableProperty MinimumProperty = BindableProperty.Create(nameof(Minimum), typeof(T), typeof(Range<T>), T.MinValue);
    public static readonly BindableProperty MaximumProperty = BindableProperty.Create(nameof(Maximum), typeof(T), typeof(Range<T>), T.MaxValue);
    public static readonly BindableProperty IsMinimumIncludedProperty = BindableProperty.Create(nameof(IsMinimumIncluded), typeof(bool), typeof(Range<T>), true);
    public static readonly BindableProperty IsMaximumIncludedProperty = BindableProperty.Create(nameof(IsMaximumIncluded), typeof(bool), typeof(Range<T>), true);

    public T Minimum
    {
        get => (T)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }
    public T Maximum
    {
        get => (T)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }
    public bool IsMinimumIncluded
    {
        get => (bool)GetValue(IsMinimumIncludedProperty);
        set => SetValue(IsMinimumIncludedProperty, value);
    }
    public bool IsMaximumIncluded
    {
        get => (bool)GetValue(IsMaximumIncludedProperty);
        set => SetValue(IsMaximumIncludedProperty, value);
    }

    public override bool Check(object? value)
    {
        T val;
        switch (value)
        {
            case T t:
                val = t;
                break;
            case string s when T.TryParse(s, null, out var t):
                val = t;
                break;
            default:
                return false;
        }
        int c = val.CompareTo(Minimum);
        if (c < 0 || (c == 0 && !IsMinimumIncluded)) return false;
        c = val.CompareTo(Maximum);
        if (c > 0 || (c == 0 && !IsMaximumIncluded)) return false;
        return true;
    }

    public override string GetErrorMessage(string? title)
    {
        return string.Format(ErrorMessage ?? Strings.DefaultOutOfRangeErrorMessage, title ?? Strings.Value);
    }
}
