using OpenTabletDriver.Plugin.Platform.Keyboard;

namespace HoldKeyPlugin;

public sealed class OpenTabletDriverKeyEmitter : IKeyEmitter
{
    private static readonly HashSet<string> Modifiers = new(StringComparer.OrdinalIgnoreCase)
    {
        "Application", "Command", "RightApplication",
        "Shift", "RightShift",
        "Alt", "Option", "RightAlt",
        "Control", "RightControl"
    };

    private static readonly Dictionary<string, string> Aliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Command"] = "Application",
        ["Option"] = "Alt",
        ["RightApplication"] = "Application"
    };

    private readonly IVirtualKeyboard keyboard;
    private readonly Dictionary<string, string> supportedKeys;

    public OpenTabletDriverKeyEmitter(IVirtualKeyboard keyboard)
    {
        this.keyboard = keyboard ?? throw new ArgumentNullException(nameof(keyboard));
        supportedKeys = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var key in keyboard.SupportedKeys)
            supportedKeys[key] = key;
    }

    public bool IsSupported(string key) => TryResolve(key, out _);

    public bool IsModifier(string key) => Modifiers.Contains(key);

    public void KeyDown(string key, bool isRepeat) => keyboard.Press(Resolve(key));

    public void KeyUp(string key) => keyboard.Release(Resolve(key));

    private string Resolve(string key)
    {
        if (TryResolve(key, out var resolved))
            return resolved;

        throw new ArgumentException($"Unsupported key: {key}", nameof(key));
    }

    private bool TryResolve(string key, out string resolved)
    {
        if (supportedKeys.TryGetValue(key, out resolved!))
            return true;

        return Aliases.TryGetValue(key, out var alias)
            && supportedKeys.TryGetValue(alias, out resolved!);
    }
}
