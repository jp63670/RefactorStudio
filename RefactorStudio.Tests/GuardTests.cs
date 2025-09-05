using System.IO;
using Xunit;

namespace RefactorStudio.Tests;

public class GuardTests
{
    [Fact]
    public void MainPage_ShouldNotUseOutputsRelativePath()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var source = File.ReadAllText(Path.Combine(root, "RefactorStudio.App", "MainPage.xaml.cs"));
        Assert.DoesNotContain("Path.Combine(\"outputs\"", source);
    }

    [Fact]
    public void RecipeRunner_ShouldNotCombineStepNameRaw()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var source = File.ReadAllText(Path.Combine(root, "RefactorStudio.Core", "Services", "RecipeRunnerYaml.cs"));
        Assert.DoesNotContain("Path.Combine(outputRoot, $\"{step.Name}", source);
    }
}
