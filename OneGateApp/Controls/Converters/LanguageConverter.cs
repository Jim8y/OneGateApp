using NeoOrder.OneGate.Properties;
using System.Globalization;

namespace NeoOrder.OneGate.Controls.Converters;

class LanguageConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            null or "" => Strings.SystemLanguage,
            string language => new CultureInfo(language).NativeName,
            IEnumerable<string> languages => string.Join(", ", languages.Select(p => new CultureInfo(p).NativeName)),
            _ => null,
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
