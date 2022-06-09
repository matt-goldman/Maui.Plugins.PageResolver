using DemoProject.Services;
using DemoProject.ViewModels;
using Maui.Plugins.PageResolver;

namespace DemoProject
{
    public static partial class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                //.UsePageResolver()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            //UseAutoreg(builder.Services);

            builder.Services.UsePageResolver();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ParamPage>();
            builder.Services.AddTransient<BaseViewModel>();
            builder.Services.AddTransient<ParamViewModel>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddSingleton<INameService, NameService>();

            return builder.Build();
        }

        static partial void UseAutoreg(IServiceCollection services);
    }
}