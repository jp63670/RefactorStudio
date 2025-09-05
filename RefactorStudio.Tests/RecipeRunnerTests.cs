using RefactorStudio.Core.Models;
using RefactorStudio.Core.Services;
using System.IO;

namespace RefactorStudio.Tests;

public class RecipeRunnerTests
{
    private class StubAdapter : IModelAdapter
    {
        public Task<string> ExecuteAsync(string prompt) => Task.FromResult(prompt + "-result");
    }

    [Fact]
    public async Task RunAsync_WritesOutputsPerStep()
    {
        var recipe = new RecipeYaml
        {
            Id = "test",
            Version = "1",
            Steps = new List<RecipeStep>
            {
                new() { Name = "step1", Prompt = "p1" },
                new() { Name = "step2", Prompt = "p2" }
            }
        };

        var runner = new RecipeRunnerYaml(new StubAdapter());
        await runner.RunAsync(recipe);

        var dir = Path.Combine("outputs", "test");
        try
        {
            var output1 = Path.Combine(dir, "step1.txt");
            var output2 = Path.Combine(dir, "step2.txt");
            Assert.True(File.Exists(output1));
            Assert.True(File.Exists(output2));
            Assert.Contains(output1, recipe.Steps[0].Outputs);
            Assert.Contains(output2, recipe.Steps[1].Outputs);
            Assert.Equal("p1-result", await File.ReadAllTextAsync(output1));
            Assert.Equal("p2-result", await File.ReadAllTextAsync(output2));
        }
        finally
        {
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
        }
    }
}
