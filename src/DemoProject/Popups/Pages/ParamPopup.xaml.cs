using DemoProject.Popups.ViewModels;
using Mopups.Pages;

namespace DemoProject.Popups.Pages;

public partial class ParamPopup : PopupPage
{
	public ParamPopup(ParamPopupViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;
    }
}