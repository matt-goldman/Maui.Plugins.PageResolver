using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DemoProject.ViewModels;

public class BaseViewModel : INotifyPropertyChanged
{
    public INavigation Navigation { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertChanged([CallerMemberName] string propertyName = "")
    {
        var changed = PropertyChanged;
        if (changed == null)
            return;

        changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
