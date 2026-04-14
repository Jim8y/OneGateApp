using Neo;
using Neo.SmartContract;
using Neo.Wallets;
using System.Globalization;

namespace NeoOrder.OneGate.Controls.Converters;

class AddressConverter : IValueConverter
{
    readonly ProtocolSettings protocolSettings;

    public AddressConverter()
    {
        protocolSettings = Application.Current!.Handler.GetRequiredService<ProtocolSettings>();
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            UInt160 hash => hash.ToAddress(protocolSettings.AddressVersion),
            KeyPair key => Contract.CreateSignatureRedeemScript(key.PublicKey).ToScriptHash().ToAddress(protocolSettings.AddressVersion),
            _ => null
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string address) return null;
        if (targetType != typeof(UInt160)) return null;
        return address.ToScriptHash(protocolSettings.AddressVersion);
    }
}
