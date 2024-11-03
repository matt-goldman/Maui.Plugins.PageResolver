namespace DemoProject.Popups.ViewModels;

public class PopupViewModel
{
    public string Message { get; set; }

    public PopupViewModel(IPopupDependencyService dependency)
    {
        Message = dependency.GetMessage();
    }
}
