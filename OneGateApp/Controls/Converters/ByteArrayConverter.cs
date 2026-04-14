using System.Globalization;
using System.Text;

namespace NeoOrder.OneGate.Controls.Converters;

class ByteArrayConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not byte[] bytes) return null;
        return parameter switch
        {
            "base64" => System.Convert.ToBase64String(bytes),
            "hex" => System.Convert.ToHexString(bytes),
            "utf8" => Encoding.UTF8.GetString(bytes),
            _ => null
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string s) return null;
        return parameter switch
        {
            "base64" => System.Convert.FromBase64String(s),
            "hex" => System.Convert.FromHexString(s),
            "utf8" => Encoding.UTF8.GetBytes(s),
            _ => null
        };
    }
}
