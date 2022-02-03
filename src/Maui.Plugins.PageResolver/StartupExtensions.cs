using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;

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
#
        /// <summary>
        /// Registers the services in the service collection with the page resolver
        /// </summary>
        /// <param name="builder"></param>
        public static MauiAppBuilder UsePageResolver(this MauiAppBuilder builder)
        {
            var sp = builder.Services.BuildServiceProvider();
            Resolver.RegisterServiceProvider(sp);

            return builder;
        }
    }
}