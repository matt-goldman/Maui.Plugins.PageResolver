namespace DemoProject;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class PageParamPage : ContentPage
{
	public PageParamPage(PageParamViewModel viewModel, string name)
	{
		InitializeComponent();
		viewModel.NameParam = name;
		BindingContext = viewModel;
	}
}