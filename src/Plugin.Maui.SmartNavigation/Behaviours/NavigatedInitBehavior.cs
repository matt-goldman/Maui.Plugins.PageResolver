#nullable enable
using Microsoft.Maui.Controls;
using System;

namespace Plugin.Maui.SmartNavigation.Behaviours;

public class NavigatedInitBehavior : Behavior<Page>
{
    private bool _ran = false;
    
    protected override void OnAttachedTo(Page page)
    {
        page.NavigatedTo += OnNavigatedTo;
        base.OnAttachedTo(page);
    }

    protected override void OnDetachingFrom(Page page)
    {
        page.NavigatedTo -= OnNavigatedTo;
        base.OnDetachingFrom(page);
    }
    
    private async void OnNavigatedTo(object? sender, NavigatedToEventArgs e)
    {
        if (sender is Page { BindingContext: IViewModelLifecycle viewModel })
        {
            var isFirstNavigation = !_ran;

            await viewModel.OnInitAsync(isFirstNavigation);

            _ran = true;
        }
    }
}
