using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Plugin.Maui.SmartNavigation.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Maui.SmartNavigation.Extensions;

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

    /// <summary>
    /// Resolves a page of type T (must inherit from Page) and inserts it onto the navigation stack before a specific page
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="navigation"></param>
    /// <param name="before"></param>
    public static void InsertPageBefore<T>(this INavigation navigation, Page before) where T : Page
    {
        var resolvedPage = Resolver.Resolve<T>();
        navigation.InsertPageBefore(resolvedPage, before);
    }

    /// <summary>
    /// Creates a new window with a resolved page of type T (must inherit from Page)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static Window CreateWindow<T>() where T : Page
    {
        var resolvedPage = Resolver.Resolve<T>();
        return new Window(resolvedPage);
    }

    /// <summary>
    /// Creates a new window with a resolved page of type T (must inherit from Page) and opens it, and returns the window
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static Window OpenWindow<T>(this Application application) where T : Page
    {
        var window = CreateWindow<T>();
        application.OpenWindow(window);
        return window;
    }

    #endregion paramaterless navigation

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

    /// <summary>
    /// Resolves a page of type T (must inherit from Page) and inserts it onto the navigation stack before a specific page
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="navigation"></param>
    /// <param name="before"></param>
    /// <param name="parameters">The constructor parameters expected by the page to be resolved</param>
    public static void InsertPageBefore<T>(this INavigation navigation, Page before, params object[] parameters) where T : Page
    {
        var page = ResolvePage<T>(parameters);
        navigation.InsertPageBefore(page, before);
    }

    /// <summary>
    /// Creates a new window with a resolved page of type T (must inherit from Page)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameters">The constructor parameters expected by the page to be resolved</param>
    public static Window CreateWindow<T>(params object[] parameters) where T : Page
    {
        var page = ResolvePage<T>(parameters);
        return new Window(page);
    }

    /// <summary>
    /// Creates a new window with a resolved page of type T (must inherit from Page) and opens it, and returns the window
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameters">The constructor parameters expected by the page to be resolved</param>
    public static Window OpenWindow<T>(this Application application, params object[] parameters) where T : Page
    {
        var window = CreateWindow<T>(parameters);
        application.OpenWindow(window);
        return window;
    }

    #endregion parameterized navigation

    internal static Page ResolvePage<T>(params object[] parameters) where T : Page
    {
        var serviceProvider = Resolver.GetServiceProvider();

        var pageType = typeof(T);
        var viewModelType = Resolver.GetViewModelType(pageType);

        if (viewModelType is not null && parameters.Any(x => x.GetType().Equals(viewModelType)))
        {
            viewModelType = null;
        }

        if (viewModelType == null)
        {
            return CreatePageWithoutViewModel<T>(serviceProvider, parameters);
        }

        return CreatePageWithViewModel<T>(serviceProvider, viewModelType, parameters);
    }

    private static Page CreatePageWithoutViewModel<T>(IServiceProvider serviceProvider, params object[] parameters) where T : Page
    {
        return ActivatorUtilities.CreateInstance<T>(serviceProvider, parameters);
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
        var sp = Resolver.GetServiceProvider();

        var constructors = type.GetConstructors();
        foreach (var constructor in constructors)
        {
            var ctorParams = constructor.GetParameters();

            var nonInjectableParams = ctorParams.Where(p => !IsRegisteredDependency(sp, p.ParameterType)).ToArray();

            if (nonInjectableParams.Length == parameters.Length)
            {
                for (int i = 0; i < nonInjectableParams.Length; i++)
                {
                    if (!nonInjectableParams[i].ParameterType.IsAssignableFrom(parameters[i].GetType()))
                        break;
                    if (i == nonInjectableParams.Length - 1)
                        return true; // all parameters match
                }
            }
        }
        return false;
    }

    private static bool ParametersMatchConstructorsExcludingType(Type type, Type excludeType, params object[] parameters)
    {
        var sp = Resolver.GetServiceProvider();

        var constructors = type.GetConstructors();
        foreach (var constructor in constructors)
        {
            var ctorParams = constructor.GetParameters().Where(p => p.ParameterType != excludeType).ToArray();

            var nonInjectableParams = ctorParams.Where(p => !IsRegisteredDependency(sp, p.ParameterType)).ToArray();

            if (nonInjectableParams.Length == parameters.Length)
            {
                for (int i = 0; i < nonInjectableParams.Length; i++)
                {
                    if (!nonInjectableParams[i].ParameterType.IsAssignableFrom(parameters[i].GetType()))
                        break;
                    if (i == nonInjectableParams.Length - 1)
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
        catch (MissingMemberException ex)
        {
            try
            {
                return ActivatorUtilities.CreateInstance<T>(Resolver.GetServiceProvider());
            }
            catch (Exception ex1)
            {
                throw new AggregateException("Failed to create page with ViewModel", ex, ex1);
            }
        }
    }

    private static bool IsRegisteredDependency(IServiceProvider serviceProvider, Type type)
    {
        if (type.IsPrimitive || type == typeof(string) || type.IsValueType)
        {
            return false;
        }

        try
        {
            var services = serviceProvider.GetServices(type);
            return services.Any();
        }
        catch (Exception)
        {
            return false;
        }
    }
}