namespace HoldKeyPlugin;

public sealed class SystemRepeatTimer : IRepeatTimer
{
    private readonly object sync = new();
    private Timer? repeatTimer;
    private Timer? safetyTimer;

    public void Start(
        TimeSpan repeatInterval,
        TimeSpan maximumHold,
        Action onRepeat,
        Action onSafetyTimeout)
    {
        if (repeatInterval <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(repeatInterval));
        if (maximumHold <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(maximumHold));

        lock (sync)
        {
            StopCore();
            repeatTimer = new Timer(_ => onRepeat(), null, repeatInterval, repeatInterval);
            safetyTimer = new Timer(_ => onSafetyTimeout(), null, maximumHold, Timeout.InfiniteTimeSpan);
        }
    }

    public void Stop()
    {
        lock (sync)
            StopCore();
    }

    private void StopCore()
    {
        repeatTimer?.Dispose();
        safetyTimer?.Dispose();
        repeatTimer = null;
        safetyTimer = null;
    }

    public void Dispose() => Stop();
}
