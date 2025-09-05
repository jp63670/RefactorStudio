namespace RefactorStudio.Core.Services;

public interface IRecipeRunner
{
    Task<IReadOnlyList<string>> RunAsync(string recipePath, string outputRoot, CancellationToken ct = default);
}

