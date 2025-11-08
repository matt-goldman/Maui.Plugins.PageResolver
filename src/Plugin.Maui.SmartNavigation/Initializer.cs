using System;
using Microsoft.Maui.Hosting;
using Plugin.Maui.SmartNavigation.Services;

namespace Plugin.Maui.SmartNavigation;

internal class Initializer : IMauiInitializeService
{
#region Implementation of IMauiInitializeService

    /// <inheritdoc />
    public void Initialize(IServiceProvider services)
    {
        Resolver.RegisterServiceProvider(services);
    }

#endregion
}