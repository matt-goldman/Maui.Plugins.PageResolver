using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace Maui.Plugins.PageResolver;

public static class NavigationExtensions
{
    #region paramaterless navigation

    /// <summary>
    /// Resolves a page of type T (must inherit from Page) and pushes a new instance onto the navigation stack
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="navigation"></param>
    /// <returns></returns>
    public static async Task PushAsync<T>(this INavigation navigation) where T : Page
    {
        var resolvedPage = Resolver.Resolve<T>();

        await navigation.PushAsync(resolvedPage);
    }

    /// <summary>
    /// Resolves a page of type T (must inherit from Page) and pushes a new modal instance onto the navigation stack
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="navigation"></param>
    /// <returns></returns>
    public static async Task PushModalAsync<T>(this INavigation navigation) where T : Page
    {
        var resolvedPage = Resolver.Resolve<T>();

        await navigation.PushModalAsync(resolvedPage);
    }
    
    #endregion


    #region parameterized navigation
    
    /// <summary>
    /// Resolves a page of type T (must inherit from Page) and pushes a new instance onto the navigation stack
    /// </summary>
    /// <typeparam name="T">The type of the page to be resolved</typeparam>
    /// <param name="navigation"></param>
    /// <param name="parameters">The constructor parameters expected by the page to be resolved</param>
    /// <returns></returns>
    public static async Task PushAsync<T>(this INavigation navigation, params object[] parameters) where T : Page
    {
        var page = ResolvePage<T>(parameters);
        await navigation.PushAsync(page);
    }  

    /// <summary>
    /// Resolves a page of type T (must inherit from Page) and pushes a new modal instance onto the navigation stack
    /// </summary>
    /// <typeparam name="T">The type of the page to be resolved</typeparam>
    /// <param name="navigation"></param>
    /// <param name="parameters">The constructor parameters expected by the page to be resolved</param>
    /// <returns></returns>
    public static async Task PushModalAsync<T>(this INavigation navigation, params object[] parameters) where T : Page
    {
        var page = ResolvePage<T>(parameters);
        await navigation.PushModalAsync(page);
    }


    private static Page ResolvePage<T>(params object[] parameters) where T : Page
    {
        var serviceProvider = Resolver.GetServiceProvider();

        var pageType = typeof(T);
        var viewModelType = Resolver.GetViewModelType(pageType);

        if (viewModelType == null)
        {
            return CreatePageWithoutViewModel<T>(serviceProvider, parameters);
        }

        return CreatePageWithViewModel<T>(serviceProvider, viewModelType, parameters);
    }

    private static Page CreatePageWithoutViewModel<T>(IServiceProvider serviceProvider, params object[] parameters) where T : Page
    {
        var resolvedPage = ActivatorUtilities.CreateInstance<T>(serviceProvider, typeof(T), parameters);
        return resolvedPage;
    }

    private static Page CreatePageWithViewModel<T>(IServiceProvider serviceProvider, Type viewModelType, params object[] parameters) where T : Page
    {
        var viewModel = ActivatorUtilities.CreateInstance(serviceProvider, viewModelType, parameters);

        try
        {
            return ActivatorUtilities.CreateInstance<T>(serviceProvider, viewModel);

        }
        catch (MissingMemberException)
        {
            return ActivatorUtilities.CreateInstance<T>(Resolver.GetServiceProvider(), parameters);
        }
    }
    
    #endregion
}