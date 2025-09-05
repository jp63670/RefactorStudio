using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using RefactorStudio.Core.Models;

namespace RefactorStudio.Core.Services;

public class RecipeRunnerYaml : IRecipeRunner
{
    private readonly IModelAdapter _adapter;

    public RecipeRunnerYaml(IModelAdapter adapter)
    {
        _adapter = adapter;
    }

    public async Task<IReadOnlyList<string>> RunAsync(string recipePath, string outputRoot, CancellationToken ct = default)
    {
        if (!File.Exists(recipePath))
            throw new FileNotFoundException("Recipe file not found.", recipePath);

        Console.WriteLine($"Recipe: {recipePath}");
        Console.WriteLine($"Output folder: {outputRoot}");

        Directory.CreateDirectory(outputRoot);

        var recipe = RecipeLoader.Load(recipePath);
        var written = new List<string>();

        foreach (var step in recipe.Steps)
        {
            ct.ThrowIfCancellationRequested();

            var result = await _adapter.ExecuteAsync(step.Prompt);
            if (string.IsNullOrEmpty(result))
                result = step.Prompt;

            var file = Path.Combine(outputRoot, $"{step.Name}.txt");
            await File.WriteAllTextAsync(file, result, ct);
            Console.WriteLine($"Wrote: {file}");
            written.Add(file);
        }

        return written;
    }
}
