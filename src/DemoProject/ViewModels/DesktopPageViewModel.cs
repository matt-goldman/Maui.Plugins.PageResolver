using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DemoProject.Pages;

namespace DemoProject.ViewModels;

public partial class DesktopPageViewModel
    : ObservableObject
{
    Window? _vmWindow;
    Window? _winParamsWindow;
    Window? _vmParamsWindow;

    [ObservableProperty]
    bool _isMainWindowOpen;

    [ObservableProperty]
    bool _isWinParamsWindowOpen;

    [ObservableProperty]
    bool _isVmParamsWindowOpen;

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
