using HoldKeyPlugin;
using Xunit;

namespace HoldKeyPlugin.Tests;

public sealed class MacQuartzKeyEmitterTests
{
    [Fact]
    public void NativeBackend_CreatesAndReleasesQuartzEventOnMacOS()
    {
        if (!OperatingSystem.IsMacOS())
            return;

        var backend = new NativeQuartzEventBackend();
        var keyEvent = backend.CreateKeyboardEvent(0x06, true);

        Assert.NotEqual(IntPtr.Zero, keyEvent);
        backend.Release(keyEvent);
    }

    [Fact]
    public void KeyDown_PostsNativeEventWithAutorepeat()
    {
        var backend = new RecordingQuartzBackend();
        var emitter = new MacQuartzKeyEmitter(backend);

        emitter.KeyDown("Z", true);

        Assert.Contains("create:6:True", backend.Events);
        Assert.Contains("integer:8:1", backend.Events);
        Assert.Contains("post:0", backend.Events);
        Assert.Equal("release", backend.Events[^1]);
    }

    [Fact]
    public void ModifierFlags_AreAppliedUntilModifierIsReleased()
    {
        var backend = new RecordingQuartzBackend();
        var emitter = new MacQuartzKeyEmitter(backend);

        emitter.KeyDown("Application", false);
        emitter.KeyDown("Space", false);
        emitter.KeyUp("Space");
        emitter.KeyUp("Application");

        Assert.Equal(3, backend.Events.Count(entry => entry == "flags:1048576"));
        Assert.Equal("flags:0", backend.Events[^3]);
    }

    [Fact]
    public void UnsupportedKey_IsRejectedBeforeCreatingEvent()
    {
        var backend = new RecordingQuartzBackend();
        var emitter = new MacQuartzKeyEmitter(backend);

        Assert.Throws<ArgumentException>(() => emitter.KeyDown("UnknownKey", false));
        Assert.Empty(backend.Events);
    }

    private sealed class RecordingQuartzBackend : IQuartzEventBackend
    {
        public List<string> Events { get; } = [];

        public IntPtr CreateKeyboardEvent(ushort keyCode, bool isDown)
        {
            Events.Add($"create:{keyCode}:{isDown}");
            return new IntPtr(123);
        }

        public void SetFlags(IntPtr keyEvent, ulong flags) => Events.Add($"flags:{flags}");
        public void SetIntegerField(IntPtr keyEvent, int field, long value) => Events.Add($"integer:{field}:{value}");
        public void Post(uint tap, IntPtr keyEvent) => Events.Add($"post:{tap}");
        public void Release(IntPtr keyEvent) => Events.Add("release");
    }
}
