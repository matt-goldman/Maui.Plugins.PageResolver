# Plugin.Maui.SmartNavigation v2 for .NET 10 — Spec

## 1. Context

PageResolver evolves into **Plugin.Maui.SmartNavigation**. The goal is to provide a small, focused navigation and lifecycle layer that ties MAUI DI and CT.MVVM style ViewModels together without becoming an MVVM framework.

## 2. Objectives

* Strongly typed navigation for Shell and non‑Shell.
* Predictable ViewModel lifecycle initialisation with behaviours.
* Zero magic strings for routes.
* Keep API small, testable, and framework‑agnostic.

## 3. Non‑goals

* Do not ship ViewModel base classes.
* Do not own the DI container beyond registrations.
* Do not add messaging/event aggregator.
* Do not scaffold templates or a full framework.

## 4. Summary of user‑visible changes

1. **Rename**: package and namespaces to `Plugin.Maui.SmartNavigation`. PageResolver remains as a shim for a sunset period.
2. **Navigation service**: introduce `INavigationManager` with Shell and stack/modal operations.
3. **Smart routes**: new neutral `Route` type with centralised registration and query builder.
4. **Attribute inversion**: source generator is opt‑in via `[AutoDependencies]`. `UseAutoDependencies()` still applies generated registrations.
5. **Lifecycle behaviours**: ship three behaviours for VM initialisation. (New feature.)

## 5. Package layout

* **NuGet ID**: `Plugin.Maui.SmartNavigation`
* **Assemblies/namespaces**

  * `Plugin.Maui.SmartNavigation` (core)
  * `Plugin.Maui.SmartNavigation.Routing`
  * `Plugin.Maui.SmartNavigation.Behaviors`
  * `Plugin.Maui.SmartNavigation.Analyzers` (optional, separate package)

## 6. Public API

### 6.1 Route

```csharp
namespace Plugin.Maui.SmartNavigation.Routing;

public enum RouteKind { Page, Modal, Popup, External }

public sealed record Route(
    string Path,                    // e.g. "products/details"
    string? Name = null,
    RouteKind Kind = RouteKind.Page
)
{
    public string Build(object? query = null); // builds path?x=y using public properties
}
```

### 6.2 Navigation service

```csharp
public interface INavigationManager
{
    // Shell
    Task GoToAsync(Route route, object? query = null);
    Task GoBackAsync();

    // Stack
    Task PushAsync<TPage>(object? args = null) where TPage : Page;
    Task PopAsync();

    // Modal
    Task PushModalAsync<TPage>(object? args = null, bool wrapInNav = true) where TPage : Page;
    Task PopModalAsync();

    // Optional convenience
    Task SmartBackAsync(); // Pops modal if present else Shell ".." else PopAsync
}
```

**Notes**

* Works with Shell present or absent. Shell path is used when available, otherwise the registry maps `Route` to page types and falls back to `PushAsync`.
* Keep `Push*` even in Shell apps for scenarios like small flows or DI‑heavy screens that are not routes.

### 6.3 Registration

```csharp
public interface IRouteRegistry
{
    void Register(Route route, Type pageType);
    void Register(Route route, Func<Page> factory); // DI factory if needed
    Type? Resolve(Route route);
}
```

**Module/feature organisation is caller‑defined**

```csharp
public static class Routes
{
    public static class Products
    {
        public static readonly Route List    = new("products/list");
        public static readonly Route Details = new("products/details");
    }
}
```

### 6.4 Behaviours

```csharp
public interface IAsyncInitializable { Task InitializeAsync(); }
public interface IViewModelLifecycle { Task OnInitAsync(); Task OnDeinitAsync(); }

// Run once after first render/Loaded
public sealed class ViewModelInitOnLoadedBehavior : Behavior<Page> { /* wires Page.Loaded */ }

// Run once after navigation completes (Shell NavigatedTo)
public sealed class ViewModelInitOnNavigatedToBehavior : Behavior<Page> { /* wires NavigatedTo */ }

// Run on appear/disappear every time
public sealed class ViewModelLifecycleBehavior : Behavior<Page> { /* wires Appearing/Disappearing */ }
```

### 6.5 App builder extensions

```csharp
public sealed record SmartNavOptions(bool PreferShell = true);

public static class SmartNavigationAppBuilderExtensions
{
    public static MauiAppBuilder UseSmartNavigation(this MauiAppBuilder b, SmartNavOptions? opt = null);
    public static MauiAppBuilder UseAutoDependencies(this MauiAppBuilder b); // applies generated registrations
}
```

