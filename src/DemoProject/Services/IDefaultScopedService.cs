namespace DemoProject.Services;

public interface IDefaultScopedService
{
    int GetCount();
    void IncreaseCount(int? count = null);
}

public class DefaultScopedService : IDefaultScopedService
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
