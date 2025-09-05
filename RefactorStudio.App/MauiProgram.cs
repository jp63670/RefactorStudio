using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;
using RefactorStudio.Adapters;
using RefactorStudio.Core.Services;

namespace RefactorStudio.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<IModelAdapter, EchoAdapter>();
        builder.Services.AddSingleton<IRecipeRunner, RecipeRunnerYaml>();
        builder.Services.AddSingleton<RunRecipeService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
