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
            Console.WriteLine($"Selected recipe path: {recipePath}");

            var outputRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RefactorStudio", "outputs");
            Console.WriteLine($"Output root: {outputRoot}");

            var safeName = MakeSafeName(Path.GetFileNameWithoutExtension(recipePath));
            var recipeDir = Path.Combine(outputRoot, safeName);
            Directory.CreateDirectory(recipeDir);
            Console.WriteLine($"Per-recipe folder: {recipeDir}");

            var adapter = new EchoAdapter();
            IRecipeRunner runner = new RecipeRunnerYaml(adapter);
            await runner.RunAsync(recipePath, recipeDir);
        }
        catch (OperationCanceledException)
        {
            // user cancelled; no-op
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await DisplayAlert("Run Recipe failed", ex.Message, "OK");
        }
    }

    private static string MakeSafeName(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name;
    }
}
