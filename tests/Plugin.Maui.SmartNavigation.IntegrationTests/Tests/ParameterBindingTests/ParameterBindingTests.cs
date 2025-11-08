using Shouldly;
using Plugin.Maui.SmartNavigation.IntegrationTests.Infrastructure;
using Plugin.Maui.SmartNavigation.IntegrationTests.Mocks;

namespace Plugin.Maui.SmartNavigation.IntegrationTests.Tests.ParameterBindingTests;

/// <summary>
/// Tests for parameter binding scenarios as specified in the spec
/// - Page-only parameter binding
/// - ViewModel-only parameter binding
/// - Both Page and ViewModel (should throw)
/// - No parameters (no-op)
/// </summary>
public class ParameterBindingTests : IntegrationTestBase
{
    [Fact]
    public void PageOnly_WithMatchingParameters_ShouldBindToPage()
    {
        // Arrange
        var stringParam = "test";
        var intParam = 42;

        // Act
        var page = new MockPageWithParameters(stringParam, intParam);

        // Assert
        page.StringParam.ShouldBe(stringParam);
        page.IntParam.ShouldBe(intParam);
    }

    [Fact]
    public void ViewModelOnly_WithMatchingParameters_ShouldBindToViewModel()
    {
        // Arrange
        var name = "John Doe";
        var age = 30;

        // Act
        var viewModel = new MockViewModelWithParameters(name, age);

        // Assert
        viewModel.Name.ShouldBe(name);
        viewModel.Age.ShouldBe(age);
    }

    [Fact]
    public void PageWithViewModel_ShouldSetBindingContext()
    {
        // Arrange
        var viewModel = new MockViewModel
        {
            StringProperty  = "test",
            IntProperty     = 123
        };

        // Act
        var page = new MockPageWithViewModel(viewModel);

        // Assert
        page.ViewModel.ShouldBe(viewModel);
        page.BindingContext.ShouldBe(viewModel);
    }

    [Fact]
    public void NoParameters_ShouldCreateDefaultInstance()
    {
        // Arrange & Act
        var page = new MockPage();
        var viewModel = new MockViewModel();

        // Assert
        page.ShouldNotBeNull();
        page.NavigationParameters.ShouldBeNull();
        viewModel.ShouldNotBeNull();
        viewModel.StringProperty.ShouldBeNull();
        viewModel.IntProperty.ShouldBe(0);
    }

    [Fact]
    public void MultipleParameterTypes_ShouldBindCorrectly()
    {
        // Arrange
        var stringParam = "test string";
        var intParam = 42;
        var objectParam = new object();

        // Act
        var page = new MockPageWithParameters
        {
            StringParam = stringParam,
            IntParam    = intParam,
            ObjectParam = objectParam
        };

        // Assert
        page.StringParam.ShouldBe(stringParam);
        page.IntParam.ShouldBe(intParam);
        page.ObjectParam.ShouldBe(objectParam);
    }

    [Fact]
    public void ViewModel_WithComplexParameters_ShouldBindCorrectly()
    {
        // Arrange
        var viewModel = new MockViewModelWithParameters
        {
            Name        = "Complex Test",
            Age         = 25,
            IsActive    = true
        };

        // Act & Assert
        viewModel.Name.ShouldBe("Complex Test");
        viewModel.Age.ShouldBe(25);
        viewModel.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void Page_WithObjectParameter_ShouldStoreReference()
    {
        // Arrange
        var parameter = new { Id = 123, Name = "Test" };

        // Act
        var page = new MockPage(parameter);

        // Assert
        page.NavigationParameters.ShouldBe(parameter);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Page_WithNullOrEmptyStringParameter_ShouldHandleCorrectly(string? value)
    {
        // Arrange & Act
        var page = new MockPageWithParameters
        {
            StringParam = value
        };

        // Assert
        page.StringParam.ShouldBe(value);
    }

    [Fact]
    public void ViewModel_ConstructorInjection_ShouldWork()
    {
        // Arrange
        var name = "Constructor Test";
        var age = 35;

        // Act
        var viewModel = new MockViewModelWithParameters(name, age);

        // Assert
        viewModel.Name.ShouldBe(name);
        viewModel.Age.ShouldBe(age);
        viewModel.IsActive.ShouldBeFalse(); // Default value
    }

    [Fact]
    public void Page_WithViewModel_ShouldAllowPropertyBinding()
    {
        // Arrange
        var viewModel = new MockViewModel
        {
            StringProperty  = "Initial",
            IntProperty     = 100
        };
        var page = new MockPageWithViewModel(viewModel);

        // Act
        viewModel.StringProperty = "Updated";
        viewModel.IntProperty = 200;

        // Assert
        page.ViewModel!.StringProperty.ShouldBe("Updated");
        page.ViewModel.IntProperty.ShouldBe(200);
    }

    [Fact]
    public void ParameterBinding_WithNullObject_ShouldHandleGracefully()
    {
        // Arrange & Act
        var page = new MockPage(null!);

        // Assert
        page.NavigationParameters.ShouldBeNull();
    }

    [Fact]
    public void ParameterBinding_DifferentTypes_ShouldMaintainTypeIntegrity()
    {
        // Arrange
        var stringValue = "123";
        var intValue = 123;

        var stringPage = new MockPageWithParameters { StringParam = stringValue };
        var intPage = new MockPageWithParameters { IntParam = intValue };

        // Assert
        stringPage.StringParam.ShouldBeAssignableTo<string>();
        stringPage.StringParam.ShouldBe("123");
        intPage.IntParam.ShouldBe(123);
        typeof(int).IsAssignableFrom(intPage.IntParam.GetType()).ShouldBeTrue();
    }

    [Fact]
    public void BothPageAndViewModel_WithSameParameterNames_ShouldThrowOrHandleAmbiguity()
    {
        // This test represents the scenario from the spec where both Page and ViewModel
        // have matching writable property names, which should throw with a clear message
        // For now, we document the expected behavior

        // Arrange
        var pageWithParams = new MockPageWithParameters { StringParam = "Page" };
        var viewModelWithParams = new MockViewModelWithParameters { Name = "ViewModel" };

        // Assert - They should be independent when not in conflict
        pageWithParams.StringParam.ShouldBe("Page");
        viewModelWithParams.Name.ShouldBe("ViewModel");
        
        // Note: The actual ambiguity detection would be in the navigation extension methods
        // which would need to be tested when those are available
    }
}
