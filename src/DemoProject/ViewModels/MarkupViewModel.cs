using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DemoProject.ViewModels;

public partial class MarkupViewModel(INameService nameService) : BaseViewModel
{
    [ObservableProperty]
    public partial string? Name { get; set; }

    [RelayCommand]
    void GetName()
    {
        Name = nameService.GetName();
    }
}
