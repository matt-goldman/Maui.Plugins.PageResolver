namespace DemoProject.Popups.ViewModels;

public class ParamPopupViewModel
{
    public string Message { get; set; }

    public ParamPopupViewModel(IPopupDependencyService dependency, string param)
    {
        Message = dependency.GetParamMessage(param);
    }
}
