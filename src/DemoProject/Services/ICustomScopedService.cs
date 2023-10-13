using Maui.Plugins.PageResolver.Attributes;

namespace DemoProject.Services;

public interface ICustomScopedService : IDefaultScopedService
{
}

[Transient]
public class CustomScopedService : ICustomScopedService
{
    private int _count = 0;

    public int GetCount()
    {
        return _count;
    }

    public void IncreaseCount(int? count = null)
    {
        if (count.HasValue)
        {
            _count += count.Value;
        }
        else
        {
            _count++;
        }
    }
}
