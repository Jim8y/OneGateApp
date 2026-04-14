using Neo.Wallets;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NeoOrder.OneGate.Models;

public partial class WalletCreationContext : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public string WalletName { get; set; } = "My Wallet";
    public string? Password { get; set { field = value; OnPropertyChanged(); } }
    public Mnemonic? Mnemonic { get; set { field = value; OnPropertyChanged(); } }
    public string? MnemonicPhrase => Mnemonic?.ToString();
    public IEnumerable<string>? ShuffledWords => Mnemonic?.Shuffle();
    public byte[]? PrivateKey { get; set; }
    public KeyPair[]? DerivedKeys { get; set; }

    void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
