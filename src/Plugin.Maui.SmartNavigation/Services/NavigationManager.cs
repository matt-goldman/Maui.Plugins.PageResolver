using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Plugin.Maui.SmartNavigation.Extensions;
using Plugin.Maui.SmartNavigation.Routing;

namespace Plugin.Maui.SmartNavigation.Services;

#nullable enable
internal partial class NavigationManager(INavigation navigation) : INavigationManager
{
    public async Task GoBackAsync()
    {
        var current = Application.Current?.Windows[0].Page;

        // Priority 1: Pop modal if present
        if (navigation.ModalStack?.Count > 0)
        {
            await navigation.PopModalAsync();
            return;
        }

        // Priority 2: Shell navigation
        if (current is Shell shell)
        {
            await shell.GoToAsync("..");
            return;
        }

        // Priority 3: Regular navigation stack
        await navigation.PopAsync();
    }

    public async Task GoToAsync(Route route, string? query = null)
    {
        var current = Application.Current?.Windows[0].Page;

        if (current is Shell shell)
        {
            await shell.GoToAsync(route.Build(query));
            return;
        }

        throw new InvalidOperationException(
            $"Cannot navigate to route '{route.Path}'. Shell navigation is not available. " +
            "Use PushAsync<TPage>() for hierarchical navigation instead.");
    }

    public Task PopAsync() => navigation.PopAsync();

    public Task PopModalAsync() => navigation.PopModalAsync();

    public Task PushAsync<TPage>(object? args = null) where TPage : Page => navigation.PushAsync<TPage>(args);

    public Task PushModalAsync<TPage>(object? args = null) where TPage : Page => navigation.PushModalAsync<TPage>(args);
}
