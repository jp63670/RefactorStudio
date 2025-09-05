using RefactorStudio.Adapters;
using RefactorStudio.Core.Services;
using System.IO;

namespace RefactorStudio.Tests;

public class RecipeRunnerTests
{
    [Fact]
    public async Task RunAsync_CreatesOutputsPerStep()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var sample = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "RefactorStudio.Recipes", "samples", "echo.yaml"));
        var recipePath = Path.Combine(tempDir, "echo.yaml");
        File.Copy(sample, recipePath);

        var outputDir = Path.Combine(tempDir, "outputs", "echo");
        Directory.CreateDirectory(outputDir);

        IRecipeRunner runner = new RecipeRunnerYaml(new EchoAdapter());
        var written = await runner.RunAsync(recipePath, outputDir);

        try
        {
            Assert.True(Directory.Exists(outputDir));
            Assert.NotEmpty(written);
            foreach (var file in written)
            {
                Assert.True(File.Exists(file));
                var content = await File.ReadAllTextAsync(file);
                Assert.False(string.IsNullOrWhiteSpace(content));
            }
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public async Task RunAsync_BadPathThrows()
    {
        IRecipeRunner runner = new RecipeRunnerYaml(new EchoAdapter());
        var ex = await Assert.ThrowsAsync<FileNotFoundException>(async () =>
            await runner.RunAsync("doesnotexist.yaml", "outputs"));
        Assert.Contains("Recipe file not found", ex.Message);
    }
}

