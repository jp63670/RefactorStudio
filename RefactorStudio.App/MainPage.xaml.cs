using Microsoft.Maui.Storage;
using RefactorStudio.Adapters;
using RefactorStudio.Core.Services;
using System.Collections.Generic;
using System.IO;

namespace RefactorStudio.App;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked(object? sender, EventArgs e)
    {
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }

    private async void OnRunRecipeClicked(object? sender, EventArgs e)
    {
        try
        {
            var file = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select recipe",
                FileTypes = new FilePickerFileType(new Dictionary<Microsoft.Maui.Devices.DevicePlatform, IEnumerable<string>>
                {
                    { Microsoft.Maui.Devices.DevicePlatform.WinUI, new[] { ".yaml", ".yml" } },
                    { Microsoft.Maui.Devices.DevicePlatform.Unknown, new[] { ".yaml", ".yml" } }
                })
            });

            if (file == null)
                return;

            var recipePath = Path.GetFullPath(file.FullPath);
            var recipeName = Path.GetFileNameWithoutExtension(recipePath);
            var outputRoot = Path.Combine("outputs", recipeName);
            Directory.CreateDirectory(outputRoot);

            var adapter = new EchoAdapter();
            IRecipeRunner runner = new RecipeRunnerYaml(adapter);
            await runner.RunAsync(recipePath, outputRoot);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}

