using Neo;
using Neo.Wallets;
using NeoOrder.OneGate.Data;
using NeoOrder.OneGate.Pages;
using NeoOrder.OneGate.Services;
using Xunit;

public class PasswordPersistenceTests
{
    [Fact]
    public async Task Successful_page_submission_persists_new_password_before_success()
    {
        using var fixture = new WalletFixture();
        var settings = new ApplicationDbContext();
        var page = new ChangePasswordPage(new ScreenSecurity(), settings, fixture)
        {
            CurrentPassword = WalletFixture.OldPassword,
            Password = WalletFixture.NewPassword
        };
        await page.SubmitForTestAsync();

        Wallet reopened = Wallet.Open(fixture.Wallet.Path, null, ProtocolSettings.Default)!;
        Assert.True(reopened.VerifyPassword(WalletFixture.NewPassword));
        Assert.False(reopened.VerifyPassword(WalletFixture.OldPassword));
        Assert.False(settings.Settings.HasBiometricCredential);
    }

    [Fact]
    public void Wrong_password_does_not_write_or_change_wallet()
    {
        using var fixture = new WalletFixture();
        byte[] before = File.ReadAllBytes(fixture.Wallet.Path);
        bool persisted = false;
        Assert.False(WalletPasswordService.ChangePassword(fixture.Wallet, "incorrect", WalletFixture.NewPassword, _ => persisted = true));
        Assert.False(persisted);
        Assert.Equal(before, File.ReadAllBytes(fixture.Wallet.Path));
        Assert.True(fixture.Wallet.VerifyPassword(WalletFixture.OldPassword));
    }

    [Fact]
    public void Failed_save_preserves_original_file_and_live_password()
    {
        using var fixture = new WalletFixture();
        byte[] before = File.ReadAllBytes(fixture.Wallet.Path);
        Assert.Throws<IOException>(() => WalletPasswordService.ChangePassword(fixture.Wallet,
            WalletFixture.OldPassword, WalletFixture.NewPassword, _ => throw new IOException("Simulated full disk")));

        Assert.Equal(before, File.ReadAllBytes(fixture.Wallet.Path));
        Assert.True(fixture.Wallet.VerifyPassword(WalletFixture.OldPassword));
        Assert.False(fixture.Wallet.VerifyPassword(WalletFixture.NewPassword));
        Wallet reopened = Wallet.Open(fixture.Wallet.Path, null, ProtocolSettings.Default)!;
        Assert.True(reopened.VerifyPassword(WalletFixture.OldPassword));
        Assert.Single(Directory.GetFiles(Path.GetDirectoryName(fixture.Wallet.Path)!));
    }
}

sealed class WalletFixture : IWalletProvider, IDisposable
{
    public const string OldPassword = "old-regression-password";
    public const string NewPassword = "new-regression-password";
    readonly string directory = Directory.CreateTempSubdirectory("onegate-password-test-").FullName;
    public Wallet Wallet { get; }
    public event EventHandler<Wallet?>? WalletChanged { add { } remove { } }
    public WalletFixture()
    {
        Wallet = Neo.Wallets.Wallet.Create("Disposable regression wallet", Path.Combine(directory, "wallet.json"), OldPassword, ProtocolSettings.Default)!;
        Wallet.CreateAccount();
        Wallet.Save();
    }
    public Wallet? GetWallet() => Wallet;
    public void Dispose() => Directory.Delete(directory, true);
}
