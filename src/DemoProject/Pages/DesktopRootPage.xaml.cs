namespace DemoProject.Pages;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class DesktopRootPage : ContentPage
{
	public DesktopRootPage(DesktopPageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}