using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.Hosting;

namespace Maui.Plugins.PageResolver;

public static class StartupExtensions
{
    /// <summary>
    /// Registers the services in the service collection with the page resolver
    /// </summary>
    /// <param name="sc"></param>
    public static void UsePageResolver(this IServiceCollection sc)
    {
        sc.TryAddEnumerable( ServiceDescriptor.Transient<IMauiInitializeService, Initializer>() );
    }

    /// <summary>
    /// Registers the services in the service collection with the page resolver
    /// </summary>
    /// <param name="builder"></param>
    public static MauiAppBuilder UsePageResolver(this MauiAppBuilder builder)
    {
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Transient<IMauiInitializeService, Initializer>() );

        return builder;
    }
}
