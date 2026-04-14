using System.Globalization;

namespace NeoOrder.OneGate.Controls.Converters;

class StringSelectConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is not string alphabet || alphabet.Length < 2) return null;
        return value switch
        {
            false => alphabet[0..1],
            true => alphabet[1..2],
            _ => null
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string s) return null;
        if (parameter is not string alphabet || alphabet.Length < 2) return null;
        return alphabet.IndexOf(s) switch
        {
            0 => false,
            1 => true,
            _ => null
        };
    }
}
