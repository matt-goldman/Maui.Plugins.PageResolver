using System.Threading.Tasks;

namespace Plugin.Maui.SmartNavigation.Behaviours;

public interface IViewModelLifecycle
{
    Task OnInitAsync();
}