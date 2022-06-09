using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Maui.Hosting;

namespace Maui.Plugins.PageResolver
{
    public static class Resolver
    {
        internal class Initializer : IMauiInitializeService
        {
#region Implementation of IMauiInitializeService

            /// <inheritdoc />
            public void Initialize( IServiceProvider services )
            {
                if ( Resolver.scope == null )
                {
                    Resolver.RegisterServiceProvider( services );
                }
            }

#endregion
        }

        private static IServiceScope scope;

        /// <summary>
        /// Registers the service provider and creates a dependency scope
        /// </summary>
        /// <param name="sp"></param>
        internal static void RegisterServiceProvider(IServiceProvider sp)
        {
            scope = sp.CreateScope();
        }

        /// <summary>
        /// Returns a resolved instance of the requested type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal static T Resolve<T>() where T : class
        {
            var result = scope.ServiceProvider.GetRequiredService<T>();

            return result;
        }

        internal static IServiceProvider GetServiceProvider()
        {
            return scope.ServiceProvider;
        }
    }
}
