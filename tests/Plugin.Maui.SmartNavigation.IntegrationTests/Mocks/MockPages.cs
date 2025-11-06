namespace Plugin.Maui.SmartNavigation.IntegrationTests.Mocks;

/// <summary>
/// Mock page for testing navigation
/// </summary>
public class MockPage : Page
{
    public object? NavigationParameters { get; set; }

    public MockPage()
    {
    }

    public MockPage(object parameters)
    {
        NavigationParameters = parameters;
    }
}

/// <summary>
/// Mock page with ViewModel for testing
/// </summary>
public class MockPageWithViewModel : Page
{
    public MockViewModel? ViewModel { get; }

    public MockPageWithViewModel()
    {
    }

    public MockPageWithViewModel(MockViewModel viewModel)
    {
        ViewModel       = viewModel;
        BindingContext  = viewModel;
    }
}

/// <summary>
/// Mock page with parameters for testing parameter binding
/// </summary>
public class MockPageWithParameters : Page
{
    public string? StringParam { get; set; }
    public int IntParam { get; set; }
    public object? ObjectParam { get; set; }

    public MockPageWithParameters()
    {
    }

    public MockPageWithParameters(string stringParam, int intParam)
    {
        StringParam = stringParam;
        IntParam    = intParam;
    }
}

/// <summary>
/// Mock Shell page for testing Shell navigation
/// </summary>
public class MockShellPage : Shell
{
    public MockShellPage()
    {
    }
}

/// <summary>
/// Mock modal page for testing modal navigation
/// </summary>
public class MockModalPage : Page
{
    public bool IsModal { get; set; }

    public MockModalPage()
    {
        IsModal = true;
    }
}
