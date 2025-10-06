#nullable enable
using Microsoft.Maui.Controls;

namespace Plugin.Maui.SmartNavigation.Behaviours;

public class ViewModelInitBehavior : Behavior<Page>
{
    private bool _ran;
    
    protected override void OnAttachedTo(Page page)
    {
        page.NavigatedTo += OnNavigatedTo;
        base.OnAttachedTo(page);
    }

    protected override void OnDetachingFrom(Page page)
    {
        base.OnDetachingFrom(page);
    }
    
    private async void OnNavigatedTo(object? sender, NavigatedToEventArgs e)
    {
        // TODO: Implement async fire and forget
        if (_ran) return;

        _ran = true;

        if (sender is Page { BindingContext: IViewModelInit viewModel })
        {
            await viewModel.InitializeAsync();
        }
    }
}
