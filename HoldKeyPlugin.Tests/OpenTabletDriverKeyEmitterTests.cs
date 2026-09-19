using OpenTabletDriver.Plugin.Platform.Keyboard;
using Xunit;

namespace HoldKeyPlugin.Tests;

public sealed class OpenTabletDriverKeyEmitterTests
{
    [Fact]
    public void KeyEvents_AreForwardedToTheDriverKeyboard()
    {
        var keyboard = new RecordingVirtualKeyboard(["Z", "Application", "Space"]);
        var emitter = new OpenTabletDriverKeyEmitter(keyboard);

        emitter.KeyDown("Z", false);
        emitter.KeyDown("Z", true);
        emitter.KeyUp("Z");

        Assert.Equal(["down:Z", "down:Z", "up:Z"], keyboard.Events);
    }

    [Fact]
    public void SupportedKeys_AreMatchedCaseInsensitively()
    {
        var keyboard = new RecordingVirtualKeyboard(["Z"]);
        var emitter = new OpenTabletDriverKeyEmitter(keyboard);

        Assert.True(emitter.IsSupported("z"));

        emitter.KeyDown("z", false);
        Assert.Equal("down:Z", Assert.Single(keyboard.Events));
    }

    [Theory]
    [InlineData("Application")]
    [InlineData("Shift")]
    [InlineData("RightShift")]
    [InlineData("Alt")]
    [InlineData("RightAlt")]
    [InlineData("Control")]
    [InlineData("RightControl")]
    public void PlatformModifiers_AreNotRepeated(string key)
    {
        var keyboard = new RecordingVirtualKeyboard([key]);
        var emitter = new OpenTabletDriverKeyEmitter(keyboard);

        Assert.True(emitter.IsModifier(key));
    }

    [Fact]
    public void UnsupportedKey_IsRejectedWithoutSendingAnEvent()
    {
        var keyboard = new RecordingVirtualKeyboard(["Z"]);
        var emitter = new OpenTabletDriverKeyEmitter(keyboard);

        Assert.Throws<ArgumentException>(() => emitter.KeyDown("Unknown", false));
        Assert.Empty(keyboard.Events);
    }

    internal sealed class RecordingVirtualKeyboard(IEnumerable<string> supportedKeys) : IVirtualKeyboard
    {
        public List<string> Events { get; } = [];
        public IEnumerable<string> SupportedKeys { get; } = supportedKeys;

        public void Press(string key) => Events.Add($"down:{key}");
        public void Release(string key) => Events.Add($"up:{key}");
        public void Press(IEnumerable<string> keys)
        {
            foreach (var key in keys)
                Press(key);
        }

        public void Release(IEnumerable<string> keys)
        {
            foreach (var key in keys)
                Release(key);
        }
    }
}
