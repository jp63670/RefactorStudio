using RefactorStudio.Core.Models;
using YamlDotNet.Serialization;

namespace RefactorStudio.Core.Services;

public static class RecipeLoader
{
    public static RecipeYaml Load(string path)
    {
        var yaml = File.ReadAllText(path);
        var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
        var recipe = deserializer.Deserialize<RecipeYaml>(yaml);
        if (recipe == null)
        {
            throw new InvalidOperationException("Invalid recipe file.");
        }
        return recipe;
    }
}
