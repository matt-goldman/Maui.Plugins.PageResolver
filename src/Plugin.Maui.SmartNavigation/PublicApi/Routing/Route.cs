#nullable enable
#pragma warning disable IDE0130 // Namespace does not match folder structure - intended
using System.Collections.Generic;
using System.Linq;

namespace Plugin.Maui.SmartNavigation.Routing;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Specifies the type of navigation route used within the application.
/// </summary>
/// <remarks>Use this enumeration to indicate whether a navigation action should open a standard page, a modal
/// dialog, a popup window, or an external resource. The selected value determines how the navigation is presented to
/// the user.</remarks>
public enum RouteKind { Page, Modal, Popup, External }

/// <summary>
/// Represents an abstract route definition, including its path, optional name, and route kind. Provides methods to
/// construct route URLs with optional query parameters.
/// </summary>
/// <remarks>Use the Build methods to generate a route URL with optional query parameters. The route URL is
/// constructed by combining the path and name, followed by any query string if provided. This type is intended to be
/// inherited for specific route implementations.</remarks>
/// <param name="Path">The base path segment of the route. This value is used as the primary identifier for the route and must not be null.</param>
/// <param name="Name">An optional name segment appended to the route path. If not specified, only the base path is used.</param>
/// <param name="Kind">The type of route, such as page or API, which determines how the route is handled within the application. Defaults
/// to RouteKind.Page.</param>
public abstract record Route(
    string Path,
    string? Name = null,
    RouteKind Kind = RouteKind.Page
)
{
    /// <summary>
    /// Builds a route string by combining the base path and name, optionally appending a query string.
    /// </summary>
    /// <param name="query">An optional query string to append to the route. If null or whitespace, no query string is added.</param>
    /// <returns>A string representing the constructed route, including the query string if provided.</returns>
    public string Build(string? query = null)
    {
        var baseRoute = string.IsNullOrWhiteSpace(Name) ? Path : $"{Path}/{Name}";

        return string.IsNullOrWhiteSpace(query) ? baseRoute : $"{baseRoute}?{query}";
    }

    /// <summary>
    /// Builds a query string using the specified key-value parameters.
    /// </summary>
    /// <param name="paramaters">A dictionary containing the query parameters to include in the string. Keys represent parameter names and values
    /// represent parameter values. If null or empty, a default query string is built.</param>
    /// <returns>A string representing the constructed query with the provided parameters, formatted as key-value pairs separated
    /// by ampersands.</returns>
    public string Build(Dictionary<string, string> paramaters)
    {
        if (paramaters == null || paramaters.Count == 0)
            return Build();

        var query = string.Join("&", paramaters.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        
        return Build(query);
    }
}
