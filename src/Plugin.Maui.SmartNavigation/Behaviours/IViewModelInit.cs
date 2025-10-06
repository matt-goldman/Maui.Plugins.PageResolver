using System.Threading.Tasks;

namespace Plugin.Maui.SmartNavigation.Behaviours;

public interface IViewModelInit
{
    Task InitializeAsync();
}