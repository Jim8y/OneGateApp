using NeoOrder.OneGate.Data;
using System.Globalization;
using System.Text.Json;

namespace NeoOrder.OneGate.Controls.Converters;

class LocalizerConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string text) return null;
        var localizer = JsonSerializer.Deserialize<Dictionary<string, string>>(text)!;
        return localizer.Localize(parameter as string);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
