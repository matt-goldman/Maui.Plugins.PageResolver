using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DemoProject.Pages;
using DemoProject.Popups.Pages;
using Mopups.Services;
using Plugin.Maui.SmartNavigation.Extensions;
using System.Diagnostics;

namespace DemoProject.ViewModels;

public partial class MainViewModel(INameService nameService) : BaseViewModel
{
    [ObservableProperty]
    public partial string? Name { get; set; }

    [RelayCommand]
    private void GetName()
    {
        Name = nameService.GetName();
    }

    [RelayCommand]
    private async Task GoToPageParamPage()
    {
        Name = nameService.GetName();

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
        if (Navigation is null) return;

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
    private static Task ShowEasyPopup()
    {
        return MopupService.Instance.PushAsync<EasyPopup>();
    }

    [RelayCommand]
    private static Task ShowParamPopup()
    {
        return MopupService.Instance.PushAsync<ParamPopup>("It's alive!");
    }

    [RelayCommand]
    private async Task TriggerAggregateException()
    {
        try
        {
            await Navigation.PushAsync<AggregateExceptionPage>("test");
        }
        catch(AggregateException ex)
        {
            Debug.WriteLine(ex);
            Debugger.Break();
            throw;
        }
    }

    [RelayCommand]
    private async Task GoToNavigationManagerDemo()
    {
        await Navigation.PushAsync<NavigationManagerDemoPage>();
    }
}