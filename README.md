# Plugin.Maui.SmartNavigation

[![NuGet Status](https://img.shields.io/nuget/v/Plugin.Maui.SmartNavigation.svg?style=flat)](https://www.nuget.org/packages/Plugin.Maui.SmartNavigation/)
[![Nuget](https://img.shields.io/nuget/dt/Plugin.Maui.SmartNavigation)](https://www.nuget.org/packages/Plugin.Maui.SmartNavigation)

A simple, predictable navigation library for .NET MAUI.
It resolves pages and view models through DI, supports both Shell and non-Shell apps, and gives you a single, type-safe API for every navigation scenario.

> **Note:** This library was renamed from `Maui.Plugins.PageResolver` to `Plugin.Maui.SmartNavigation` in v3.0 for .NET 10. See the migration guide below.

## Quick Start

### Register the plugin

```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseSmartNavigation(); // add this

        return builder.Build();
    }
}
```

### Navigate to a page

```csharp
public class MyViewModel
{
    private readonly INavigationManager _navigation;

    public MyViewModel(INavigationManager navigation)
    {
        _navigation = navigation;
    }

    public Task ShowDetails()
        => _navigation.PushAsync<DetailsPage>();
}
```

Or via extension methods on `INavigation`:

```csharp
await Navigation.PushAsync<MyPage>();
```

### Passing parameters

```csharp
await _navigation.PushAsync<MyPage>(myParam1, "bob", 4);
```

### Modal navigation

```csharp
await _navigation.PushModalAsync<SettingsPage>();
await _navigation.PopModalAsync();
```

### Shell routing (type-safe)

```csharp
await _navigation.GoToAsync(new Route("details"));
```

**Note:** Ths is for illustration and not the recommended usage of the `Route` record type - see wiki page "Best Practices" (coming soon).

### Going back

```csharp
await _navigation.GoBackAsync();
```

`GoBackAsync` automatically chooses the correct navigation context (Shell, modal, or the stack).

## Why SmartNavigation?

* Works with **Shell and non-Shell** navigation using the same API
* Fully **DI-resolved pages and view models**
* No framework, no magic – just **type-safe navigation**
* Optional **async initialisation lifecycle** for ViewModels
* Plays nicely with MVVM, MVU, or no pattern at all
* Minimal setup, no ceremony

## INavigationManager

Designed to take the guesswork out of picking the right lifecycle method for initialising ViewModels. It gives you something close to `OnInitializedAsync` in Blazor.

SmartNavigation abstracts MAUI’s three navigation systems (Shell, navigation stack, and modal stack) into one unified service:

```csharp
public interface INavigationManager
{
    Task GoToAsync(Route route, string? query = null);
    Task GoBackAsync();
    Task PushAsync<TPage>(object? args = null) where TPage : Page;
    Task PopAsync();
    Task PushModalAsync<TPage>(object? args = null) where TPage : Page;
    Task PopModalAsync();
}
```

## ViewModel Lifecycle (NavigatedInitBehaviour)

Implement `IViewModelLifecycle` on your ViewModel and SmartNavigation will run async initialisation automatically when the page is navigated to:

```csharp
public class MyViewModel : IViewModelLifecycle
{
    public Task OnInitAsync(bool isFirstNavigation)
    {
        if (isFirstNavigation)
            return LoadDataAsync();

        return Task.CompletedTask;
    }
}
```

Attach via XAML:

```xml
<ContentPage xmlns:behaviours="clr-namespace:Plugin.Maui.SmartNavigation.Behaviours;assembly=Plugin.Maui.SmartNavigation">
    <ContentPage.Behaviours>
        <behaviours:NavigatedInitBehaviour />
    </ContentPage.Behaviours>
</ContentPage>
```

## Source Generator (Opt-In)

The optional source generator can register pages, view models, and services automatically.
Enable it:

```csharp
[UseAutoDependencies]
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        return MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseAutodependencies() // Generated extension method
            .Build();
    }
}
```

## Lifetime Attributes

Defaults:

* Pages – transient
* ViewModels – transient
* Services – singleton

(Must follow naming conventions - see wiki (coming soon))

Override default lifetimes using attributes:

```csharp
[Transient]
public class CustomScopedService : ICustomScopedService { }
```

## Shell vs Non-Shell Navigation

SmartNavigation works seamlessly with both:

* **Shell** – `GoToAsync(Route)` with type-safe routes
* **Navigation stack** – `PushAsync<TPage>()`
* **Modal** – `PushModalAsync<TPage>()`
* **Automatic back logic** – `GoBackAsync()` picks the correct behaviour

No special configuration is required.

## Migration from PageResolver 2.x

### Breaking changes

1. **Package rename**

```xml
<!-- Old -->
<PackageReference Include="Goldie.MauiPlugins.PageResolver" Version="2.x" />

<!-- New -->
<PackageReference Include="Plugin.Maui.SmartNavigation" Version="3.0" />
```

2. **Namespaces changed**

```csharp
// Old
using Maui.Plugins.PageResolver;

// New
using Plugin.Maui.SmartNavigation;
```

3. **Source generator is now opt-in**

```csharp
[UseAutoDependencies]
```

4. **Remove old bootstrapping**
   `UsePageResolver()` is no longer required or present.

### Migration checklist

* [ ] Update NuGet package
* [ ] Update namespaces
* [ ] Add `[UseAutoDependencies]` if using the generator
* [ ] Update any custom mappings or extensions
* [ ] Remove any calls to `UsePageResolver()`
* [ ] Test navigation flows (API surface unchanged where not noted)

## Demo Project & Examples

The demo project shows:

* Basic navigation
* Parameter passing
* Modal navigation
* Shell routing
* ViewModel lifecycle
* Popup support (Mopups)
* Service scopes
* Navigation patterns for modular apps

See: `src/DemoProject`

## Video Walkthrough

**Note:** This is for the legacy version. New video coming soon.

<a href="http://www.youtube.com/watch?feature=player_embedded&v=qx8A4zIe9dU" target="_blank">
  <img src="http://img.youtube.com/vi/qx8A4zIe9dU/hqdefault.jpg" alt="Watch the video" />
</a>

## Documentation

See the wiki (coming soon) for guides, examples, and best practices.
