using HoldKeyPlugin;
using Xunit;

namespace HoldKeyPlugin.Tests;

public sealed class NativeHoldKeyBindingTests
{
    [Fact]
    public void Press_ClampsConfigurationAndDelegatesToController()
    {
        var controller = new RecordingController();
        using var binding = new NativeHoldKeyBinding(controller)
        {
            Keys = "Application+Space",
            RepeatIntervalMilliseconds = 1,
            SafetyReleaseSeconds = 999
        };

        binding.Press(null!, null!);

        Assert.Equal("Application+Space", controller.Keys);
        Assert.Equal(TimeSpan.FromMilliseconds(15), controller.RepeatInterval);
        Assert.Equal(TimeSpan.FromSeconds(300), controller.MaximumHold);
    }

    [Fact]
    public void ReleaseAndDispose_DelegateToController()
    {
        var controller = new RecordingController();
        var binding = new NativeHoldKeyBinding(controller);

        binding.Release(null!, null!);
        binding.Dispose();

        Assert.Equal(1, controller.ReleaseCalls);
        Assert.True(controller.Disposed);
    }

    private sealed class RecordingController : IHoldKeyController
    {
        public string? Keys { get; private set; }
        public TimeSpan RepeatInterval { get; private set; }
        public TimeSpan MaximumHold { get; private set; }
        public int ReleaseCalls { get; private set; }
        public bool Disposed { get; private set; }

        public bool IsPressed => Keys is not null;

        public void Press(string combination, TimeSpan repeatInterval, TimeSpan maximumHold)
        {
            Keys = combination;
            RepeatInterval = repeatInterval;
            MaximumHold = maximumHold;
        }

        public void Release() => ReleaseCalls++;
        public void Dispose() => Disposed = true;
    }
}
