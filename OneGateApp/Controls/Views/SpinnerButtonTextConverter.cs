using System.Globalization;

namespace NeoOrder.OneGate.Controls.Views;

class SpinnerButtonTextConverter : IMultiValueConverter
{
    object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2) return "";
        if (values[0] is not string text) return "";
        if (values[1] is not bool isBusy) return "";
        return isBusy ? "" : text;
    }

    object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return null!;
    }
}
