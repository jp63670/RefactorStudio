namespace RefactorStudio.Core.Models;

public interface IModelAdapter
{
    Task<string> ExecuteAsync(string prompt);
}
