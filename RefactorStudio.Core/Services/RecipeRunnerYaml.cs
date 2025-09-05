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

        var recipe = RecipeLoader.Load(recipePath);
        var safeName = MakeSafeName(recipe.Id ?? Path.GetFileNameWithoutExtension(recipePath));
        var recipeDir = Path.Combine(outputRoot, safeName);
        Console.WriteLine($"Per-recipe folder: {recipeDir}");
        Directory.CreateDirectory(recipeDir);

        var outputs = new List<string>();
        foreach (var step in recipe.Steps)
        {
            ct.ThrowIfCancellationRequested();
            var result = await _adapter.ExecuteAsync(step.Prompt);
            var file = Path.Combine(recipeDir, $"{step.Name}.txt");
            await File.WriteAllTextAsync(file, result, ct);
            if (new FileInfo(file).Length == 0)
                throw new InvalidOperationException($"Step '{step.Name}' produced empty output.");
            step.Outputs.Add(file);
            outputs.Add(file);
            Console.WriteLine($"Written file: {file}");
        }

        return outputs;
    }

    private static string MakeSafeName(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }
        return name;
    }
}
