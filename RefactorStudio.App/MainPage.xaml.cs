using Microsoft.Maui.Storage;
using RefactorStudio.Adapters;
using RefactorStudio.Core.Services;
using System.Collections.Generic;
using System.IO;

﻿namespace RefactorStudio.App;

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

                if (!file.FullPath.Contains(Path.Combine("RefactorStudio.Recipes", "samples")))
                        return;

                var recipe = RecipeLoader.Load(file.FullPath);
                var adapter = new EchoAdapter();
                var runner = new RecipeRunnerYaml(adapter);
                await runner.RunAsync(recipe);
        }
}
