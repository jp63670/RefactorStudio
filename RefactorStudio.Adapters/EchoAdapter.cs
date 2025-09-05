using RefactorStudio.Core.Models;

namespace RefactorStudio.Adapters;

public class EchoAdapter : IModelAdapter
{
    public Task<string> ExecuteAsync(string prompt)
    {
        return Task.FromResult(prompt);
    }
}
