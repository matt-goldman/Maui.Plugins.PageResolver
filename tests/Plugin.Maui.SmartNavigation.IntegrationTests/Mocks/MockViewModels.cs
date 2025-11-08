using Plugin.Maui.SmartNavigation.Behaviours;

namespace Plugin.Maui.SmartNavigation.IntegrationTests.Mocks;

/// <summary>
/// Mock ViewModel for testing
/// </summary>
public class MockViewModel
{
    public string? StringProperty { get; set; }
    public int IntProperty { get; set; }
    public object? ObjectProperty { get; set; }

    public MockViewModel()
    {
    }

    public MockViewModel(string stringProperty, int intProperty)
    {
        StringProperty  = stringProperty;
        IntProperty     = intProperty;
    }
}

/// <summary>
/// Mock ViewModel implementing IViewModelLifecycle for testing lifecycle
/// </summary>
public class MockLifecycleViewModel : IViewModelLifecycle
{
    public int OnInitAsyncCallCount { get; private set; }
    public bool? LastIsFirstNavigation { get; private set; }
    public List<bool> NavigationHistory { get; } = new();

    public Task OnInitAsync(bool isFirstNavigation)
    {
        OnInitAsyncCallCount++;
        LastIsFirstNavigation = isFirstNavigation;
        NavigationHistory.Add(isFirstNavigation);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Mock ViewModel with parameters for testing parameter binding
/// </summary>
public class MockViewModelWithParameters
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public bool IsActive { get; set; }

    public MockViewModelWithParameters()
    {
    }

    public MockViewModelWithParameters(string name, int age)
    {
        Name    = name;
        Age     = age;
    }
}
