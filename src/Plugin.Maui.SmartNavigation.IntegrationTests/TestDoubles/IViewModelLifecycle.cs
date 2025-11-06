namespace Plugin.Maui.SmartNavigation.Behaviours;

/// <summary>
/// Defines a contract for handling initialization logic in a ViewModel when navigation occurs.
/// </summary>
public interface IViewModelLifecycle
{
    /// <summary>
    /// Performs asynchronous initialization logic when navigation occurs.
    /// </summary>
    Task OnInitAsync(bool isFirstNavigation);
}
