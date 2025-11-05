using System.Threading.Tasks;

namespace Plugin.Maui.SmartNavigation.Behaviours;

/// <summary>
/// Defines a contract for handling initialization logic in a ViewModel when navigation occurs.
/// </summary>
/// <remarks>Implement this interface to perform setup or resource allocation when a ViewModel is first navigated
/// to or reactivated. Implementations should avoid long-running operations in initialization to ensure responsive navigation.</remarks>
public interface IViewModelLifecycle
{
    /// <summary>
    /// Performs asynchronous initialization logic when navigation occurs.
    /// </summary>
    /// <param name="isFirstNavigation">Indicates whether this is the first navigation to the component. Pass <see langword="true"/> for the initial
    /// navigation; otherwise, <see langword="false"/>.</param>
    /// <returns>A task that represents the asynchronous initialization operation.</returns>
    /// <remarks>This is called from an async void method in the NavigatedInitBehavior; you MUST handle exceptions in your implementation!</remarks>
    Task OnInitAsync(bool isFirstNavigation);
}