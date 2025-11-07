using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Plugin.Maui.SmartNavigation.Services;
using System;

#pragma warning disable IDE0130 // Namespace does not match folder structure - intended for public API
namespace Plugin.Maui.SmartNavigation;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class ResolveViewModel<T> : IMarkupExtension<T>
{
    public T ViewModel { get; set; }
    
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

[ContentProperty(nameof(ViewModel))]
public class ResolveViewModel : IMarkupExtension
{
    public Type ViewModel { get; set; }

    public object ProvideValue(IServiceProvider serviceProvider)
    {
        var sp = Resolver.GetServiceProvider();
        var result = sp.GetRequiredService(ViewModel);

        return result;
    }
}
