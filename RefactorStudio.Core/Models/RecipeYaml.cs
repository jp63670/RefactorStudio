namespace RefactorStudio.Core.Models;

public class RecipeYaml
{
    public string Id { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
     public string? Description { get; set; }
    public List<RecipeStep> Steps { get; set; } = new();
}

public class RecipeStep
{
    public string Name { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public List<string> Outputs { get; set; } = new();
}
