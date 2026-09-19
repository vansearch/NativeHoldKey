using OpenTabletDriver.Plugin;
using Xunit;

namespace HoldKeyPlugin.Tests;

public sealed class PlatformKeyEmitterFactoryTests
{
    [Fact]
    public void MacOs_UsesQuartzEmitter()
    {
        var emitter = PlatformKeyEmitterFactory.Create(PluginPlatform.MacOS, null);

        Assert.IsType<MacQuartzKeyEmitter>(emitter);
    }

    [Theory]
    [InlineData(PluginPlatform.Windows)]
    [InlineData(PluginPlatform.Linux)]
    public void WindowsAndLinux_UseDriverVirtualKeyboard(PluginPlatform platform)
    {
        var keyboard = new OpenTabletDriverKeyEmitterTests.RecordingVirtualKeyboard(["Z"]);

        var emitter = PlatformKeyEmitterFactory.Create(platform, keyboard);

        Assert.IsType<OpenTabletDriverKeyEmitter>(emitter);
    }

    [Theory]
    [InlineData(PluginPlatform.Windows)]
    [InlineData(PluginPlatform.Linux)]
    public void WindowsAndLinux_RequireDriverVirtualKeyboard(PluginPlatform platform)
    {
        Assert.Throws<InvalidOperationException>(() =>
            PlatformKeyEmitterFactory.Create(platform, null));
    }

    [Fact]
    public void UnsupportedPlatform_IsRejected()
    {
        Assert.Throws<PlatformNotSupportedException>(() =>
            PlatformKeyEmitterFactory.Create(PluginPlatform.FreeBSD, null));
    }
}
