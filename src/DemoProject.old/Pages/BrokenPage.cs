namespace DemoProject.Pages;

/// <summary>
/// This page has a dependency on a service that should not be registered. Runtime exception expected when navigating to this page.
/// </summary>
public class BrokenPage : ContentPage
{
	public BrokenPage(IIgnoredService ignoredService)
	{
		Content = new VerticalStackLayout
		{
			Children = {
				new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = ignoredService.GetHello()
				}
			}
		};
	}
}