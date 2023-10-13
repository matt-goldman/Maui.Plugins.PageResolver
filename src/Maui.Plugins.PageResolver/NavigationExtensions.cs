using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.Controls;
using System;
using System.Linq;
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
        // Check if parameters fit the ViewModel's constructors
        if (ParametersMatchConstructors(viewModelType, parameters))
        {
            var viewModel = ActivatorUtilities.CreateInstance(serviceProvider, viewModelType, parameters);
            return CreatePageUsingViewModel<T>(serviceProvider, viewModel);
        }
        // Check if parameters fit the Page's constructors, excluding the ViewModel type
        else if (ParametersMatchConstructorsExcludingType(typeof(T), viewModelType, parameters))
        {
            return ActivatorUtilities.CreateInstance<T>(serviceProvider, parameters);
        }
        else
        {
            throw new ArgumentException("Provided parameters do not match the constructors of the Page or ViewModel.");
        }
    }

    private static bool ParametersMatchConstructors(Type type, params object[] parameters)
    {
        var constructors = type.GetConstructors();
        foreach (var constructor in constructors)
        {
            var ctorParams = constructor.GetParameters();
            if (ctorParams.Length == parameters.Length)
            {
                for (int i = 0; i < ctorParams.Length; i++)
                {
                    if (!ctorParams[i].ParameterType.IsAssignableFrom(parameters[i].GetType()))
                        break;
                    if (i == ctorParams.Length - 1)
                        return true; // all parameters match
                }
            }
        }
        return false;
    }

    private static bool ParametersMatchConstructorsExcludingType(Type type, Type excludeType, params object[] parameters)
    {
        var constructors = type.GetConstructors();
        foreach (var constructor in constructors)
        {
            var ctorParams = constructor.GetParameters().Where(p => p.ParameterType != excludeType).ToArray();
            if (ctorParams.Length == parameters.Length)
            {
                for (int i = 0; i < ctorParams.Length; i++)
                {
                    if (!ctorParams[i].ParameterType.IsAssignableFrom(parameters[i].GetType()))
                        break;
                    if (i == ctorParams.Length - 1)
                        return true; // all parameters match
                }
            }
        }
        return false;
    }

    private static Page CreatePageUsingViewModel<T>(IServiceProvider serviceProvider, object viewModel) where T : Page
    {
        try
        {
            return ActivatorUtilities.CreateInstance<T>(serviceProvider, viewModel);
        }
        catch (MissingMemberException)
        {
            return ActivatorUtilities.CreateInstance<T>(Resolver.GetServiceProvider());
        }
    }

    #endregion
}