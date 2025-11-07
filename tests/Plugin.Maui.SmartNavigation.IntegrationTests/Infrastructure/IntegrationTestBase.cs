namespace Plugin.Maui.SmartNavigation.IntegrationTests.Infrastructure;

/// <summary>
/// Base class for integration tests providing common setup and utilities
/// </summary>
public abstract class IntegrationTestBase : IDisposable
{
    protected IServiceProvider ServiceProvider { get; private set; }
    protected IServiceCollection Services { get; private set; }
    protected MauiApp? MauiApp { get; private set; }
    protected Application? App { get; private set; }

    protected IntegrationTestBase()
    {
        Services = new ServiceCollection();
        SetupServices(Services);
        ServiceProvider = Services.BuildServiceProvider();
    }

    /// <summary>
    /// Override to configure services for the test
    /// </summary>
    protected virtual void SetupServices(IServiceCollection services)
    {
        // Base implementation does nothing
        // Derived classes can override to add their own services
    }

    /// <summary>
    /// Initializes a MAUI application for testing using the host builder pattern.
    /// This properly initializes the MAUI infrastructure including DI, handlers, etc.
    /// </summary>
    /// <param name="mainPage">Optional main page to use. If null, a default page is created.</param>
    protected void InitializeMauiApp(Page? mainPage = null)
    {
        MauiApp = TestMauiProgram.CreateMauiApp(mainPage);
        App = MauiApp.Services.GetRequiredService<IApplication>() as Application;

        if (App != null)
        {
            Application.Current = App;
        }
    }

    /// <summary>
    /// Initializes a MAUI application with Shell for testing Shell-based navigation.
    /// </summary>
    protected void InitializeMauiAppWithShell()
    {
        MauiApp = TestMauiProgram.CreateMauiAppWithShell();
        App = MauiApp.Services.GetRequiredService<IApplication>() as Application;

        if (App != null)
        {
            Application.Current = App;
        }
    }

    /// <summary>
    /// Initializes a MAUI application with a regular page for testing non-Shell navigation.
    /// </summary>
    protected void InitializeMauiAppWithPage()
    {
        MauiApp = TestMauiProgram.CreateMauiAppWithPage();
        App = MauiApp.Services.GetRequiredService<IApplication>() as Application;

        if (App != null)
        {
            Application.Current = App;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }

            // Clean up MAUI app
            if (MauiApp != null)
            {
                Application.Current = null;
                // MauiApp doesn't implement IDisposable, but we should clean up the reference
                MauiApp = null;
            }
        }
    }
}
