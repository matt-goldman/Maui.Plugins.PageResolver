using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Maui.Plugins.PageResolver;

internal static partial class Resolver
{
    private static IServiceScope scope;

    internal static readonly Dictionary<Type, Type> ViewModelLookup = new();

    internal static void InitialiseViewModelLookup(Assembly assembly)
    {
        var pages = assembly.DefinedTypes.Where(t => t.IsClass && t.Name.EndsWith("Page"));

        var viewModels = assembly.DefinedTypes.Where(t => t.IsClass && t.Name.EndsWith("ViewModel"));

        foreach (var page in pages)
        {
            var matches = viewModels.Where(vm =>
                           vm.Name == $"{page.Name}ViewModel" || vm.Name == page.Name.Substring(0, page.Name.Length - 4) + "ViewModel").ToList();

            if (matches.Count == 1)
                ViewModelLookup.Add(page, matches[0]);
        }
    }

    internal static void InitialiseViewModelLookup(Dictionary<Type, Type> ViewModelMappings)
    {
        ViewModelLookup.Clear();

        foreach (var mapping in ViewModelMappings)
        {
            ViewModelLookup.Add(mapping.Key, mapping.Value);
        }
    }

    internal static Type GetViewModelType(Type pageType)
    {
        if (ViewModelLookup.ContainsKey(pageType))
            return ViewModelLookup[pageType];

        return null;
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

    internal static void AddMappingRange(Dictionary<Type, Type> mappings)
    {
        foreach (var mapping in mappings)
        {
            ViewModelLookup[mapping.Key] = mapping.Value;
        }
    }
}
