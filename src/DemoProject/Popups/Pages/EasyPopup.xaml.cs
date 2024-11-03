using DemoProject.Popups.ViewModels;
using Mopups.Pages;

namespace DemoProject.Popups.Pages;

public partial class EasyPopup : PopupPage
{
	public EasyPopup(PopupViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;
    }
}