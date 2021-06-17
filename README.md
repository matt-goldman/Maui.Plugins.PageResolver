# MAUI PageResolver
A simple and lightweight page resolver for use in .NET MAUI projects.

While we're waiting for the full MVVM frameworks we know and love to be updated for MAUI, or if you just want a simple page resolver with DI without using a full MVVM framework (or if you want to use MVU), this package will let you navigate to fully resolved pages, with view models and dependencies, by calling:

```cs
await Navigation.PushAsync<MyPage>();
```

# Getting Started

## Step 1: Install Nuget package
Install the Nuget package

```cs
dotnet add package Goldie.MauiPlugins.PageResolver
```

## Step 2: Register dependencies
Currently, your services, view models, and pages all need to be registered in the service collection. Update the Configure method in your Startup.cs as follows

```cs
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Compatibility;
// Add reference to PageResolver
using Maui.Plugins.PageResolver;
// Add any required references to your services, view models, etc.
using MyApp.Services;
using MyApp.ViewModels;
using MyApp.Views;

namespace MyApp
{
    public class Startup : IStartup
    {
        public void Configure(IAppHostBuilder appBuilder)
        {
            appBuilder
                .UseFormsCompatibility()
                .UseMauiApp<App>()
                // Add ConfigureServices
                .ConfigureServices(services =>
                {
                    // register your services
                    services.AddSingleton<IMyService, MyService>();

                    // register your view models
                    services.AddTransient<MyViewModel>();

                    // register your views
                    services.AddTransient<MyPage>();

                    // register the page resolver
                    services.UsePageResolver();
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
        }
    }
}
```

## Step 3: Inject your dependencies

Use constructor injection to add your dependencies to your pages and view models. E.g.:

```cs
public class MyPage
{
    public MyViewModel ViewModel { get; set; }

    public MyPage(MyViewModel viewModel)
    {
        ViewModel = viewModel;
        BindingContext = ViewModel;
    }
}
```
And in your view models:

```cs
public class MyViewModel
{
    public IMyService Service { get; set; }

    public MyViewModel(IMyService myService)
    {
        Service = myService;
    }
}
```

## Step 4: Use in your code

In your code, navigate to your page by calling:

```cs
await Navigation.PushAsync<MyPage>();
```

This will return a fully resolved instance of `MyPage` including all dependencies.

Modal pages are also supported:

```cs
await Navigation.PushModalAsync<MyPage>();
```

# Notes

This is just something I put together for myself (with some input from [William Liebenberg](https://github.com/william-liebenberg)), but thought it might be useful to others. If you use it and it's helpful, great. If not, please remember it's an early attempt at doing something useful for a preview version of MAUI. If you have comments or suggestions, feedback is welcome.

# TODO
- [ ] Use refelction / code generation to automatically register pages and view models
- [ ] (Pending C# 10) Add a global using for this package
- [ ] Set up GitHub Action to publish package
- [ ] (Possibly) change namespace used