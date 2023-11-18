namespace DemoProject.ViewModels;

public class VmParamViewModel
{
    public string NameParam { get; set; }

    public string NameFromServiceParam { get; set; }

    public VmParamViewModel(INameService nameService, string name)
    {
        NameParam = name;
        NameFromServiceParam = nameService.GetName();
    }
}
