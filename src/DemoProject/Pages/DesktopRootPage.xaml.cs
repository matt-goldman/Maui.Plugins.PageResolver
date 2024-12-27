namespace DemoProject.Pages;

public partial class DesktopRootPage : ContentPage
{
	public DesktopRootPage(DesktopPageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}