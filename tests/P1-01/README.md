# P1-01 password persistence regression

Run `dotnet test tests/P1-01/PasswordPersistence.Tests.csproj`.

The test links the production change-password page and the actual Neo 3.10.0
wallet implementation. MAUI visual/navigation objects are replaced with small
test doubles. Every wallet is newly generated in a disposable temporary
directory; no real wallet, credentials, network or funds are used.

The essential invariant is that a success notification means a newly opened
wallet file accepts the new password and rejects the old password. Biometric
configuration must remain intact when a password change is not committed.
