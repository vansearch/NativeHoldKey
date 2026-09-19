namespace HoldKeyPlugin;

public sealed class MacQuartzKeyEmitter : IKeyEmitter
{
    private const int KeyboardEventAutorepeat = 8;
    private const uint HidEventTap = 0;
    private readonly IQuartzEventBackend backend;
    private ulong currentFlags;

    public MacQuartzKeyEmitter() : this(new NativeQuartzEventBackend())
    {
    }

    public MacQuartzKeyEmitter(IQuartzEventBackend backend)
    {
        this.backend = backend;
    }

    public bool IsSupported(string key) => MacKeyMap.TryGetKeyCode(key, out _);
    public bool IsModifier(string key) => MacKeyMap.IsModifier(key);

    public void KeyDown(string key, bool isRepeat) => Post(key, true, isRepeat);
    public void KeyUp(string key) => Post(key, false, false);

    private void Post(string key, bool isDown, bool isRepeat)
    {
        if (!MacKeyMap.TryGetKeyCode(key, out var keyCode))
            throw new ArgumentException($"Unsupported key: {key}", nameof(key));

        UpdateFlags(key, isDown);
        var keyEvent = backend.CreateKeyboardEvent(keyCode, isDown);
        if (keyEvent == IntPtr.Zero)
            throw new InvalidOperationException("macOS could not create a keyboard event.");

        try
        {
            backend.SetFlags(keyEvent, currentFlags);
            if (isRepeat)
                backend.SetIntegerField(keyEvent, KeyboardEventAutorepeat, 1);
            backend.Post(HidEventTap, keyEvent);
        }
        finally
        {
            backend.Release(keyEvent);
        }
    }

    private void UpdateFlags(string key, bool isDown)
    {
        var flag = key.ToLowerInvariant() switch
        {
            "shift" or "rightshift" => 0x0002_0000UL,
            "control" or "rightcontrol" => 0x0004_0000UL,
            "alt" or "option" or "rightalt" => 0x0008_0000UL,
            "application" or "command" or "rightapplication" => 0x0010_0000UL,
            _ => 0UL
        };

        if (isDown)
            currentFlags |= flag;
        else
            currentFlags &= ~flag;
    }
}
