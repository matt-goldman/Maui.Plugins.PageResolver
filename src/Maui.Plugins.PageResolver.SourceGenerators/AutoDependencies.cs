﻿using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maui.Plugins.PageResolver.SourceGenerators
{
    [Generator]
    public class AutoDependencies : ISourceGenerator
    {
        private Dictionary<string, HashSet<ITypeSymbol>> _dependencies;

        private HashSet<string> _namespaces;

        public void Execute(GeneratorExecutionContext context)
        {
            StringBuilder lb = new StringBuilder();

            Log.Init(lb);

            try
            {
                // TODO: get all referenced assemblies
                var assembly = context.Compilation.Assembly;

                Log.WriteLine($"Scanning assembly: {assembly.Name}");

                var types = GetAllTypes(context.Compilation.SourceModule.ContainingAssembly.GlobalNamespace);

                InitialiseDependencies(types);

                Log.WriteLine("Getting MauiProgram...");

                var mauiProgramName = $"{assembly.Name}.MauiProgram";

                Log.WriteLine($"Global namespace: {context.Compilation.Assembly.GlobalNamespace.Name}");

                var mauiProgram = context.Compilation
                    .GetTypeByMetadataName(mauiProgramName);

                if (mauiProgram is null)
                {
                    Log.WriteLine("MauiProgram not found");
                    throw new Exception("MauiProgram not found.");
                }

                Log.WriteLine($"Found main method: {mauiProgram.Name}");

                StringBuilder sourceBuilder = new StringBuilder();

                foreach (var ns in _namespaces)
                {
                    sourceBuilder.AppendLine(ns);
                }

                sourceBuilder.Append($@"// ---------------
// <auto-generated>
//   Generated by the MauiPageResolver Auto-registration module.
//   https://github.com/matt-goldman/Maui.Plugins.PageResolver
// </auto-generated>
// ---------------

namespace {mauiProgram.ContainingNamespace.ToDisplayString()};

public static class PageResolverExtensions
{{
    private static Dictionary<Type, Type>() ViewModelMappings = new();

    public static MauiAppBuilder UseAutodependencies(this MauiAppBuilder builder)
    {{
");

                // add page registrations
                foreach (var page in _dependencies["Pages"])
                {
                    string lifetime = _dependencies["ExplicitSingletons"].Contains(page) ? "Singleton" : "Transient";

                    sourceBuilder.AppendLine($"         builder.Services.Add{lifetime}<{page.Name}>();");
                }

                // add ViewModel registrations
                foreach (var vm in _dependencies["ViewModels"])
                {
                    string lifetime = _dependencies["ExplicitSingletons"].Contains(vm) ? "Singleton" : "Transient";

                    sourceBuilder.AppendLine($"         builder.Services.Add{lifetime}<{vm.Name}>();");
                }

                // add Service registrations
                foreach (var service in _dependencies["Services"])
                {
                    var ifName = $"I{service.Name}";

                    string lifetime = _dependencies["ExplicitTransients"].Contains(service) ? "Transient" : "Singleton";

                    var abstraction = _dependencies["Abstractions"].Where(a => a.Name == ifName).FirstOrDefault();

                    if (abstraction is null)
                    {
                        sourceBuilder.AppendLine($"         builder.Services.Add{lifetime}<{service.Name}>();");
                    }
                    else
                    {
                        sourceBuilder.AppendLine($"         builder.Services.Add{lifetime}<{ifName}, {service.Name}>();");
                    }
                }

                var mappings = GetPageToViewModelMappings();

                foreach (var mapping in mappings)
                {
                    sourceBuilder.AppendLine($"         ViewModelMappings.Add(typeof({mapping.Key.Name}), typeof({mapping.Value.Name}));");
                }

                sourceBuilder.AppendLine($"         builder.Services.UsePageResolver(ViewModelMappings);");

                sourceBuilder.AppendLine($"         return builder;");

                // close the partial method and class
                sourceBuilder.Append(@"    }
}");

                // generate the source file
                var typeName = mauiProgram.Name;

                context.AddSource("PageResolverExtensions.g.cs", sourceBuilder.ToString());
                Log.WriteLine($"Generated: PageResolverExtensions.g.cs, {sourceBuilder}");
            }
            catch (Exception ex)
            {
                Log.WriteLine($"{ex}");
                Log.WriteLine($"{ex.StackTrace}");
            }
            finally
            {
                Log.FlushLog();
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // no initialisation
        }

        private void InitialiseDependencies(IEnumerable<ITypeSymbol> types)
        {
            var comparer = SymbolEqualityComparer.Default;

            _dependencies = new Dictionary<string, HashSet<ITypeSymbol>>
            {
                { "Pages", new HashSet<ITypeSymbol>(comparer) },
                { "ViewModels", new HashSet<ITypeSymbol> (comparer) },
                { "Services", new HashSet<ITypeSymbol>(comparer) },
                { "Abstractions", new HashSet<ITypeSymbol>(comparer) },
                { "ExplicitSingletons", new HashSet<ITypeSymbol>(comparer) },
                { "ExplicitTransients", new HashSet<ITypeSymbol>(comparer) }
            };

            _namespaces = new HashSet<string>
            {
                "using Maui.Plugins.PageResolver;"
            };

            var singletons = types.Where(type =>
                    type.GetAttributes().Any(ad =>
                    ad.AttributeClass.ToDisplayString() == "Maui.Plugins.PageResolver.Attributes.SingletonAttribute"));
            _dependencies["ExplicitSingletons"] = new HashSet<ITypeSymbol>(singletons, comparer);

            var transients = types.Where(type =>
                type.GetAttributes().Any(ad =>
                ad.AttributeClass.ToDisplayString() == "Maui.Plugins.PageResolver.Attributes.TransientAttribute"));
            _dependencies["ExplicitTransients"] = new HashSet<ITypeSymbol>(transients, comparer);

            var pages = types.Where(t => t.TypeKind == TypeKind.Class && t.Name.EndsWith("Page"));
            _dependencies["Pages"] = new HashSet<ITypeSymbol>(pages, comparer);

            var viewModels = types.Where(t => t.TypeKind == TypeKind.Class && t.Name.EndsWith("ViewModel"));
            _dependencies["ViewModels"] = new HashSet<ITypeSymbol>(viewModels, comparer);

            var services = types.Where(t => t.TypeKind == TypeKind.Class && t.Name.EndsWith("Service"));
            _dependencies["Services"] = new HashSet<ITypeSymbol>(services, comparer);

            var abstractions = types.Where(t => t.TypeKind == TypeKind.Interface && t.Name.EndsWith("Service"));
            _dependencies["Abstractions"] = new HashSet<ITypeSymbol>(abstractions, comparer);

            Log.WriteLine($"Found {pages.Count()} pages.");
            Log.WriteLine($"Found {viewModels.Count()} ViewModels.");
            Log.WriteLine($"Found {services.Count()} Services.");
            Log.WriteLine($"Found {abstractions.Count()} interfaces.");

            // add all the using statements
            foreach (var page in pages)
            {
                if (page is null)
                    Log.WriteLine($"No type {page} found");

                var pageNamespace = $"using {page.ContainingNamespace.ToDisplayString()};";

                _namespaces.Add(pageNamespace);

                Log.WriteLine($"Found {page.Name} in {pageNamespace}");
            }

            foreach (var vm in viewModels)
            {
                var vmNamespace = $"using {vm.ContainingNamespace.ToDisplayString()};";

                _namespaces.Add(vmNamespace);

                Log.WriteLine($"Found {vm.Name} in {vmNamespace}");
            }

            foreach (var service in services)
            {
                var serviceNamespace = $"using {service.ContainingNamespace.ToDisplayString()};";

                _namespaces.Add(serviceNamespace);

                Log.WriteLine($"Found {service.Name} in {serviceNamespace}");
            }

            foreach (var i in abstractions)
            {
                var iNamespace = $"using {i.ContainingNamespace.ToDisplayString()};";

                _namespaces.Add(iNamespace);

                Log.WriteLine($"Found {i.Name} in {iNamespace}");
            }
        }

        private Dictionary<Type, Type> GetPageToViewModelMappings()
        {
            var VMLookup = new Dictionary<Type, Type>();

            foreach (var page in _dependencies["Pages"])
            {
                var matches = _dependencies["ViewModels"].Where(vm =>
                               vm.Name == $"{page.Name}ViewModel" || vm.Name == page.Name.Substring(0, page.Name.Length - 4) + "ViewModel").ToList();

                if (matches.Count == 1)
                    VMLookup.Add(page.GetType(), matches[0].GetType());
            }

            return VMLookup;
        }

        private static IEnumerable<ITypeSymbol> GetAllTypes(INamespaceSymbol root)
        {
            foreach (var namespaceOrTypeSymbol in root.GetMembers())
            {
                if (namespaceOrTypeSymbol is INamespaceSymbol @namespace) foreach (var nested in GetAllTypes(@namespace)) yield return nested;

                else if (namespaceOrTypeSymbol is ITypeSymbol type) yield return type;
            }
        }
    }
}
