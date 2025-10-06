#nullable enable
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Plugin.Maui.SmartNavigation.Routing;

#pragma warning disable IDE0130 // Namespace does not match folder structure - intended for public API
namespace Plugin.Maui.SmartNavigation;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public partial interface INavigationManager
{
    // Shell
    Task GoToAsync(Route route, object? query = null);

    Task GoBackAsync();

    // Stack
    Task PushAsync<TPage>(object? args = null) where TPage : Page;
    Task PopAsync();

    // Modal
    Task PushModalAsync<TPage>(object? args = null, bool wrapInNav = true) where TPage : Page;
    Task PopModalAsync();

    // Optional convenience
    Task SmartBackAsync(); // Pops modal if present else Shell ".." else PopAsync
}
