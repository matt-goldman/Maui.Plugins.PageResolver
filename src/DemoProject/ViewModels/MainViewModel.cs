using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DemoProject.Pages;
using DemoProject.Popups.Pages;
using Mopups.Services;

namespace DemoProject.ViewModels;

[ObservableObject]
public partial class MainViewModel : BaseViewModel
{
    private readonly INameService _nameService;

    [ObservableProperty]
    private string _name = string.Empty;

    public MainViewModel(INameService nameService)
    {
        _nameService = nameService;
    }

    [RelayCommand]
    private void GetName()
    {
        Name = _nameService.GetName();
    }

    [RelayCommand]
    private async Task GoToPageParamPage()
    {
        Name = _nameService.GetName();

        await Navigation.PushAsync<PageParamPage>(Name);
    }

    [RelayCommand]
    private async Task GoToVmParamPage()
    {
        await Navigation.PushAsync<VmParamPage>("Name passed as parameter");
    }

    [RelayCommand]
    private async Task GoToMarkup()
    {
        await Navigation.PushAsync(new MarkupPage());
    }

    [RelayCommand]
    private async Task GoToScopeCheck()
    {
        await Navigation.PushAsync<ScopeCheckPage>();
    }

    [RelayCommand]
    private async Task GoToBrokenPage()
    {
        await Navigation.PushAsync<BrokenPage>();
    }

    [RelayCommand]
    private Task ShowEasyPopup()
    {
        return MopupService.Instance.PushAsync<EasyPopup>();
    }

    [RelayCommand]
    private Task ShowParamPopup()
    {
        return MopupService.Instance.PushAsync<ParamPopup>("It's alive!");
    }
}
