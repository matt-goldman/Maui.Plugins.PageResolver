using Microsoft.Maui.Controls;
using Plugin.Maui.SmartNavigation.Behaviours;

namespace Plugin.Maui.SmartNavigation;

public class InitContentPage : ContentPage
{
    public InitContentPage()
    {
        Behaviors.Add(new NavigatedInitBehavior());
    }
}