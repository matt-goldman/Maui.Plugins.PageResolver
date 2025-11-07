using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DemoProject.Pages;
using Plugin.Maui.SmartNavigation.Behaviours;

namespace DemoProject.ViewModels;

/// <summary>
/// Demonstrates the use of INavigationManager service and IViewModelLifecycle
/// </summary>
public partial class NavigationManagerDemoViewModel(INavigationManager navigationManager, INameService nameService) : ObservableObject, IViewModelLifecycle
{
    [ObservableProperty]
    public partial string? Message { get; set; }

    [ObservableProperty]
    public partial string? InitMessage { get; set; }

    // IViewModelLifecycle implementation
    public async Task OnInitAsync(bool isFirstNavigation)
    {
        try
        {
            // Simulate async data loading
            await Task.Delay(500);
            
            if (isFirstNavigation)
            {
                InitMessage = $"First navigation! Welcome {nameService.GetName()}";
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
        await navigationManager.PushAsync<ScopeCheckPage>();
    }

    [RelayCommand]
    private async Task NavigateToMarkup()
    {
        Message = "Navigating to markup page...";
        await navigationManager.PushAsync<MarkupPage>();
    }

    [RelayCommand]
    private async Task ShowModalPage()
    {
        Message = "Showing modal page...";
        await navigationManager.PushModalAsync<PageParamPage>("Modal Parameter from INavigation Page");
    }

    [RelayCommand]
    private async Task GoBack()
    {
        Message = "Going back (automatically determines navigation type)...";
        await navigationManager.GoBackAsync();
    }
}
