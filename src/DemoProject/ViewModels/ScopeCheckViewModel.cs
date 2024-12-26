using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace DemoProject.ViewModels;

[ObservableObject]
public partial class ScopeCheckViewModel : BaseViewModel
{
    private readonly IDefaultScopedService _defaultScopedService;
    private readonly ICustomScopedService _customScopedService;

    [ObservableProperty]
    private int _defaultCount;

    [ObservableProperty]
    private int _customCount;

    public ICommand IncreaseCountCommand => new Command(() => IncreaseCount());

    public ScopeCheckViewModel(IDefaultScopedService defaultScopedService, ICustomScopedService customScopedService)
    {
        _defaultScopedService = defaultScopedService;
        _customScopedService = customScopedService;

        DefaultCount = _defaultScopedService.GetCount();
        CustomCount = _customScopedService.GetCount();
    }

    public void IncreaseCount()
    {
        _defaultScopedService.IncreaseCount();
        _customScopedService.IncreaseCount();

        DefaultCount = _defaultScopedService.GetCount();
        CustomCount = _customScopedService.GetCount();
    }
}
