using System.Globalization;

namespace NeoOrder.OneGate.Controls.Converters;

class IsEqualMultiValueConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2) return true;
        if (values.All(p => p is null)) return true;
        if (values.Any(p => p is null)) return false;
        for (int i = 1; i < values.Length; i++)
            if (!values[0].Equals(values[i]))
                return false;
        return true;
    }

    public object[]? ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return null;
    }
}
