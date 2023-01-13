using System.Windows.Input;

namespace DemoProject.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly INameService _nameService;

    public ICommand GetNameCommand => new Command(() => GetName());

    public ICommand GoToNameCommand => new Command(async () => await GoToName());

    public string Name { get; set; }

    public MainViewModel(INameService nameService)
    {
        _nameService = nameService;
    }

    void GetName()
    {
        Name = _nameService.GetName();

        OnPropertChanged(nameof(Name));
    }

    async Task GoToName()
    {
        Name = _nameService.GetName();

        await Navigation.PushAsync<ParamPage>(Name);
    }
}
