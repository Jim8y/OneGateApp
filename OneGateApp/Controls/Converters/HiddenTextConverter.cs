using System.Globalization;

namespace NeoOrder.OneGate.Controls.Converters;

class HiddenTextConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is not [object text, bool show]) return null;
        return show ? text : parameter;
    }

    public object[]? ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return null;
    }
}
