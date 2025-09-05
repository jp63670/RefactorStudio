using System;
using System.IO;
using System.Text;
using RefactorStudio.Core.Models;

namespace RefactorStudio.Core.Services;

public static class OutputPathUtil
{
    public static string SafeSegment(string segment)
    {
        if (string.IsNullOrWhiteSpace(segment))
            return "output";

        var sb = new StringBuilder();
        bool lastDash = false;
        foreach (var ch in segment.ToLowerInvariant())
        {
            if ((ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9') || ch == '-' || ch == '_')
            {
                if (ch == '-' && lastDash)
                    continue;
                sb.Append(ch);
                lastDash = ch == '-';
            }
            else
            {
                if (!lastDash)
                {
                    sb.Append('-');
                    lastDash = true;
                }
            }
        }
        var result = sb.ToString().Trim('-');
        if (result.Length > 64)
            result = result[..64];
        return string.IsNullOrEmpty(result) ? "output" : result;
    }

    public static string EnsureUniqueFile(string file)
    {
        var dir = Path.GetDirectoryName(file)!;
        var name = Path.GetFileNameWithoutExtension(file);
        var ext = Path.GetExtension(file);
        var candidate = file;
        int i = 1;
        while (File.Exists(candidate))
        {
            candidate = Path.Combine(dir, $"{name}-{i}{ext}");
            i++;
        }
        return candidate;
    }

    public static string GetRecipeDirectory(string recipePath, RecipeYaml recipe, string outputRoot)
    {
        var fileBase = Path.GetFileNameWithoutExtension(recipePath);
        var dirName = string.IsNullOrEmpty(recipe.Id) ? fileBase : recipe.Id;
        if (!string.IsNullOrEmpty(recipe.Id) && !string.Equals(recipe.Id, fileBase, StringComparison.OrdinalIgnoreCase))
        {
            dirName = $"{recipe.Id}-{fileBase}";
        }
        dirName = SafeSegment(dirName);
        var recipeDir = Path.Combine(outputRoot, dirName);
        Directory.CreateDirectory(recipeDir);
        return recipeDir;
    }
}
