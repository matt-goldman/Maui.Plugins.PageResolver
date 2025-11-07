using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Plugin.Maui.SmartNavigation.Extensions;
using Plugin.Maui.SmartNavigation.Routing;

namespace Plugin.Maui.SmartNavigation.Services;

#nullable enable
internal partial class NavigationManager() : INavigationManager
{
    public async Task GoBackAsync()
    {
        // Priority 1: Pop modal if present
        if (CurrentNavigation?.ModalStack?.Count > 0)
        {
            await CurrentNavigation.PopModalAsync();
            return;
        }

        // Priority 2: Shell navigation
        if (CurrentPage is Shell shell)
        {
            await shell.GoToAsync("..");
            return;
        }

        // Priority 3: Regular navigation stack
        if (CurrentNavigation?.NavigationStack?.Count > 1)
        {
            await CurrentNavigation.PopAsync();
        }
    }

    public async Task GoToAsync(Route route, string? query = null)
    {
        if (CurrentPage is Shell shell)
        {
            await shell.GoToAsync(route.Build(query));
            return;
        }

        throw new InvalidOperationException(
            $"Cannot navigate to route '{route.Path}'. Shell navigation is not available. " +
            "Use PushAsync<TPage>() for hierarchical navigation instead.");
    }

    public Task PopAsync() => CurrentNavigation?.PopAsync()??Task.CompletedTask;

    public Task PopModalAsync() => CurrentNavigation?.PopModalAsync()??Task.CompletedTask;

    public Task PushAsync<TPage>(object? args = null) where TPage : Page => args switch 
    {
        null => CurrentNavigation.PushAsync<TPage>(),
        _ => CurrentNavigation.PushAsync<TPage>(args)
    };

    public Task PushModalAsync<TPage>(object? args = null) where TPage : Page => args switch 
    {
        null => CurrentNavigation.PushModalAsync<TPage>(),
        _ => CurrentNavigation.PushModalAsync<TPage>(args)
    };

    private static Page? CurrentPage => Application.Current?.Windows[0].Page;

    private static INavigation? CurrentNavigation => CurrentPage?.Navigation;
}
