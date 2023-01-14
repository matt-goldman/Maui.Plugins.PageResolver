using System.Windows.Input;

namespace DemoProject.ViewModels;

public class MarkupViewModel : BaseViewModel
{
    private readonly INameService _nameService;

    public string Name { get; set; }

    public ICommand GetNameCommand => new Command(() => GetName());

    public MarkupViewModel(INameService nameService)
	{
        _nameService = nameService;
    }

    void GetName()
    {
        Name = _nameService.GetName();

        OnPropertChanged(nameof(Name));
    }
}
