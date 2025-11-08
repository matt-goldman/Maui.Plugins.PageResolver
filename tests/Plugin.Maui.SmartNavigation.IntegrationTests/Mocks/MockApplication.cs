namespace Plugin.Maui.SmartNavigation.IntegrationTests.Mocks;

/// <summary>
/// Mock application for testing that can be used with MauiApp.CreateBuilder().UseMauiApp&lt;MockApplication&gt;()
/// </summary>
public class MockApplication : Application
{
    private Page? _mainPage;

    /// <summary>
    /// Default constructor required for MauiApp builder pattern
    /// </summary>
    public MockApplication()
    {
    }

    /// <summary>
    /// Constructor for direct instantiation in tests (legacy pattern)
    /// </summary>
    public MockApplication(Page page) : this()
    {
        _mainPage = page;
    }

    /// <summary>
    /// Sets the main page to be used when creating windows
    /// </summary>
    public void SetMainPage(Page page)
    {
        _mainPage = page;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Use the configured main page, or create a default one if not set
        var page = _mainPage ?? new ContentPage { Title = "Mock Page" };
        return new Window(page);
    }
}
