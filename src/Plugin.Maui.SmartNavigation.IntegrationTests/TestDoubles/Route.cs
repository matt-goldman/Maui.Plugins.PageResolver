namespace Plugin.Maui.SmartNavigation.Routing;

/// <summary>
/// Specifies the type of navigation route used within the application.
/// </summary>
public enum RouteKind { Page, Modal, Popup, External }

/// <summary>
/// Represents an abstract route definition for testing
/// </summary>
public abstract record Route(
    string Path,
    string? Name = null,
    RouteKind Kind = RouteKind.Page
)
{
    /// <summary>
    /// Builds a route string by combining the base path and name, optionally appending a query string.
    /// </summary>
    public string Build(string? query = null)
    {
        var baseRoute = string.IsNullOrWhiteSpace(Name) ? Path : $"{Path}/{Name}";
        return string.IsNullOrWhiteSpace(query) ? baseRoute : $"{baseRoute}?{query}";
    }

    /// <summary>
    /// Builds a query string using the specified key-value parameters.
    /// </summary>
    public string Build(Dictionary<string, string> parameters)
    {
        if (parameters == null || parameters.Count == 0)
            return Build();

        var query = string.Join("&", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        return Build(query);
    }
}
