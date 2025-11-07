using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DemoProject.Pages;
using Plugin.Maui.SmartNavigation;
using Plugin.Maui.SmartNavigation.Behaviours;

namespace DemoProject.ViewModels;

/// <summary>
/// Demonstrates the use of INavigationManager service and IViewModelLifecycle
/// </summary>
public partial class NavigationManagerDemoViewModel : ObservableObject, IViewModelLifecycle
{
    private readonly INavigationManager _navigationManager;
    private readonly INameService _nameService;

    [ObservableProperty]
    private string? _message;

    [ObservableProperty]
    private string? _initMessage;

    public NavigationManagerDemoViewModel(INavigationManager navigationManager, INameService nameService)
    {
        _navigationManager = navigationManager;
        _nameService = nameService;
    }

    // IViewModelLifecycle implementation
    public async Task OnInitAsync(bool isFirstNavigation)
    {
        try
        {
            // Simulate async data loading
            await Task.Delay(500);
            
            if (isFirstNavigation)
            {
                InitMessage = $"First navigation! Welcome {_nameService.GetName()}";
            }
            else
            {
                InitMessage = "Returning to this page";
            }
        }
        catch (Exception ex)
        {
            InitMessage = $"Error during initialization: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task NavigateToScopeCheck()
    {
        Message = "Using INavigationManager.PushAsync...";
        await _navigationManager.PushAsync<ScopeCheckPage>();
    }

    [RelayCommand]
    private async Task NavigateToMarkup()
    {
        Message = "Navigating to markup page...";
        await _navigationManager.PushAsync<MarkupPage>();
    }

    [RelayCommand]
    private async Task ShowModalPage()
    {
        Message = "Showing modal page...";
        await _navigationManager.PushModalAsync<PageParamPage>("Modal Parameter");
    }

    [RelayCommand]
    private async Task GoBack()
    {
        Message = "Going back (automatically determines navigation type)...";
        await _navigationManager.GoBackAsync();
    }
}
