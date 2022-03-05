using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace Maui.Plugins.PageResolver
{
    public static class NavigationExtensions
    {
        /// <summary>
        /// Resolves a page of type T (must inhetit from ContentPage) and pushes a new instance onto the navigation stack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="navigation"></param>
        /// <returns></returns>
        public static async Task PushAsync<T>(this INavigation navigation) where T : ContentPage
        {
            var resolvedPage = Resolver.Resolve<T>();

            await navigation.PushAsync(resolvedPage);
        }

        /// <summary>
        /// Resolves a page of type T (must inhetit from ContentPage) and pushes a new instance onto the navigation stack
        /// </summary>
        /// <typeparam name="T">The type of the page to be resolved</typeparam>
        /// <param name="navigation"></param>
        /// <param name="parameters">The constructor parameters expected by the page to be resolved</param>
        /// <returns></returns>
        public static async Task PushAsync<T>(this INavigation navigation, params object[] parameters) where T : ContentPage
        {
            var resolvedPage = ActivatorUtilities.CreateInstance<T>(Resolver.GetServiceProvider(), parameters);

            await navigation.PushAsync(resolvedPage);
        }

        /// <summary>
        /// Resolves a page of type T (must inhetit from ContentPage) and pushes a new modal instance onto the navigation stack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="navigation"></param>
        /// <returns></returns>
        public static async Task PushModalAsync<T>(this INavigation navigation) where T : ContentPage
        {
            var resolvedPage = Resolver.Resolve<T>();

            await navigation.PushModalAsync(resolvedPage);
        }

        /// <summary>
        /// Resolves a page of type T (must inhetit from ContentPage) and pushes a new modal instance onto the navigation stack
        /// </summary>
        /// <typeparam name="T">The type of the page to be resolved</typeparam>
        /// <param name="navigation"></param>
        /// <param name="parameters">The constructor parameters expected by the page to be resolved</param>
        /// <returns></returns>
        public static async Task PushModalAsync<T>(this INavigation navigation, params object[] parameters) where T : ContentPage
        {
            var resolvedPage = ActivatorUtilities.CreateInstance<T>(Resolver.GetServiceProvider(), parameters);

            await navigation.PushModalAsync(resolvedPage);
        }
    }
}