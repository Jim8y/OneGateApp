using Neo.Network.P2P;
using Neo.SmartContract;
using Neo.Wallets;
using System.Security.Cryptography;

namespace NeoOrder.OneGate;

static class Workarounds
{
#if WINDOWS
    // This fixed the issue where the Picker's Title would not be shown as a placeholder on Windows.
    // By setting the PlaceholderText of the native control to the Title of the Picker, we ensure that it behaves as expected on Windows.
    // We can remove this workaround once the issue is resolved in .NET MAUI.
    // See https://github.com/dotnet/maui/pull/33007
    public static void FixPickerHandler()
    {
        Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping(nameof(Picker.Title), (handler, view) =>
        {
            if (handler.PlatformView is not null && view is Picker picker && !string.IsNullOrWhiteSpace(picker.Title))
            {
                handler.PlatformView.PlaceholderText = picker.Title;
                picker.Title = null;
            }
        });
    }
#endif

    // This method is a workaround for the fact that Neo.Cryptography.Crypto.Sign() does not work on Android.
    // Because it only sets the D parameter of the ECParameters, but not the Q parameter, which is required for signing on Android.
    // The issue has been resolved in the Neo master branch, but it has not been ported to the Neo master-n3 branch yet, which is what we are using in this project.
    // See https://github.com/neo-project/neo/pull/4519
    public static byte[] Sign(byte[] message, KeyPair key)
    {
        var curve = ECCurve.NamedCurves.nistP256;
        byte[] pubkey = key.PublicKey.EncodePoint(false);
        using var ecdsa = ECDsa.Create(new ECParameters
        {
            Curve = curve,
            D = key.PrivateKey,
            Q = new ECPoint
            {
                X = pubkey[1..33],
                Y = pubkey[33..]
            }
        });
        return ecdsa.SignData(message, HashAlgorithmName.SHA256);
    }

    // Same issue as above, but for signing with a wallet.
    // This can be removed once the issue is resolved in the Neo master-n3 branch.
    // See https://github.com/neo-project/neo/pull/4519
    public static bool SignWithWorkaround(this Wallet wallet, ContractParametersContext context)
    {
        if (context.Network != wallet.ProtocolSettings.Network) return false;
        var fSuccess = false;
        foreach (var scriptHash in context.ScriptHashes)
        {
            var account = wallet.GetAccount(scriptHash);
            if (account is null || account.Lock) continue;
            var multiSigContract = account.Contract;
            if (multiSigContract != null && Neo.SmartContract.Helper.IsMultiSigContract(multiSigContract.Script, out int m, out Neo.Cryptography.ECC.ECPoint[]? points))
            {
                foreach (var point in points)
                {
                    account = wallet.GetAccount(point);
                    if (account?.HasKey != true) continue;
                    var key = account.GetKey()!;
                    var signature = Sign(context.Verifiable.GetSignData(context.Network), key);
                    var ok = context.AddSignature(multiSigContract, key.PublicKey, signature);
                    if (ok) m--;
                    fSuccess |= ok;
                    if (context.Completed || m <= 0) break;
                }
                continue;
            }
            else if (account.HasKey)
            {
                var key = account.GetKey()!;
                var signature = Sign(context.Verifiable.GetSignData(context.Network), key);
                fSuccess |= context.AddSignature(account.Contract!, key.PublicKey, signature);
                continue;
            }
        }
        return fSuccess;
    }
}
