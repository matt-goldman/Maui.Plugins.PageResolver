using DemoProject.ViewModels;

namespace DemoProject
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel viewModel)
        {
            viewModel.Navigation = Navigation;
            BindingContext = viewModel;
            InitializeComponent();
        }

    }
}