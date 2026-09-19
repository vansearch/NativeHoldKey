using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.DependencyInjection;
using OpenTabletDriver.Plugin.Platform.Keyboard;
using OpenTabletDriver.Plugin.Tablet;

namespace HoldKeyPlugin;

[PluginName("Native Hold Key"), SupportedPlatform(PluginPlatform.MacOS | PluginPlatform.Windows | PluginPlatform.Linux)]
public sealed class NativeHoldKeyBinding : IStateBinding, IDisposable
{
    private IHoldKeyController? controller;

    public NativeHoldKeyBinding()
    {
    }

    public NativeHoldKeyBinding(IHoldKeyController controller)
    {
        this.controller = controller;
    }

    [Resolved]
    public IVirtualKeyboard? Keyboard { private get; set; }

    [OnDependencyLoad]
    public void Initialize()
    {
        if (controller is not null)
            return;

        try
        {
            var emitter = PlatformKeyEmitterFactory.Create(
                PlatformKeyEmitterFactory.CurrentPlatform,
                Keyboard);
            controller = new HoldKeyController(emitter, new SystemRepeatTimer());
        }
        catch (Exception exception)
        {
            Log.WriteNotify("Native Hold Key", exception.Message);
        }
    }

    [Property("Keys (example: Z or Application+Space)")]
    public string Keys { get; set; } = "Z";

    [Property("Repeat interval (ms)")]
    public int RepeatIntervalMilliseconds { get; set; } = 40;

    [Property("Safety release (seconds)")]
    public int SafetyReleaseSeconds { get; set; } = 15;

    public void Press(TabletReference tablet, IDeviceReport report)
    {
        try
        {
            GetController().Press(
                Keys,
                TimeSpan.FromMilliseconds(Math.Clamp(RepeatIntervalMilliseconds, 15, 1000)),
                TimeSpan.FromSeconds(Math.Clamp(SafetyReleaseSeconds, 1, 300)));
        }
        catch (Exception exception)
        {
            Log.WriteNotify("Native Hold Key", exception.Message);
        }
    }

    public void Release(TabletReference tablet, IDeviceReport report)
    {
        try
        {
            GetController().Release();
        }
        catch (Exception exception)
        {
            Log.WriteNotify("Native Hold Key", exception.Message);
        }
    }

    private IHoldKeyController GetController()
    {
        Initialize();
        return controller
            ?? throw new InvalidOperationException("A virtual keyboard is unavailable on this platform.");
    }

    public void Dispose() => controller?.Dispose();

    public override string ToString() => $"Native Hold Key: {Keys}";
}
