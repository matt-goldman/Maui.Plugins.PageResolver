#nullable enable
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Plugin.Maui.SmartNavigation.Routing;

#pragma warning disable IDE0130 // Namespace does not match folder structure - intended for public API
namespace Plugin.Maui.SmartNavigation;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public partial interface INavigationManager
{
    /// <summary>
    /// Navigates to a specific route with optional query parameters.
    /// </summary>
    /// <param name="route"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <remarks>
    /// This method can only be called when the current page is a Shell.
    /// </remarks>
    Task GoToAsync(Route route, string? query = null);

    /// <summary>
    /// Navigates asynchronously to the previous page, attempting modal, Shell, and navigation stack in priority order.
    /// </summary>
    /// <remarks>
    /// This method attempts to navigate backward using the following priority:
    /// <list type="number">
    /// <item><description>Modal navigation stack - pops the current modal page if present</description></item>
    /// <item><description>Shell navigation - navigates back within Shell if the current page is a Shell page</description></item>
    /// <item><description>Traditional navigation stack - pops from the navigation stack if available</description></item>
    /// </list>
    /// If none of these navigation contexts are available, the operation may have no effect.
    /// </remarks>
    /// <returns>A task that represents the asynchronous navigation operation. The task completes when the navigation has
    /// finished.</returns>
    Task GoBackAsync();

    /// <summary>
    /// Navigates asynchronously to the specified page, optionally passing arguments to the new page.
    /// </summary>
    /// <typeparam name="TPage">The type of the page to navigate to. Must derive from <see cref="Page"/>.</typeparam>
    /// <param name="args">An optional object containing arguments to be passed to the target page. Can be <see langword="null"/> if no
    /// arguments are required.</param>
    /// <returns>A task that represents the asynchronous navigation operation.</returns>
    Task PushAsync<TPage>(object? args = null) where TPage : Page;

    /// <summary>
    /// Pops the current hierarchical page from the navigation stack asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous pop operation.</returns>
    Task PopAsync();

    /// <summary>
    /// Navigates to a new modal page of the specified type asynchronously.
    /// </summary>
    /// <remarks>Use this method to present a modal page on top of the current navigation stack. The modal
    /// page will remain visible until it is dismissed. This method is typically used for scenarios that require user
    /// interaction before returning to the previous page.</remarks>
    /// <typeparam name="TPage">The type of the page to display modally. Must derive from <see cref="Page"/>.</typeparam>
    /// <param name="args">An optional argument object to pass to the modal page. Can be <see langword="null"/> if no arguments are
    /// required.</param>
    /// <returns>A task that represents the asynchronous navigation operation.</returns>
    Task PushModalAsync<TPage>(object? args = null) where TPage : Page;
    
    /// <summary>
    /// Dismisses the topmost modal page asynchronously from the navigation stack.
    /// </summary>
    /// <remarks>If there are no modal pages on the stack, the operation has no effect. This method should be
    /// awaited to ensure that the modal page is fully dismissed before performing subsequent navigation
    /// actions.</remarks>
    /// <returns>A task that represents the asynchronous dismiss operation.</returns>
    Task PopModalAsync();
    
    
}
