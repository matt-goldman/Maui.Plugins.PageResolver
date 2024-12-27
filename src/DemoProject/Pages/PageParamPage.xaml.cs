namespace DemoProject;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class PageParamPage : ContentPage
{
	public PageParamPage(string name)//PageParamViewModel viewModel, string name)
	{
		InitializeComponent();
		//viewModel.NameParam = name;
		//BindingContext = viewModel;
	}
}