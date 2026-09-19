using HoldKeyPlugin;
using Xunit;

namespace HoldKeyPlugin.Tests;

public sealed class HoldKeyControllerTests
{
    [Fact]
    public void Press_HoldsAllKeysInConfiguredOrder()
    {
        var emitter = new RecordingEmitter();
        var timer = new ManualRepeatTimer();
        using var controller = new HoldKeyController(emitter, timer);

        controller.Press("Application+Space", TimeSpan.FromMilliseconds(40), TimeSpan.FromSeconds(10));

        Assert.Equal(["down:Application:False", "down:Space:False"], emitter.Events);
        Assert.True(controller.IsPressed);
    }

    [Fact]
    public void Repeat_EmitsAutorepeatForNonModifierKeysOnly()
    {
        var emitter = new RecordingEmitter();
        var timer = new ManualRepeatTimer();
        using var controller = new HoldKeyController(emitter, timer);
        controller.Press("Application+Z", TimeSpan.FromMilliseconds(40), TimeSpan.FromSeconds(10));

        timer.FireRepeat();

        Assert.Equal("down:Z:True", emitter.Events[^1]);
        Assert.DoesNotContain("down:Application:True", emitter.Events);
    }

    [Fact]
    public void Release_ReleasesKeysInReverseOrderAndStopsTimer()
    {
        var emitter = new RecordingEmitter();
        var timer = new ManualRepeatTimer();
        using var controller = new HoldKeyController(emitter, timer);
        controller.Press("Application+Space", TimeSpan.FromMilliseconds(40), TimeSpan.FromSeconds(10));

        controller.Release();

        Assert.Equal(["up:Space", "up:Application"], emitter.Events.TakeLast(2));
        Assert.False(timer.IsRunning);
        Assert.False(controller.IsPressed);
    }

    [Fact]
    public void DuplicatePress_DoesNotSendDuplicateKeyDown()
    {
        var emitter = new RecordingEmitter();
        var timer = new ManualRepeatTimer();
        using var controller = new HoldKeyController(emitter, timer);

        controller.Press("Z", TimeSpan.FromMilliseconds(40), TimeSpan.FromSeconds(10));
        controller.Press("Z", TimeSpan.FromMilliseconds(40), TimeSpan.FromSeconds(10));

        Assert.Equal(["down:Z:False"], emitter.Events);
    }

    [Fact]
    public void SafetyTimeout_ReleasesHeldKeys()
    {
        var emitter = new RecordingEmitter();
        var timer = new ManualRepeatTimer();
        using var controller = new HoldKeyController(emitter, timer);
        controller.Press("Z", TimeSpan.FromMilliseconds(40), TimeSpan.FromSeconds(2));

        timer.FireSafetyTimeout();

        Assert.Equal("up:Z", emitter.Events[^1]);
        Assert.False(controller.IsPressed);
    }

    [Fact]
    public void Dispose_ReleasesHeldKeys()
    {
        var emitter = new RecordingEmitter();
        var timer = new ManualRepeatTimer();
        var controller = new HoldKeyController(emitter, timer);
        controller.Press("Z", TimeSpan.FromMilliseconds(40), TimeSpan.FromSeconds(10));

        controller.Dispose();

        Assert.Equal("up:Z", emitter.Events[^1]);
    }

    [Theory]
    [InlineData("")]
    [InlineData("+")]
    [InlineData("UnknownKey")]
    public void Press_RejectsInvalidCombinations(string keys)
    {
        var emitter = new RecordingEmitter();
        var timer = new ManualRepeatTimer();
        using var controller = new HoldKeyController(emitter, timer);

        Assert.Throws<ArgumentException>(() =>
            controller.Press(keys, TimeSpan.FromMilliseconds(40), TimeSpan.FromSeconds(10)));
        Assert.Empty(emitter.Events);
    }

    [Fact]
    public void Press_ReleasesPreviouslyPressedKeysWhenEmitterFails()
    {
        var emitter = new ThrowingEmitter("Space");
        var timer = new ManualRepeatTimer();
        using var controller = new HoldKeyController(emitter, timer);

        Assert.Throws<InvalidOperationException>(() =>
            controller.Press("Application+Space", TimeSpan.FromMilliseconds(40), TimeSpan.FromSeconds(10)));

        Assert.Equal(["down:Application", "up:Application"], emitter.Events);
        Assert.False(controller.IsPressed);
    }

    [Fact]
    public void Press_ReleasesKeysWhenTimerCannotStart()
    {
        var emitter = new RecordingEmitter();
        using var controller = new HoldKeyController(emitter, new ThrowingTimer());

        Assert.Throws<InvalidOperationException>(() =>
            controller.Press("Z", TimeSpan.FromMilliseconds(40), TimeSpan.FromSeconds(10)));

        Assert.Equal(["down:Z:False", "up:Z"], emitter.Events);
        Assert.False(controller.IsPressed);
    }

    private sealed class RecordingEmitter : IKeyEmitter
    {
        public List<string> Events { get; } = [];

        public bool IsSupported(string key) => MacKeyMap.TryGetKeyCode(key, out _);
        public bool IsModifier(string key) => MacKeyMap.IsModifier(key);
        public void KeyDown(string key, bool isRepeat) => Events.Add($"down:{key}:{isRepeat}");
        public void KeyUp(string key) => Events.Add($"up:{key}");
    }

    private sealed class ManualRepeatTimer : IRepeatTimer
    {
        private Action? repeat;
        private Action? safetyTimeout;

        public bool IsRunning { get; private set; }

        public void Start(TimeSpan repeatInterval, TimeSpan maximumHold, Action onRepeat, Action onSafetyTimeout)
        {
            IsRunning = true;
            repeat = onRepeat;
            safetyTimeout = onSafetyTimeout;
        }

        public void Stop()
        {
            IsRunning = false;
            repeat = null;
            safetyTimeout = null;
        }

        public void FireRepeat() => repeat?.Invoke();
        public void FireSafetyTimeout() => safetyTimeout?.Invoke();
        public void Dispose() => Stop();
    }

    private sealed class ThrowingEmitter(string failingKey) : IKeyEmitter
    {
        public List<string> Events { get; } = [];

        public bool IsSupported(string key) => MacKeyMap.TryGetKeyCode(key, out _);
        public bool IsModifier(string key) => MacKeyMap.IsModifier(key);
        public void KeyDown(string key, bool isRepeat)
        {
            if (key == failingKey)
                throw new InvalidOperationException("Synthetic failure");
            Events.Add($"down:{key}");
        }
        public void KeyUp(string key) => Events.Add($"up:{key}");
    }

    private sealed class ThrowingTimer : IRepeatTimer
    {
        public void Start(TimeSpan repeatInterval, TimeSpan maximumHold, Action onRepeat, Action onSafetyTimeout) =>
            throw new InvalidOperationException("Synthetic failure");
        public void Stop() { }
        public void Dispose() { }
    }
}
