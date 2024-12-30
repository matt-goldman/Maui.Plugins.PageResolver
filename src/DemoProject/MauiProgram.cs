using DemoProject.Popups.Pages;
using DemoProject.Popups.ViewModels;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;

namespace DemoProject;

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
			})
			.ConfigureMopups()
			.UseAutodependencies();

			builder.Services.AddTransient<EasyPopup>();
			builder.Services.AddTransient<ParamPopup>();

			StartupExtensions.UpsertViewModelMapping<ParamPopup, ParamPopupViewModel>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
