using Maui.Plugins.PageResolver.Attributes;

namespace DemoProject.Services;

public interface IIgnoredService
{
    string GetHello();
}

[Ignore]
public class IgnoredService : IIgnoredService
{
    public string GetHello() => "Hello";
}
