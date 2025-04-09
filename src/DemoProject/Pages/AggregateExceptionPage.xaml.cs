namespace DemoProject.Pages;

public partial class AggregateExceptionPage : ContentPage
{
    public AggregateExceptionPage(AggregateExceptionViewModel vm)
    {
        InitializeComponent();

        throw new MissingMemberException("This is a test exception");
    }
}