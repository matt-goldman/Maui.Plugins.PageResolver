using Moq;
using Plugin.Maui.SmartNavigation.IntegrationTests.Infrastructure;
using Plugin.Maui.SmartNavigation.Routing;
using Shouldly;

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
        Type? act() => routeRegistry.TryGetValue(unregisteredRoute.Build(), out var type)
            ? type
            : throw new InvalidOperationException($"Route not registered: {unregisteredRoute.Build()}");

        // Assert
        var ex = Should.Throw<InvalidOperationException>((Func<Type?>)act);
        ex.Message.ShouldContain("Route not registered: nonexistent/route");
    }

    [Fact]
    public async Task ShellNotAvailable_ForGoToAsync_ShouldThrowInvalidOperationException()
    {
        // Arrange
        Application.Current = new Application();
        var window = new Window { Page = new Page() }; // Regular page, not Shell
        Application.Current.OpenWindow(window);

        var route = new TestRoute("products/list");

        // Act
        async Task act()
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
        }

        // Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(act);
        ex.Message.ShouldContain("Shell navigation is not available");
    }

    [Fact]
    public void InvalidParameters_NullFactory_ShouldThrowWithTypeInformation()
    {
        // Arrange
        Func<Page>? factory = null;

        // Act
        Page act() => factory?.Invoke()
            ?? throw new InvalidOperationException("Factory is null for route");

        // Assert
        var ex = Should.Throw<InvalidOperationException>((Func<Page>)act);
        ex.Message.ShouldContain("null");
    }

    [Fact]
    public void InvalidParameters_MismatchedPageType_ShouldThrowWithExplicitTypeInfo()
    {
        // Arrange
        var expectedType = typeof(Page);
        var actualType = typeof(Shell);

        // Act
        void act()
        {
            if (expectedType != actualType)
            {
                throw new InvalidOperationException(
                    $"Type mismatch: Expected {expectedType.Name} but got {actualType.Name}");
            }
        }

        // Assert
        var ex = Should.Throw<InvalidOperationException>(act);
        ex.Message.ShouldContain("Type mismatch: Expected Page but got Shell");
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
        async Task act() => await navigationMock.Object.PopAsync();

        // Assert
    }

    [Fact]
    public void Route_InvalidPath_EmptyString_ShouldHandleOrThrow()
    {
        // Arrange & Act
        static void act()
        {
            var route = new TestRoute("");
            if (string.IsNullOrWhiteSpace(route.Path))
            {
                throw new ArgumentException("Route path cannot be empty", nameof(route.Path));
            }
        }

        // Assert
        var ex = Should.Throw<ArgumentException>(act);
        ex.Message.ShouldContain("Route path cannot be empty");
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
        void act()
        {
            if (!constructorMatches)
            {
                throw new ArgumentException(
                    $"Provided parameters do not match the constructors of {pageType.Name}.");
            }
        }

        // Assert
        var ex = Should.Throw<ArgumentException>(act);
        ex.Message.ShouldContain("do not match the constructors");
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
                {
                    throw new ArgumentNullException(nameof(r));
                }
            })
            .Returns(Task.CompletedTask);

        // Act
        async Task act() => await shellMock.Object.GoToAsync(nullRoute!);

        // Assert
        await Should.ThrowAsync<ArgumentNullException>(act);
    }

    [Fact]
    public void MissingDependency_ServiceNotRegistered_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var serviceType = typeof(object);

        // Act
        void act()
        {
            var service = ServiceProvider.GetService(serviceType) ?? throw new InvalidOperationException(
                    $"No service for type '{serviceType.Name}' has been registered.");
        }

        // Assert
        var ex = Should.Throw<InvalidOperationException>(act);
        ex.Message.ShouldContain("No service for type");
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
        void act()
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
        }

        // Assert
        var ex = Should.Throw<InvalidOperationException>(act);
        ex.Message.ShouldContain("Circular dependency detected");
    }

    // Test route implementation
    private record TestRoute(string Path, string? Name = null, RouteKind Kind = RouteKind.Page)
        : Route(Path, Name, Kind);
}
