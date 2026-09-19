using Xunit;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;

namespace HoldKeyPlugin.Tests;

public sealed class PackagingTests
{
    [Fact]
    public void NativeBinding_IsDeclaredForDesktopPlatforms()
    {
        var attribute = Assert.Single(
            typeof(NativeHoldKeyBinding).GetCustomAttributes(typeof(SupportedPlatformAttribute), false));

        Assert.Equal(
            PluginPlatform.MacOS | PluginPlatform.Windows | PluginPlatform.Linux,
            ((SupportedPlatformAttribute)attribute).Platform);
    }

    [Fact]
    public void PluginProject_UsesPortableOfficialPackageReference()
    {
        var projectPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "../../../../HoldKeyPlugin/HoldKeyPlugin.csproj"));
        var project = File.ReadAllText(projectPath);

        Assert.Contains("PackageReference Include=\"OpenTabletDriver.Plugin\" Version=\"0.6.7\"", project);
        Assert.DoesNotContain("/Applications/", project);
        Assert.Contains("<Version>1.1.0.0</Version>", project);
    }
}
