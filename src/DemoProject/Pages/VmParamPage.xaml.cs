namespace DemoProject.Pages;

public partial class VmParamPage : ContentPage
{
	public VmParamPage(VmParamViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}