using DemoProject.ViewModels;

namespace DemoProject
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
        }

    }
}