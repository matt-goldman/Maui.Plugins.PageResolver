namespace DemoProject.Pages;

public partial class ScopeCheckPage : ContentPage
{
	public ScopeCheckPage(ScopeCheckViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}