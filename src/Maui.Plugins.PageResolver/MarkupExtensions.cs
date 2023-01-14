using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Xaml;
using System;

namespace Maui.Plugins.PageResolver;

public class ResolveViewModel<T> : IMarkupExtension<T>
{
    public string MyProperty { get; set; }

    public T ProvideValue(IServiceProvider serviceProvider)
    {
        var sp = Resolver.GetServiceProvider();
        var result = sp.GetRequiredService<T>();

        return result;
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return ProvideValue(serviceProvider);
    }
}
