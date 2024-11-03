﻿using Mopups.Interfaces;
using Mopups.Pages;

namespace Maui.Plugins.PageResolver;

public static class MopupExtensions
{
    public static async Task PushAsync<T>(this IPopupNavigation navigation) where T: PopupPage
    {
        var popup = Resolver.Resolve<T>() as PopupPage
            ?? throw new ArgumentException("Could not resolve popup page");

        await navigation.PushAsync(popup, true);
    }

    public static async Task PushAsync<T>(this IPopupNavigation navigation, params object[] parameters) where T : PopupPage
    {
        var popup = NavigationExtensions.ResolvePage<T>(parameters) as PopupPage
            ?? throw new ArgumentException("Could not resolve popup page");

        await navigation.PushAsync(popup, true);
    }
}
