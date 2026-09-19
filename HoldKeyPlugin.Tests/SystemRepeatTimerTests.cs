using HoldKeyPlugin;
using Xunit;

namespace HoldKeyPlugin.Tests;

public sealed class SystemRepeatTimerTests
{
    [Fact]
    public void Start_InvokesRepeatAndSafetyCallbacks()
    {
        using var timer = new SystemRepeatTimer();
        using var repeated = new ManualResetEventSlim();
        using var released = new ManualResetEventSlim();

        timer.Start(
            TimeSpan.FromMilliseconds(10),
            TimeSpan.FromMilliseconds(40),
            repeated.Set,
            released.Set);

        Assert.True(repeated.Wait(TimeSpan.FromSeconds(1)));
        Assert.True(released.Wait(TimeSpan.FromSeconds(1)));
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(10, 0)]
    public void Start_RejectsNonPositiveIntervals(int repeatMilliseconds, int holdMilliseconds)
    {
        using var timer = new SystemRepeatTimer();

        Assert.Throws<ArgumentOutOfRangeException>(() => timer.Start(
            TimeSpan.FromMilliseconds(repeatMilliseconds),
            TimeSpan.FromMilliseconds(holdMilliseconds),
            () => { },
            () => { }));
    }

    [Fact]
    public void Stop_PreventsPendingCallbacks()
    {
        using var timer = new SystemRepeatTimer();
        var calls = 0;
        timer.Start(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100), () => calls++, () => calls++);

        timer.Stop();
        Thread.Sleep(150);

        Assert.Equal(0, calls);
    }
}