## 7. Source generator

### 7.1 Old behaviour

* Generator ran by default. Attribute `[NoAutoDependencies]` disabled it.

### 7.2 New behaviour

* Generator is **opt‑in** with `[AutoDependencies]` placed on `MauiProgram` or another assembly‑level target.
* Emitted code contains DI registrations discovered via conventions:

  * Pages, ViewModels, and Services based on naming or explicit attributes.
  * Optional `[SmartRoute]` attribute on pages to emit route fields.

```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly)]
public sealed class AutoDependenciesAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public sealed class SmartRouteAttribute : Attribute
{
    public SmartRouteAttribute(string path) { Path = path; }
    public string Path { get; }
    public string? Group { get; set; } // e.g. "Products"
    public RouteKind Kind { get; set; } = RouteKind.Page;
}
```

### 7.3 Emission

* `AutoDependencies.g.cs` with `void Apply(IServiceCollection services)` that the builder calls from `UseAutoDependencies()`.
* Optional `Routes.g.cs` that emits a static `Routes` class grouped by `Group`.

### 7.4 Opt‑out per type

* `[IgnoreAutoDependency]` attribute to skip registration for a specific type (renamed from previous `[Ignore]` attribute).

## 8. Param binding rules

* Navigation binding accepts an anonymous object or record.
* Apply to Page first, else to ViewModel.
* If both targets contain at least one matching writable property name, throw with a clear message to avoid ambiguity.
* Allow dictionary fallback for advanced cases.

## 9. Error handling

* Unregistered route: throw `InvalidOperationException("Route not registered: {path}")`.
* Shell not available for `GoToAsync`: fall back to `IRouteRegistry` resolve + `PushAsync`.
* Null factory result or mismatched page type: throw with explicit type information.

## 10. Backwards compatibility

* ~~PageResolver 2.x ships as a shim package that depends on SmartNavigation and uses type‑forwarders where possible.~~
* ~~Obsolete legacy APIs with messages pointing to SmartNavigation equivalents.~~
* ~~Behaviour names are new, no replacement in PageResolver.~~

Backward compatibility will not be maintained

## 11. Usage examples

### 11.1 Shell routes

```csharp
builder.UseSmartNavigation()
       .UseAutoDependencies();

await nav.GoToAsync(Routes.Products.Details, new { id = productId });
```

### 11.2 Stack and modal

```csharp
await nav.PushAsync<ProductDetailsPage>(new { Id = productId });
await nav.PushModalAsync<LoginPage>();
```

### 11.3 Behaviours

```xml
<ContentPage xmlns:sn="clr-namespace:Plugin.Maui.SmartNavigation.Behaviors">
  <ContentPage.Behaviors>
    <sn:ViewModelInitOnLoadedBehavior />
  </ContentPage.Behaviors>
</ContentPage>
```

## 12. Analyser rules (optional package)

* **SN0001**: Disallow raw string routes at call sites when a `Route` exists.
* **SN0002**: Route registered outside the central registry.
* **SN0003**: Behaviour used without implementing the required VM interface.
* Code‑fixes to replace strings with `Route` fields.

## 13. Testing

* Route → page resolution and fallbacks.
* Param binding: page only, VM only, both (throws), none (no‑op).
* Lifecycle ordering on Loaded, NavigatedTo, Appearing across iOS/Android/Windows.
* Modal and Shell back stacks do not interfere. `SmartBackAsync` chooses the right stack.

## 14. Versioning and compat

* SmartNavigation targets .NET 10.
* PageResolver 2.x references SmartNavigation and is marked for deprecation in README.

## 15. Docs and comms

* Blog post announcing rename, routes, navigation service, behaviours, attribute inversion, and migration note.
* README quick start, Shell vs non‑Shell guide, behaviours overview, and recipes.
* NuGet icon and short description aligned with the “remove papercuts” message.

## 16. Timeline

* Week 1: finish API, generator inversion, behaviours.
* Week 2: samples, docs, icon, analyzers v0.
* Week 3: release candidate, migration validation on a sample app.

## 17. Open questions

* Should `Route.Kind` influence default navigation style automatically, or remain advisory only?
* Emit `Routes` by default when `[SmartRoute]` is present, or behind a generator option flag?
* Provide a Mopups helper in core or via an optional extension package?
