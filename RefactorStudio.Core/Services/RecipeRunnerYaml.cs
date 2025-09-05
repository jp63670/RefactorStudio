using RefactorStudio.Core.Models;

namespace RefactorStudio.Core.Services;

public class RecipeRunnerYaml
{
    private readonly IModelAdapter _adapter;

    public RecipeRunnerYaml(IModelAdapter adapter)
    {
        _adapter = adapter;
    }

    public async Task RunAsync(RecipeYaml recipe)
    {
        var baseDir = Path.Combine("outputs", recipe.Id);
        Directory.CreateDirectory(baseDir);
        foreach (var step in recipe.Steps)
        {
            var result = await _adapter.ExecuteAsync(step.Prompt);
            var file = Path.Combine(baseDir, $"{step.Name}.txt");
            await File.WriteAllTextAsync(file, result);
        }
    }
}
