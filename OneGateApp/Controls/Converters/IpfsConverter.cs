using System.Globalization;

namespace NeoOrder.OneGate.Controls.Converters;

class IpfsConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        Uri? uri = value switch
        {
            Uri u => u,
            string s => new Uri(s),
            _ => null
        };
        if (uri is null) return null;
        return uri.Scheme switch
        {
            "ipfs" => $"https://ipfs.io/ipfs/{uri.OriginalString[7..]}",
            _ => uri.ToString(),
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
