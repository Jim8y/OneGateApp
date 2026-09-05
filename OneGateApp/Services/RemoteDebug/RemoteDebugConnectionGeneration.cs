namespace NeoOrder.OneGate.Services.RemoteDebug;

// Access is serialized by RemoteDebugService.stateLock. A revoked attempt cannot
// publish either persistent trust or a live connection after a later toggle.
sealed class RemoteDebugConnectionGeneration : IDisposable
{
    CancellationTokenSource cancellation = new();
    long version;

    public readonly record struct Attempt(long Version, CancellationToken Token);

    public Attempt Capture() => new(version, cancellation.Token);

    public void Invalidate()
    {
        var previous = cancellation;
        cancellation = new();
        version++;
        previous.Cancel();
        previous.Dispose();
    }

    public void ThrowIfStale(Attempt attempt)
    {
        if (attempt.Version != version || attempt.Token.IsCancellationRequested)
            throw new OperationCanceledException("The remote-debug connection was revoked.", attempt.Token);
    }

    public void Dispose()
    {
        cancellation.Cancel();
        cancellation.Dispose();
    }
}
