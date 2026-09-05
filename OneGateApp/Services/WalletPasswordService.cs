using Neo.Wallets;
using Neo.Wallets.NEP6;
using System.Text;

namespace NeoOrder.OneGate.Services;

static class WalletPasswordService
{
    public static bool ChangePassword(Wallet wallet, string oldPassword, string newPassword)
        => ChangePassword(wallet, oldPassword, newPassword, SaveAtomically);

    internal static bool ChangePassword(Wallet wallet, string oldPassword, string newPassword, Action<NEP6Wallet> persist)
    {
        if (wallet is not NEP6Wallet nep6Wallet)
            throw new NotSupportedException("Password changes require a NEP-6 wallet.");

        // Keep concurrent password changes on this wallet from interleaving.
        lock (wallet)
        {
            if (!wallet.ChangePassword(oldPassword, newPassword)) return false;
            try
            {
                persist(nep6Wallet);
                return true;
            }
            catch
            {
                // The original file was not replaced. Keep the live wallet using
                // that same password, including when biometric settings remain.
                wallet.ChangePassword(newPassword, oldPassword);
                throw;
            }
        }
    }

    static void SaveAtomically(NEP6Wallet wallet)
    {
        string path = Path.GetFullPath(wallet.Path);
        string temporaryPath = Path.Combine(Path.GetDirectoryName(path)!, $".{Path.GetFileName(path)}.{Guid.NewGuid():N}.tmp");
        try
        {
            byte[] contents = Encoding.UTF8.GetBytes(wallet.ToJson().ToString());
            using (var file = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                file.Write(contents);
                file.Flush(flushToDisk: true);
            }
            // Staging beside the wallet keeps the rename on the same filesystem.
            File.Move(temporaryPath, path, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath)) File.Delete(temporaryPath);
        }
    }
}
