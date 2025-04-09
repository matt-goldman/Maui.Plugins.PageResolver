namespace DemoProject.ViewModels;

public class AggregateExceptionViewModel
{
    public string NameParam { get; set; }

    public string NameFromServiceParam { get; set; }

    public AggregateExceptionViewModel(INameService nameService, string name)
    {
        NameParam = name;
        NameFromServiceParam = nameService.GetName();
    }
}