using Neo;
using Neo.Wallets;
using NeoOrder.OneGate.Data;
using NeoOrder.OneGate.Properties;
using System.Globalization;
using Contact = NeoOrder.OneGate.Data.Contact;

namespace NeoOrder.OneGate.Controls.Converters;

class InWalletConverter : IValueConverter
{
    readonly ApplicationDbContext dbContext;
    readonly ProtocolSettings protocolSettings;
    readonly Wallet wallet;

    public InWalletConverter()
    {
        var handler = Application.Current!.Handler;
        dbContext = handler.GetRequiredService<ApplicationDbContext>();
        protocolSettings = handler.GetRequiredService<ProtocolSettings>();
        wallet = handler.GetRequiredService<IWalletProvider>().GetWallet()!;
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not UInt160 hash) return null;
        if (wallet.Contains(hash)) return wallet.Name;
        string address = hash.ToAddress(protocolSettings.AddressVersion);
        Contact? contact = dbContext.Contacts.FirstOrDefault(p => p.Address == address);
        if (contact is not null) return contact.Label;
        return Strings.UnknownAddress;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
