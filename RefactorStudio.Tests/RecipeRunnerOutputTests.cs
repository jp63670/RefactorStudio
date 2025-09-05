using System;
using System.IO;
using RefactorStudio.Core.Models;
using RefactorStudio.Core.Services;
using Xunit;

namespace RefactorStudio.Tests;

public class RecipeRunnerOutputTests
{
    private class StubAdapter : IModelAdapter
    {
        public Task<string> ExecuteAsync(string prompt) => Task.FromResult("result");
    }

    [Fact]
    public async Task SanitizesStepNamesAndKeepsInsideDir()
    {
        var yaml = "id: test\nversion: 1\nsteps:\n- name: '*Hello:World*'\n  prompt: hi";
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var recipePath = Path.Combine(tempDir, "sample.yaml");
        await File.WriteAllTextAsync(recipePath, yaml);
        var outputRoot = Path.Combine(tempDir, "out");
        var runner = new RecipeRunnerYaml(new StubAdapter());
        var written = await runner.RunAsync(recipePath, outputRoot);
        var recipe = RecipeLoader.Load(recipePath);
        var recipeDir = OutputPathUtil.GetRecipeDirectory(recipePath, recipe, outputRoot);
        var safe = OutputPathUtil.SafeSegment("*Hello:World*");
        Assert.Single(written);
        var file = written[0];
        Assert.StartsWith(Path.Combine(recipeDir, safe), file);
        Assert.True(File.Exists(file));
        Assert.StartsWith(Path.GetFullPath(recipeDir), Path.GetFullPath(file), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ReadOnlyCurrentDirectoryStillWrites()
    {
        var yaml = "id: test\nversion: 1\nsteps:\n- name: step\n  prompt: hi";
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var recipePath = Path.Combine(tempDir, "echo.yaml");
        await File.WriteAllTextAsync(recipePath, yaml);
        var outputRoot = Path.Combine(tempDir, "out");
        Directory.CreateDirectory(outputRoot);
        var old = Directory.GetCurrentDirectory();
        var readonlyDir = Path.Combine(tempDir, "ro");
        Directory.CreateDirectory(readonlyDir);
        Directory.SetCurrentDirectory(readonlyDir);
        try
        {
            var runner = new RecipeRunnerYaml(new StubAdapter());
            var written = await runner.RunAsync(recipePath, outputRoot);
            Assert.NotEmpty(written);
        }
        finally
        {
            Directory.SetCurrentDirectory(old);
        }
    }

    [Fact]
    public async Task SameFilenameDifferentIdCreatesDistinctDirs()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var recipe1Dir = Path.Combine(tempDir, "r1");
        var recipe2Dir = Path.Combine(tempDir, "r2");
        Directory.CreateDirectory(recipe1Dir);
        Directory.CreateDirectory(recipe2Dir);
        var recipe1Path = Path.Combine(recipe1Dir, "same.yaml");
        var recipe2Path = Path.Combine(recipe2Dir, "same.yaml");
        await File.WriteAllTextAsync(recipe1Path, "id: first\nversion:1\nsteps:\n- name:a\n  prompt:a");
        await File.WriteAllTextAsync(recipe2Path, "id: second\nversion:1\nsteps:\n- name:a\n  prompt:a");
        var outputRoot = Path.Combine(tempDir, "out");
        var runner = new RecipeRunnerYaml(new StubAdapter());
        await runner.RunAsync(recipe1Path, outputRoot);
        await runner.RunAsync(recipe2Path, outputRoot);
        var dir1 = OutputPathUtil.GetRecipeDirectory(recipe1Path, RecipeLoader.Load(recipe1Path), outputRoot);
        var dir2 = OutputPathUtil.GetRecipeDirectory(recipe2Path, RecipeLoader.Load(recipe2Path), outputRoot);
        Assert.NotEqual(dir1, dir2);
    }
}
