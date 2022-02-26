[![NuGet Status](https://img.shields.io/nuget/v/Goldie.MauiPlugins.PageResolver.svg?style=flat)](https://www.nuget.org/packages/Goldie.MauiPlugins.PageResolver/) 

# MAUI PageResolver
A simple and lightweight page resolver for use in .NET MAUI projects.

While we're waiting for the full MVVM frameworks we know and love to be updated for MAUI, or if you just want a simple page resolver with DI without using a full MVVM framework (or if you want to use MVU), this package will let you navigate to fully resolved pages, with view models and dependencies, by calling:

```cs
await Navigation.PushAsync<MyPage>();
```

# Getting Started

Check out the for full instructions on [using PageResolver](https://github.com/matt-goldman/Maui.Plugins.PageResolver/wiki/Using-the-PageResolver) and [using auto-dependency registration](https://github.com/matt-goldman/Maui.Plugins.PageResolver/wiki/Using-Auto-registration-(experimental)).

# Notes

Thanks to [William Liebenberg](https://github.com/william-liebenberg) for his input getting this off the ground.    
Please note .NET MAUI has not been released yet, so some bumps should be expected. If you find any problems with PageResolver, feedback, issues or pull requests would be most welcome.

# TODO
- [x] Use reflection / code generation to automatically register pages and view models
- [x] (Pending C# 10) Add a global using for this package
  - [x] Added to readme
- [x] Set up GitHub Action to publish package
- [ ] (Possibly) change namespace
