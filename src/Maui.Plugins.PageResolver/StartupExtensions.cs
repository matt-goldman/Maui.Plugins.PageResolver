using Microsoft.Extensions.DependencyInjection;

namespace Maui.Plugins.PageResolver
{
    public static class StartupExtensions
    {
        /// <summary>
        /// Registers the services in the service collection with the page resolver
        /// </summary>
        /// <param name="sc"></param>
        public static void UsePageResolver(this IServiceCollection sc)
        {
            var sp = sc.BuildServiceProvider();
            Resolver.RegisterServiceProvider(sp);
        }
    }
}