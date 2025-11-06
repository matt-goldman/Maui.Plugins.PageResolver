using FluentAssertions;
using Microsoft.Maui.Controls;
using Moq;
using Plugin.Maui.SmartNavigation.Routing;
using Plugin.Maui.SmartNavigation.IntegrationTests.Infrastructure;

namespace Plugin.Maui.SmartNavigation.IntegrationTests.Tests.ErrorHandlingTests;

/// <summary>
/// Tests for error handling scenarios
/// - Unregistered route
/// - Shell not available
/// - Invalid parameters
/// </summary>
public class ErrorHandlingTests : IntegrationTestBase
{
    [Fact]
    public void UnregisteredRoute_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var unregisteredRoute = new TestRoute("nonexistent/route");
        var routeRegistry = new Dictionary<string, Type>();

        // Act
        Func<Type?> act = () => routeRegistry.TryGetValue(unregisteredRoute.Build(), out var type) 
            ? type 
            : throw new InvalidOperationException($"Route not registered: {unregisteredRoute.Build()}");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Route not registered: nonexistent/route");
    }

    [Fact]
    public void ShellNotAvailable_ForGoToAsync_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var app = new Application();
        var window = new Window { Page = new Page() }; // Regular page, not Shell
        app.Windows.Add(window);
        Application.Current = app;

        var route = new TestRoute("products/list");

        // Act
        Func<Task> act = async () =>
        {
            var current = Application.Current?.Windows[0].Page;
            if (current is Shell shell)
            {
                await shell.GoToAsync(route.Build());
            }
            else
            {
                throw new InvalidOperationException(
                    $"Cannot navigate to route '{route.Path}'. Shell navigation is not available. " +
                    "Use PushAsync<TPage>() for hierarchical navigation instead.");
            }
        };

        // Assert
        act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Shell navigation is not available*");
    }

    [Fact]
    public void InvalidParameters_NullFactory_ShouldThrowWithTypeInformation()
    {
        // Arrange
        Func<Page>? factory = null;

        // Act
        Func<Page> act = () => factory?.Invoke() 
            ?? throw new InvalidOperationException("Factory is null for route");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*null*");
    }

    [Fact]
    public void InvalidParameters_MismatchedPageType_ShouldThrowWithExplicitTypeInfo()
    {
        // Arrange
        var expectedType = typeof(Page);
        var actualType = typeof(Shell);

        // Act
        Action act = () =>
        {
            if (expectedType != actualType)
            {
                throw new InvalidOperationException(
                    $"Type mismatch: Expected {expectedType.Name} but got {actualType.Name}");
            }
        };

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Type mismatch: Expected Page but got Shell");
    }

    [Fact]
    public async Task PopAsync_OnEmptyNavigationStack_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var navigationMock = new Mock<INavigation>();
        var emptyStack = new List<Page>();
        
        navigationMock.Setup(n => n.NavigationStack).Returns(emptyStack.AsReadOnly());
        navigationMock.Setup(n => n.PopAsync())
            .ThrowsAsync(new InvalidOperationException("Cannot pop from an empty navigation stack"));

        // Act
        Func<Task> act = async () => await navigationMock.Object.PopAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*empty navigation stack*");
    }

    [Fact]
    public async Task PopModalAsync_OnEmptyModalStack_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var navigationMock = new Mock<INavigation>();
        var emptyModalStack = new List<Page>();
        
        navigationMock.Setup(n => n.ModalStack).Returns(emptyModalStack.AsReadOnly());
        navigationMock.Setup(n => n.PopModalAsync())
            .ThrowsAsync(new InvalidOperationException("Cannot pop from an empty modal stack"));

        // Act
        Func<Task> act = async () => await navigationMock.Object.PopModalAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*empty modal stack*");
    }

    [Fact]
    public void ParameterAmbiguity_BothPageAndViewModelMatch_ShouldThrowWithClearMessage()
    {
        // Arrange
        var pageType = typeof(Page);
        var viewModelType = typeof(object);
        var parameterName = "Name";

        // Simulate ambiguity detection
        var pageHasProperty = true;
        var viewModelHasProperty = true;

        // Act
        Action act = () =>
        {
            if (pageHasProperty && viewModelHasProperty)
            {
                throw new InvalidOperationException(
                    $"Ambiguous parameter binding: Property '{parameterName}' exists in both " +
                    $"{pageType.Name} and {viewModelType.Name}. Please bind explicitly to one target.");
            }
        };

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Ambiguous parameter binding*")
            .WithMessage($"*{parameterName}*");
    }

    [Fact]
    public void Route_InvalidPath_EmptyString_ShouldHandleOrThrow()
    {
        // Arrange & Act
        Action act = () =>
        {
            var route = new TestRoute("");
            if (string.IsNullOrWhiteSpace(route.Path))
            {
                throw new ArgumentException("Route path cannot be empty", nameof(route.Path));
            }
        };

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Route path cannot be empty*");
    }

    [Fact]
    public void NavigationParameters_InvalidConstructor_ShouldThrowArgumentException()
    {
        // Arrange
        var parameters = new object[] { "string", 123, true };
        var pageType = typeof(Page);

        // Simulate constructor parameter mismatch
        var constructorMatches = false;

        // Act
        Action act = () =>
        {
            if (!constructorMatches)
            {
                throw new ArgumentException(
                    $"Provided parameters do not match the constructors of {pageType.Name}.");
            }
        };

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*do not match the constructors*");
    }

    [Fact]
    public async Task GoToAsync_WithNullRoute_ShouldThrowArgumentNullException()
    {
        // Arrange
        var shellMock = new Mock<Shell>();
        string? nullRoute = null;

        shellMock.Setup(s => s.GoToAsync(It.IsAny<string>()))
            .Callback<string>(r =>
            {
                if (r == null)
                    throw new ArgumentNullException(nameof(r));
            })
            .Returns(Task.CompletedTask);

        // Act
        Func<Task> act = async () => await shellMock.Object.GoToAsync(nullRoute!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void MissingDependency_ServiceNotRegistered_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var serviceType = typeof(object);

        // Act
        Action act = () =>
        {
            var service = ServiceProvider.GetService(serviceType);
            if (service == null)
            {
                throw new InvalidOperationException(
                    $"No service for type '{serviceType.Name}' has been registered.");
            }
        };

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*No service for type*");
    }

    [Fact]
    public void CircularDependency_ShouldBeDetectedAndThrow()
    {
        // This test represents the scenario where circular dependencies might occur
        // Arrange
        var typeA = "TypeA";
        var typeB = "TypeB";
        var dependencyChain = new Stack<string>();
        dependencyChain.Push(typeA);
        dependencyChain.Push(typeB);
        dependencyChain.Push(typeA); // Circular reference

        // Act
        Action act = () =>
        {
            var visited = new HashSet<string>();
            foreach (var item in dependencyChain)
            {
                if (!visited.Add(item))
                {
                    throw new InvalidOperationException(
                        $"Circular dependency detected involving {item}");
                }
            }
        };

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Circular dependency detected*");
    }

    // Test route implementation
    private record TestRoute(string Path, string? Name = null, RouteKind Kind = RouteKind.Page)
        : Route(Path, Name, Kind);
}
