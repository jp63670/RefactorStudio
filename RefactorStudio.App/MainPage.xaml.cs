using System;
using System.IO;
using Microsoft.Maui.Controls;

namespace RefactorStudio.App;

public partial class MainPage : ContentPage
{
    private readonly RunRecipeService _service;

    public MainPage(RunRecipeService service)
    {
        InitializeComponent();
        _service = service;
    }

    private async void OnRunSample(object? sender, EventArgs e)
    {
        var sample = Path.Combine("RefactorStudio.Recipes", "samples", "echo.yaml");
        await _service.RunAsync(sample);
    }

    private async void OnRunFromPath(object? sender, EventArgs e)
    {
        var path = await DisplayPromptAsync("Run from Path", "Enter absolute .yaml path:");
        if (!string.IsNullOrWhiteSpace(path))
            await _service.RunAsync(path);
    }

    private async void OnChooseOutputFolder(object? sender, EventArgs e)
    {
        var folder = await DisplayPromptAsync("Choose Output Folder", "Enter absolute folder path:");
        if (!string.IsNullOrWhiteSpace(folder))
            _service.SetOutputRoot(folder);
    }

    private async void OnBrowse(object? sender, EventArgs e)
    {
        var path = await _service.BrowseForRecipeAsync();
        if (string.IsNullOrWhiteSpace(path))
        {
            await OnRunFromPath(sender, e);
            return;
        }
        await _service.RunAsync(path);
    }
}
