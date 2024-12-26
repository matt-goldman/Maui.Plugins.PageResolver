using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace DemoProject.ViewModels;

[ObservableObject]
public partial class MarkupViewModel : BaseViewModel
{
    private readonly INameService _nameService;

    [ObservableProperty]
    private string _name = string.Empty;

    public ICommand GetNameCommand => new Command(() => GetName());

    public MarkupViewModel(INameService nameService)
	{
        _nameService = nameService;
    }

    void GetName()
    {
        Name = _nameService.GetName();
    }
}
