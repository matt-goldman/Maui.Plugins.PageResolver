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
    /// <param name="services"></param>
    /// <param name="UseParamaterisedViewModels">If true, the ViewModelResolver will be initialised with the calling assembly (required for passing ViewModel parameters in navigation). Disabled by default as it uses reflection and will have startup time impact.</param>
    public static void UsePageResolver(this IServiceCollection services, bool? UseParamaterisedViewModels = false)
    {
        services.TryAddEnumerable( ServiceDescriptor.Transient<IMauiInitializeService, Initializer>() );

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
    /// <param name="services"></param>
    /// <param name="ViewModelMappings">A dictionary that provides Page to ViewModel mappings..</param>
    public static void UsePageResolver(this IServiceCollection services, Dictionary<Type, Type> ViewModelMappings)
    {
        services.TryAddEnumerable(ServiceDescriptor.Transient<IMauiInitializeService, Initializer>());

        Resolver.InitialiseViewModelLookup(ViewModelMappings);
    }

    /// <summary>
    /// Clears the current page to ViewModel mappings and replaces with the specified mappings
    /// </summary>
    /// <param name="services"></param>
    /// <param name="ViewModelMappings"></param>
    /// <returns></returns>
    public static IServiceCollection AddViewModelMappings(this IServiceCollection services, Dictionary<Type, Type> ViewModelMappings)
    {
        Resolver.InitialiseViewModelLookup(ViewModelMappings);
        return services;
    }

    /// <summary>
    /// Adds the specified page to ViewModel mappings to the existing mappings (replaces items with existing key)
    /// </summary>
    /// <param name="ViewModelMappings"></param>
    public static void AddViewModelMappings(Dictionary<Type, Type> ViewModelMappings)
    {
        Resolver.AddMappingRange(ViewModelMappings);
    }
}
