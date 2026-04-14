using Neo;
using Neo.SmartContract;
using Neo.Wallets;
using NeoOrder.OneGate.Controls.Views.Validation;
using NeoOrder.OneGate.Properties;

namespace NeoOrder.OneGate.Controls.Popups;

public partial class PickAddressPopup : MyPopup<string?>
{
    readonly ProtocolSettings protocolSettings;

    public string Message { get; set { field = value; OnPropertyChanged(); } } = Strings.PickAddressText;
    public string[] Addresses { get; set { field = value; OnPropertyChanged(); } }

    public PickAddressPopup(ProtocolSettings protocolSettings, IWalletProvider walletProvider)
    {
        this.protocolSettings = protocolSettings;
        Addresses = walletProvider.GetWallet()!.GetAccounts().Select(p => p.Address).ToArray();
        InitializeComponent();
    }

    void OnValidateAddress(object sender, CustomValidationEventArgs e)
    {
        string address = (string)e.Value!;
        try
        {
            address.ToScriptHash(protocolSettings.AddressVersion);
        }
        catch
        {
            e.IsValid = false;
        }
    }

    async void OnSubmit(object sender, EventArgs e)
    {
        string address = radio1.IsChecked ? (string)pickerAddress.SelectedItem : entryAddress.Text;
        await CloseAsync(address);
    }

    async void OnCancel(object sender, EventArgs e)
    {
        await CloseAsync(null);
    }
}
