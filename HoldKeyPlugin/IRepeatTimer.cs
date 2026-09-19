namespace HoldKeyPlugin;

public interface IRepeatTimer : IDisposable
{
    void Start(
        TimeSpan repeatInterval,
        TimeSpan maximumHold,
        Action onRepeat,
        Action onSafetyTimeout);

    void Stop();
}
