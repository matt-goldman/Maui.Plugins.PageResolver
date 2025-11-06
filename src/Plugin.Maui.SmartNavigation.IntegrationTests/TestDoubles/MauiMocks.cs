namespace Microsoft.Maui.Controls;

/// <summary>
/// Mock Page class for testing (simulates MAUI Page)
/// </summary>
public class Page
{
    public object? BindingContext { get; set; }
    public string? Title { get; set; }
}

/// <summary>
/// Mock Shell class for testing (simulates MAUI Shell)
/// </summary>
public class Shell : Page
{
    public virtual Task GoToAsync(string route)
    {
        // Mock implementation for testing
        return Task.CompletedTask;
    }
}

/// <summary>
/// Mock INavigation interface for testing (simulates MAUI INavigation)
/// </summary>
public interface INavigation
{
    IReadOnlyList<Page> NavigationStack { get; }
    IReadOnlyList<Page> ModalStack { get; }
    
    Task PushAsync(Page page);
    Task<Page> PopAsync();
    Task PushModalAsync(Page page);
    Task<Page> PopModalAsync();
    void InsertPageBefore(Page page, Page before);
    void RemovePage(Page page);
}

/// <summary>
/// Mock Application class for testing (simulates MAUI Application)
/// </summary>
public class Application
{
    public static Application? Current { get; set; }
    public List<Window> Windows { get; } = new();
}

/// <summary>
/// Mock Window class for testing (simulates MAUI Window)
/// </summary>
public class Window
{
    public Page? Page { get; set; }

    public Window()
    {
    }

    public Window(Page page)
    {
        Page = page;
    }
}
