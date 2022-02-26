using DemoProject.Services;
using DemoProject.ViewModels;

namespace DemoProject
{
    public static partial class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            UseAutoreg(builder.Services);

            //builder.Services.AddTransient<MainPage>();

            //builder.Services.AddTransient<MainViewModel>();

            //builder.Services.AddSingleton<INameService, NameService>();

            return builder.Build();
        }

        static partial void UseAutoreg(IServiceCollection services);
    }
}