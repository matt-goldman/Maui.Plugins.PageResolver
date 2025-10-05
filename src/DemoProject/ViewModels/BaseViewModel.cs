using CommunityToolkit.Mvvm.ComponentModel;

namespace DemoProject.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    public INavigation? Navigation { get; set; }
}
