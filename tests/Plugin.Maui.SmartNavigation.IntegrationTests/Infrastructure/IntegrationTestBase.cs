namespace Plugin.Maui.SmartNavigation.IntegrationTests.Infrastructure;

/// <summary>
/// Base class for integration tests providing common setup and utilities
/// </summary>
public abstract class IntegrationTestBase : IDisposable
{
    protected IServiceProvider ServiceProvider { get; private set; }
    protected IServiceCollection Services { get; private set; }

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
        }
    }
}
