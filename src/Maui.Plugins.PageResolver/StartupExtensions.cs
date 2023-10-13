using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Maui.Plugins.PageResolver;

public static class StartupExtensions
{
    /// <summary>
    /// Registers the services in the service collection with the page resolver
    /// </summary>
    /// <param name="sc"></param>
    /// <param name="UseParamaterisedViewModels">If true, the ViewModelResolver will be initialised with the calling assembly (required for passing ViewModel parameters in navigation). Disabled by default as it uses reflection and will have startup time impact.</param>
    public static void UsePageResolver(this IServiceCollection sc, bool? UseParamaterisedViewModels = false)
    {
        sc.TryAddEnumerable( ServiceDescriptor.Transient<IMauiInitializeService, Initializer>() );

        if (UseParamaterisedViewModels ?? false)
        {
            Resolver.InitialiseViewModelLookup(Assembly.GetCallingAssembly());
        }
    }

    /// <summary>
    /// Registers the services in the service collection with the page resolver
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="UseParamaterisedViewModels">If true, the ViewModelResolver will be initialised with the calling assembly (required for passing ViewModel parameters in navigation). Disabled by default as it uses reflection and will have startup time impact.</param>
    public static MauiAppBuilder UsePageResolver(this MauiAppBuilder builder, bool? UseParamaterisedViewModels = false)
    {
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Transient<IMauiInitializeService, Initializer>() );

        if (UseParamaterisedViewModels??false)
        {
            Resolver.InitialiseViewModelLookup(Assembly.GetCallingAssembly());
        }

        return builder;
    }

    /// <summary>
    /// Registers the services in the service collection and the page-to-ViewModel mappings with the page resolver. This overload is intended for use with the Source Generator.
    /// </summary>
    /// <param name="sc"></param>
    /// <param name="ViewModelMappings">A dictionary that provides Page to ViewModel mappings..</param>
    public static void UsePageResolver(this IServiceCollection sc, Dictionary<Type, Type> ViewModelMappings)
    {
        sc.TryAddEnumerable(ServiceDescriptor.Transient<IMauiInitializeService, Initializer>());

        Resolver.InitialiseViewModelLookup(ViewModelMappings);
    }
}
