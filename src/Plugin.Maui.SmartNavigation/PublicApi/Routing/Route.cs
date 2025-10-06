#nullable enable
#pragma warning disable IDE0130 // Namespace does not match folder structure - intended
namespace Plugin.Maui.SmartNavigation.Routing;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public enum RouteKind { Page, Modal, Popup, External }

public abstract record Route(
    string Path,                    // e.g. "products/details"
    string? Name = null,
    RouteKind Kind = RouteKind.Page
)
{
    // TODO: What to do with query? Should probably be more opinionated about how it is structured
    public string Build(object? query = null)
     => string.IsNullOrWhiteSpace(Name) ? Path : $"{Path}/{Name}";
}
