namespace DemoProject.Popups.ViewModels;

public class ParamPopupViewModel
{
    public string Message { get; set; }

    public ParamPopupViewModel(string param, IPopupDependencyService dependency)
    {
        Message = dependency.GetParamMessage(param);
    }
}
