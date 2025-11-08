using Microsoft.Extensions.Logging;
using Plugin.Maui.SmartNavigation.IntegrationTests.Mocks;

namespace Plugin.Maui.SmartNavigation.IntegrationTests.Infrastructure;

/// <summary>
/// Provides a MAUI host builder for integration tests, similar to platform-specific entry points.
/// This allows tests to work with a properly initialized MAUI application without requiring
/// platform-specific UI infrastructure.
/// </summary>
public static class TestMauiProgram
{
    /// <summary>
    /// Creates and configures a MauiApp instance for testing purposes.
    /// This mirrors the pattern used in platform entry points (AppDelegate, MainApplication, etc.)
    /// but uses test doubles instead of real UI components.
    /// </summary>
    /// <param name="mainPage">Optional main page to use for the application. If null, a default Page is created.</param>
    /// <returns>A configured MauiApp instance suitable for integration testing.</returns>
    public static MauiApp CreateMauiApp(Page? mainPage = null)
    {
        var builder = MauiApp.CreateBuilder();
        
        builder
            .UseMauiApp<MockApplication>()
            .ConfigureFonts(fonts =>
            {
                // Fonts typically required for MAUI, even in headless tests
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Configure the main page if provided
        if (mainPage != null)
        {
            builder.Services.AddSingleton(mainPage);
        }

        // Add test-specific services here as needed
        // builder.Services.AddTransient<IMyService, MyTestService>();

#if DEBUG
        builder.Logging.SetMinimumLevel(LogLevel.Trace);
#endif

        return builder.Build();
    }

    /// <summary>
    /// Creates a MauiApp configured for Shell navigation testing.
    /// </summary>
    /// <returns>A configured MauiApp instance with Shell as the main page.</returns>
    public static MauiApp CreateMauiAppWithShell()
    {
        var shell = new Shell();
        return CreateMauiApp(shell);
    }

    /// <summary>
    /// Creates a MauiApp configured for non-Shell navigation testing.
    /// </summary>
    /// <returns>A configured MauiApp instance with a regular Page as the main page.</returns>
    public static MauiApp CreateMauiAppWithPage()
    {
        var page = new ContentPage { Title = "Test Page" };
        return CreateMauiApp(page);
    }
}
