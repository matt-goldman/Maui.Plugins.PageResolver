using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Plugin.Maui.SmartNavigation.Extensions;
using Plugin.Maui.SmartNavigation.Routing;

namespace Plugin.Maui.SmartNavigation.Services;

internal partial class NavigationManager(INavigation navigation) : INavigationManager
{
    public async Task GoBackAsync()
    {
        var current = Application.Current?.Windows[0].Page;

        if (current is Shell shell)
        {
            await shell.GoToAsync("..");
            return;
        }

        await navigation.PopAsync();
    }

    public async Task GoToAsync(Route route, object query = null)
    {
        var current = Application.Current?.Windows[0].Page;

        if (current is Shell shell)
        {
            await shell.GoToAsync(route.Build(query));
        }

        // No implementation for non-Shell
    }

    public Task PopAsync() => navigation.PopAsync();

    public Task PopModalAsync() => navigation.PopModalAsync();

    public Task PushAsync<TPage>(object args = null) where TPage : Page => navigation.PushAsync<TPage>(args);

    // TODO: What is the wrapInNav arg for?
    public Task PushModalAsync<TPage>(object args = null, bool wrapInNav = true) where TPage : Page => navigation.PushModalAsync<TPage>(args);

    public Task SmartBackAsync()
    {
        // TODO: What is this for?
        throw new System.NotImplementedException();
    }
}
