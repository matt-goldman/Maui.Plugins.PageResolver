using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DemoProject.Pages;
using Plugin.Maui.SmartNavigation.Extensions;

namespace DemoProject.ViewModels;

public partial class DesktopPageViewModel
    : BaseViewModel
{
    Window? _vmWindow;
    Window? _winParamsWindow;
    Window? _vmParamsWindow;

    [ObservableProperty]
    public partial bool? IsMainWindowOpen { get; set; }

    [ObservableProperty]
    public partial bool? IsWinParamsWindowOpen { get; set; }

    [ObservableProperty]
    public partial bool? IsVmParamsWindowOpen { get; set; }

    [RelayCommand]
    public void OpenWindowWithVm()
    {
        _vmWindow = App.Current?.OpenWindow<MainPage>();
        IsMainWindowOpen = true;
    }

    [RelayCommand]
    public void OpenWindowWithWinParams()
    {
        _winParamsWindow = App.Current?.OpenWindow<PageParamPage>("Name passed as page parameter");
        IsWinParamsWindowOpen = true;
    }

    [RelayCommand]
    public void OpenWindowWithVmParams()
    {
        _vmParamsWindow = App.Current?.OpenWindow<VmParamPage>("Name passed as vm parameter");

        _vmParamsWindow!.Width = 400;
        _vmParamsWindow.Height = 400;
        _vmParamsWindow.X = 100;
        _vmParamsWindow.Y = 100;

        IsVmParamsWindowOpen = true;
    }

    [RelayCommand]
    public void CloseWindowWithVm()
    {
        App.Current?.CloseWindow(_vmWindow!);
        IsMainWindowOpen = false;
    }

    [RelayCommand]
    public void CloseWindowWithWinParams()
    {
        App.Current?.CloseWindow(_winParamsWindow!);
        IsWinParamsWindowOpen = false;
    }

    [RelayCommand]
    public void CloseWindowWithVmParams()
    {
        App.Current?.CloseWindow(_vmParamsWindow!);
        IsVmParamsWindowOpen = false;
    }
}
