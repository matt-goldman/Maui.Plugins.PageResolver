using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Maui.Plugins.PageResolver;

public static partial class Resolver
{
    private static IServiceScope scope;

    private static readonly Dictionary<Type, Type> ViewModelLookup = new();

    static partial void InitialiseViewModelLookup();

    static Resolver()
    {
        InitialiseViewModelLookup();
    }

    /// <summary>
    /// Registers the service provider and creates a dependency scope
    /// </summary>
    /// <param name="sp"></param>
    internal static void RegisterServiceProvider(IServiceProvider sp)
    {
        scope ??= sp.CreateScope();
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
