using Xunit;

namespace HoldKeyPlugin.Tests;

public sealed class PackagingTests
{
    [Fact]
    public void PluginProject_UsesPortableOfficialPackageReference()
    {
        var projectPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "../../../../HoldKeyPlugin/HoldKeyPlugin.csproj"));
        var project = File.ReadAllText(projectPath);

        Assert.Contains("PackageReference Include=\"OpenTabletDriver.Plugin\" Version=\"0.6.7\"", project);
        Assert.DoesNotContain("/Applications/", project);
        Assert.Contains("<Version>1.0.0.0</Version>", project);
    }
}
