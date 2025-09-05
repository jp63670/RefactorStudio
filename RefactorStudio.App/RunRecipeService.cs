using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using RefactorStudio.Core.Services;

namespace RefactorStudio.App;

public class RunRecipeService
{
    private readonly IRecipeRunner _runner;
    private string? _outputRoot;

    public RunRecipeService(IRecipeRunner runner)
    {
        _runner = runner;
    }

    public void SetOutputRoot(string path) => _outputRoot = path;

    public async Task<string?> BrowseForRecipeAsync()
    {
        try
        {
            var file = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select recipe",
                FileTypes = FilePickerFileType.All,
            });
            return file?.FullPath;
        }
        catch
        {
            return null;
        }
    }

    public async Task<IReadOnlyList<string>> RunAsync(string recipePath, CancellationToken ct = default)
    {
        var root = _outputRoot;
        if (string.IsNullOrWhiteSpace(root))
            root = FileSystem.Current.AppDataDirectory;
        try
        {
            return await _runner.RunAsync(recipePath, root, ct);
        }
        catch (UnauthorizedAccessException)
        {
            root = FileSystem.Current.AppDataDirectory;
            return await _runner.RunAsync(recipePath, root, ct);
        }
    }
}
