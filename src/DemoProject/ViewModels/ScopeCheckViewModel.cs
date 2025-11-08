using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DemoProject.ViewModels;

public partial class ScopeCheckViewModel : BaseViewModel
{
    private readonly IDefaultScopedService _defaultScopedService;
    private readonly ICustomScopedService _customScopedService;

    [ObservableProperty]
    public partial int? DefaultCount { get; set; }

    [ObservableProperty]
    public partial int? CustomCount { get; set; }

    public ScopeCheckViewModel(IDefaultScopedService defaultScopedService, ICustomScopedService customScopedService)
    {
        _defaultScopedService = defaultScopedService;
        _customScopedService = customScopedService;

        DefaultCount = _defaultScopedService.GetCount();
        CustomCount = _customScopedService.GetCount();
    }

    [RelayCommand]
    public void IncreaseCount()
    {
        _defaultScopedService.IncreaseCount();
        _customScopedService.IncreaseCount();

        DefaultCount = _defaultScopedService.GetCount();
        CustomCount = _customScopedService.GetCount();
    }
}
