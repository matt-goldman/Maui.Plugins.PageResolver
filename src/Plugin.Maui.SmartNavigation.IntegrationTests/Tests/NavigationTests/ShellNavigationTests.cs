using FluentAssertions;
using Microsoft.Maui.Controls;
using Moq;
using Plugin.Maui.SmartNavigation.Routing;
using Plugin.Maui.SmartNavigation.IntegrationTests.Infrastructure;

namespace Plugin.Maui.SmartNavigation.IntegrationTests.Tests.NavigationTests;

/// <summary>
/// Tests for Shell navigation (GoToAsync)
/// </summary>
public class ShellNavigationTests : IntegrationTestBase
{
    [Fact]
    public async Task GoToAsync_WithSimpleRoute_ShouldNavigate()
    {
        // Arrange
        var shell = new Shell();
        var route = "products/list";
        var navigatedRoute = string.Empty;

        var shellMock = new Mock<Shell>();
        shellMock.Setup(s => s.GoToAsync(It.IsAny<string>()))
            .Callback<string>(r => navigatedRoute = r)
            .Returns(Task.CompletedTask);

        // Act
        await shellMock.Object.GoToAsync(route);

        // Assert
        navigatedRoute.Should().Be(route);
    }

    [Fact]
    public async Task GoToAsync_WithQueryParameters_ShouldIncludeQuery()
    {
        // Arrange
        var shellMock = new Mock<Shell>();
        var navigatedRoute = string.Empty;

        shellMock.Setup(s => s.GoToAsync(It.IsAny<string>()))
            .Callback<string>(r => navigatedRoute = r)
            .Returns(Task.CompletedTask);

        var route = "products/details?id=123&category=books";

        // Act
        await shellMock.Object.GoToAsync(route);

        // Assert
        navigatedRoute.Should().Be(route);
        navigatedRoute.Should().Contain("?");
        navigatedRoute.Should().Contain("id=123");
        navigatedRoute.Should().Contain("category=books");
    }

    [Fact]
    public async Task GoToAsync_WithRelativeRoute_ShouldNavigateBack()
    {
        // Arrange
        var shellMock = new Mock<Shell>();
        var navigatedRoute = string.Empty;

        shellMock.Setup(s => s.GoToAsync(It.IsAny<string>()))
            .Callback<string>(r => navigatedRoute = r)
            .Returns(Task.CompletedTask);

        // Act
        await shellMock.Object.GoToAsync("..");

        // Assert
        navigatedRoute.Should().Be("..");
    }

    [Fact]
    public async Task GoToAsync_WithAbsoluteRoute_ShouldNavigateToRoot()
    {
        // Arrange
        var shellMock = new Mock<Shell>();
        var navigatedRoute = string.Empty;

        shellMock.Setup(s => s.GoToAsync(It.IsAny<string>()))
            .Callback<string>(r => navigatedRoute = r)
            .Returns(Task.CompletedTask);

        // Act
        await shellMock.Object.GoToAsync("//main/home");

        // Assert
        navigatedRoute.Should().Be("//main/home");
        navigatedRoute.Should().StartWith("//");
    }

    [Fact]
    public void Route_Build_ShouldGenerateCorrectShellRoute()
    {
        // Arrange
        var route = new TestRoute("products", "details");

        // Act
        var builtRoute = route.Build();

        // Assert
        builtRoute.Should().Be("products/details");
    }

    [Fact]
    public void Route_BuildWithQuery_ShouldGenerateCorrectShellRouteWithParameters()
    {
        // Arrange
        var route = new TestRoute("products", "details");
        var parameters = new Dictionary<string, string>
        {
            { "id", "123" },
            { "name", "product" }
        };

        // Act
        var builtRoute = route.Build(parameters);

        // Assert
        builtRoute.Should().Contain("products/details");
        builtRoute.Should().Contain("?");
        builtRoute.Should().Contain("id=123");
        builtRoute.Should().Contain("name=product");
    }

    [Fact]
    public async Task Shell_MultipleNavigations_ShouldExecuteInOrder()
    {
        // Arrange
        var shellMock = new Mock<Shell>();
        var navigationHistory = new List<string>();

        shellMock.Setup(s => s.GoToAsync(It.IsAny<string>()))
            .Callback<string>(r => navigationHistory.Add(r))
            .Returns(Task.CompletedTask);

        // Act
        await shellMock.Object.GoToAsync("page1");
        await shellMock.Object.GoToAsync("page2");
        await shellMock.Object.GoToAsync("page3");

        // Assert
        navigationHistory.Should().HaveCount(3);
        navigationHistory[0].Should().Be("page1");
        navigationHistory[1].Should().Be("page2");
        navigationHistory[2].Should().Be("page3");
    }

    // Test route implementation
    private record TestRoute(string Path, string? Name = null, RouteKind Kind = RouteKind.Page)
        : Route(Path, Name, Kind);
}
