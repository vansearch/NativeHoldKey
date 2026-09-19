using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Platform.Keyboard;

namespace HoldKeyPlugin;

public static class PlatformKeyEmitterFactory
{
    public static PluginPlatform CurrentPlatform
    {
        get
        {
            if (OperatingSystem.IsMacOS())
                return PluginPlatform.MacOS;
            if (OperatingSystem.IsWindows())
                return PluginPlatform.Windows;
            if (OperatingSystem.IsLinux())
                return PluginPlatform.Linux;

            return PluginPlatform.Unknown;
        }
    }

    public static IKeyEmitter Create(PluginPlatform platform, IVirtualKeyboard? keyboard) => platform switch
    {
        PluginPlatform.MacOS => new MacQuartzKeyEmitter(),
        PluginPlatform.Windows or PluginPlatform.Linux => new OpenTabletDriverKeyEmitter(
            keyboard ?? throw new InvalidOperationException(
                "OpenTabletDriver did not provide a virtual keyboard for this platform.")),
        _ => throw new PlatformNotSupportedException(
            $"Native Hold Key does not support platform '{platform}'.")
    };
}
