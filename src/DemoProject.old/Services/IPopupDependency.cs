namespace DemoProject.Services;

public interface IPopupDependencyService
{
    string GetMessage();
    string GetParamMessage(string param);
}

public class PopupDependencyService : IPopupDependencyService
{
    public string GetMessage()
    {
        return "Hello from PopupDependencyService!";
    }

    public string GetParamMessage(string param)
    {
        return $"Hello from PopupDependencyService with param: {param}";
    }
}
