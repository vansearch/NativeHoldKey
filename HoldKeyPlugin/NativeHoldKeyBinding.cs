using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;

namespace HoldKeyPlugin;

[PluginName("Native Hold Key"), SupportedPlatform(PluginPlatform.MacOS)]
public sealed class NativeHoldKeyBinding : IStateBinding, IDisposable
{
    private readonly IHoldKeyController controller;

    public NativeHoldKeyBinding() : this(
        new HoldKeyController(new MacQuartzKeyEmitter(), new SystemRepeatTimer()))
    {
    }

    public NativeHoldKeyBinding(IHoldKeyController controller)
    {
        this.controller = controller;
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
            controller.Press(
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
            controller.Release();
        }
        catch (Exception exception)
        {
            Log.WriteNotify("Native Hold Key", exception.Message);
        }
    }

    public void Dispose() => controller.Dispose();

    public override string ToString() => $"Native Hold Key: {Keys}";
}
