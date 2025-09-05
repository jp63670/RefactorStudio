using RefactorStudio.Core.Models;
using RefactorStudio.Core.Services;

namespace RefactorStudio.Tests;

public class RecipeRunnerYamlTests
{
    private class StubAdapter : IModelAdapter
    {
        public Task<string> ExecuteAsync(string prompt) => Task.FromResult($"{prompt}-result");
    }

    [Fact]
    public async Task RunAsync_WritesOutputs()
    {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var sample = Path.Combine(repoRoot, "RefactorStudio.Recipes", "samples", "echo.yaml");
        var tempRecipe = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".yaml");
        File.Copy(sample, tempRecipe, true);
        var outputRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var runner = new RecipeRunnerYaml(new StubAdapter());

        IReadOnlyList<string> outputs = Array.Empty<string>();
        try
        {
            outputs = await runner.RunAsync(tempRecipe, outputRoot);
            Assert.True(Directory.Exists(outputRoot));
            Assert.NotEmpty(outputs);
            foreach (var file in outputs)
            {
                Assert.True(File.Exists(file));
                Assert.True(new FileInfo(file).Length > 0);
            }
        }
        finally
        {
            if (Directory.Exists(outputRoot))
                Directory.Delete(outputRoot, true);
            if (File.Exists(tempRecipe))
                File.Delete(tempRecipe);
        }
    }

    [Fact]
    public async Task RunAsync_BadPath_Throws()
    {
        var runner = new RecipeRunnerYaml(new StubAdapter());
        var outputRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var badPath = Path.Combine(Path.GetTempPath(), "missing.yaml");
        var ex = await Assert.ThrowsAsync<FileNotFoundException>(() => runner.RunAsync(badPath, outputRoot));
        Assert.Contains("Recipe file not found", ex.Message);
    }
}
