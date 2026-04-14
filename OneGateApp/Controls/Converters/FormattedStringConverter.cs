using System.Globalization;
using System.Text.RegularExpressions;
using Match = System.Text.RegularExpressions.Match;

namespace NeoOrder.OneGate.Controls.Converters;

partial class FormattedStringConverter : IValueConverter
{
    [GeneratedRegex(@"\{(\d+)\}")]
    static partial Regex regex { get; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string format) return null;
        if (parameter is not Span[] spans) return null;
        var formattedString = new FormattedString();
        var matches = regex.Matches(format);
        int index = 0;
        foreach (Match match in matches)
        {
            if (index < match.Index)
                formattedString.Spans.Add(new Span { Text = format[index..match.Index] });
            formattedString.Spans.Add(spans[int.Parse(match.Groups[1].ValueSpan)]);
            index = match.Index + match.Length;
        }
        if (index < format.Length)
            formattedString.Spans.Add(new Span { Text = format[index..] });
        return formattedString;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
