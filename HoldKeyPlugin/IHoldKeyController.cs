namespace HoldKeyPlugin;

public interface IHoldKeyController : IDisposable
{
    bool IsPressed { get; }
    void Press(string combination, TimeSpan repeatInterval, TimeSpan maximumHold);
    void Release();
}
