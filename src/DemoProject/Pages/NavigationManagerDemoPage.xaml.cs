namespace DemoProject.Pages;

public partial class NavigationManagerDemoPage : ContentPage
{
    public NavigationManagerDemoPage(NavigationManagerDemoViewModel vm)
    {
        BindingContext = vm;
        InitializeComponent();
    }
}
