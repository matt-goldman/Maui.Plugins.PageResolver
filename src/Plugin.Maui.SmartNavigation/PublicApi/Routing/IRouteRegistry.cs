#nullable enable
#pragma warning disable IDE0130 // Namespace does not match folder structure - intended
namespace Plugin.Maui.SmartNavigation.Routing;
#pragma warning restore IDE0130 // Namespace does not match folder structure

using System;
using Microsoft.Maui.Controls;

public interface IRouteRegistry
{
    void Register(Route route, Type pageType);
    void Register(Route route, Func<Page> factory); // DI factory if needed
    Type? Resolve(Route route);
}

// TODO: This probably isn't necessary
