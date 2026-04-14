using Neo.Extensions;
using System.Globalization;

namespace NeoOrder.OneGate.Controls.Converters;

class TimestampConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            int s => DateTimeOffset.FromUnixTimeSeconds(s).LocalDateTime,
            uint s => DateTimeOffset.FromUnixTimeSeconds(s).LocalDateTime,
            long ms => DateTimeOffset.FromUnixTimeMilliseconds(ms).LocalDateTime,
            ulong ms => DateTimeOffset.FromUnixTimeMilliseconds((long)ms).LocalDateTime,
            string ms => DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(ms)).LocalDateTime,
            _ => null
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DateTime time) return null;
        return time.ToLocalTime().ToTimestampMS();
    }
}
