namespace DemoProject;

public partial class ParamPage : ContentPage
{
	public ParamPage(ParamViewModel viewModel, string name)
	{
		InitializeComponent();
		viewModel.NameParam = name;
		BindingContext = viewModel;
	}
}