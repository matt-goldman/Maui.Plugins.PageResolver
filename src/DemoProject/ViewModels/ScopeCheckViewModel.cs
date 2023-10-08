using System.Windows.Input;

namespace DemoProject.ViewModels;

public class ScopeCheckViewModel : BaseViewModel
{
    private readonly IDefaultScopedService _defaultScopedService;
    private readonly ICustomScopedService _customScopedService;

    public int DefaultCount { get; set; }

    public int CustomCount { get; set; }

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

        OnPropertChanged(nameof(DefaultCount));
        OnPropertChanged(nameof(CustomCount));
    }
}
