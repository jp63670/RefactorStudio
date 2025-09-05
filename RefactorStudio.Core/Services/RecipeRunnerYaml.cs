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
        var recipeDir = OutputPathUtil.GetRecipeDirectory(recipePath, recipe, outputRoot);

        foreach (var step in recipe.Steps)
        {
            ct.ThrowIfCancellationRequested();

            var result = await _adapter.ExecuteAsync(step.Prompt);
            if (string.IsNullOrEmpty(result))
                result = step.Prompt;

            var safe = OutputPathUtil.SafeSegment(step.Name);
            var file = Path.Combine(recipeDir, $"{safe}.txt");
            file = OutputPathUtil.EnsureUniqueFile(file);

            var fullFile = Path.GetFullPath(file);
            var fullDir = Path.GetFullPath(recipeDir);
            if (!fullFile.StartsWith(fullDir, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Output file escaped recipe directory");

            await File.WriteAllTextAsync(file, result, ct);
            Console.WriteLine($"Wrote: {file}");
            written.Add(file);
        }

        return written;
    }
}
