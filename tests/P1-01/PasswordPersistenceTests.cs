using Neo;
using Neo.Wallets;
using NeoOrder.OneGate.Data;
using NeoOrder.OneGate.Pages;
using NeoOrder.OneGate.Services;
using NeoOrder.OneGate.Properties;
using System.Reflection;
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

    [Fact]
    public async Task Failed_page_save_shows_generic_localized_error_without_private_path()
    {
        using var fixture = new WalletFixture();
        // A directory at the wallet's destination deterministically rejects the
        // final atomic replacement on every platform, without touching real data.
        File.Move(fixture.Wallet.Path, fixture.Wallet.Path + ".original");
        Directory.CreateDirectory(fixture.Wallet.Path);
        var settings = new ApplicationDbContext();
        var page = new ChangePasswordPage(new ScreenSecurity(), settings, fixture)
        {
            CurrentPassword = WalletFixture.OldPassword,
            Password = WalletFixture.NewPassword
        };
        await page.SubmitForTestAsync();
        Assert.Equal(Strings.WalletPasswordSaveFailed, page.ErrorForTest);
        Assert.DoesNotContain(fixture.Wallet.Path, page.ErrorForTest!);
        Assert.True(settings.Settings.HasBiometricCredential);
        Assert.True(fixture.Wallet.VerifyPassword(WalletFixture.OldPassword));
    }

    [Fact]
    public async Task Password_service_does_not_lock_public_wallet_object()
    {
        using var fixture = new WalletFixture();
        using var releaseMonitor = new ManualResetEventSlim();
        var monitorEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Task holder = Task.Run(() =>
        {
            lock (fixture.Wallet)
            {
                monitorEntered.SetResult();
                releaseMonitor.Wait();
            }
        });
        await monitorEntered.Task;
        Task<bool> changing = Task.Run(() => WalletPasswordService.ChangePassword(fixture.Wallet,
                WalletFixture.OldPassword, WalletFixture.NewPassword, _ => { }));
        bool completedWhileWalletLocked;
        try
        {
            completedWhileWalletLocked = await Task.WhenAny(changing, Task.Delay(TimeSpan.FromSeconds(8))) == changing;
        }
        finally
        {
            releaseMonitor.Set();
            await holder;
        }
        Assert.True(await changing);
        Assert.True(completedWhileWalletLocked, "An unrelated monitor on the wallet must not block the password service.");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Cleanup_failure_does_not_replace_original_save_exception(bool denied)
    {
        using var fixture = new WalletFixture();
        var cleanup = typeof(WalletPasswordService).GetMethod("CleanupTemporaryFile", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.NotNull(cleanup);
        var original = new IOException("Original staging write failure");
        Action<string> failingDelete = _ => throw (denied
            ? new UnauthorizedAccessException("Cleanup denied")
            : new IOException("Cleanup locked"));
        Exception actual = Assert.Throws<IOException>(() => WalletPasswordService.ChangePassword(fixture.Wallet,
            WalletFixture.OldPassword, WalletFixture.NewPassword, _ =>
            {
                try { throw original; }
                finally { cleanup.Invoke(null, ["temporary-regression-file", failingDelete]); }
            }));
        Assert.Same(original, actual);
        Assert.True(fixture.Wallet.VerifyPassword(WalletFixture.OldPassword));
    }

    [Fact]
    public async Task Private_gate_still_serializes_changes_for_the_same_wallet()
    {
        using var fixture = new WalletFixture();
        using var releaseFirstSave = new ManualResetEventSlim();
        var firstSaving = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var secondSaving = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Task<bool> first = Task.Run(() => WalletPasswordService.ChangePassword(fixture.Wallet,
            WalletFixture.OldPassword, WalletFixture.NewPassword, wallet =>
            {
                firstSaving.SetResult();
                releaseFirstSave.Wait();
                wallet.Save();
            }));
        await firstSaving.Task;
        const string finalPassword = "final-regression-password";
        Task<bool> second = Task.Run(() => WalletPasswordService.ChangePassword(fixture.Wallet,
            WalletFixture.NewPassword, finalPassword, wallet => { secondSaving.SetResult(); wallet.Save(); }));
        bool overlapped;
        try
        {
            overlapped = await Task.WhenAny(secondSaving.Task, Task.Delay(500)) == secondSaving.Task;
        }
        finally { releaseFirstSave.Set(); }
        Assert.True(await first);
        Assert.True(await second);
        Assert.False(overlapped, "The next password change must wait for the first file save.");
        Assert.True(Wallet.Open(fixture.Wallet.Path, null, ProtocolSettings.Default)!.VerifyPassword(finalPassword));
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
