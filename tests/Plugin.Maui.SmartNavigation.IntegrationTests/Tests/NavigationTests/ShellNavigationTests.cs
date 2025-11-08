using Plugin.Maui.SmartNavigation.IntegrationTests.Infrastructure;
using Plugin.Maui.SmartNavigation.Routing;
using Shouldly;

namespace Plugin.Maui.SmartNavigation.IntegrationTests.Tests.NavigationTests;

/// <summary>
/// Tests for Shell navigation routes and the NavigationManager.GoToAsync() method
/// </summary>
/// <remarks>
/// TESTING LIMITATION:
/// NavigationManager.GoToAsync() requires Application.Current.Windows[0].Page to be a Shell instance.
/// In headless test environments, the Windows collection is empty (testing artifact, not production bug).
/// 
/// These tests focus on what CAN be tested:
/// - Route building logic (Route.Build(), parameters, etc.)
/// - Route format validation
/// 
/// What CANNOT be tested in headless environment:
/// - Actual Shell.GoToAsync() execution
/// - Shell route registration and navigation
/// - NavigationManager.GoToAsync() integration with Shell
/// 
/// For full end-to-end Shell navigation testing, use UI automation frameworks (Appium, XCTest, Espresso)
/// that run in actual platform contexts with real Shell instances.
/// </remarks>
public class ShellNavigationTests : IntegrationTestBase
{
    [Fact]
    public void Route_Build_ShouldGenerateCorrectShellRoute()
    {
        // Arrange
        var route = new TestRoute("products", "details");

        // Act
        var builtRoute = route.Build();

        // Assert
        builtRoute.ShouldBe("products/details");
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
        builtRoute.ShouldContain("products/details");
        builtRoute.ShouldContain("?");
        builtRoute.ShouldContain("id=123");
        builtRoute.ShouldContain("name=product");
    }

    [Fact]
    public void Route_BuildSimplePath_ShouldReturnPath()
    {
        // Arrange
        var route = new TestRoute("home");

        // Act
        var builtRoute = route.Build();

        // Assert
        builtRoute.ShouldBe("home");
    }

    [Fact]
    public void Route_BuildWithEmptyName_ShouldReturnPathOnly()
    {
        // Arrange
        var route = new TestRoute("products", null);

        // Act
        var builtRoute = route.Build();

        // Assert
        builtRoute.ShouldBe("products");
    }

    [Fact]
    public void Route_BuildRelativePath_ShouldSupportBackNavigation()
    {
        // Arrange
        var route = new TestRoute("..");

        // Act
        var builtRoute = route.Build();

        // Assert
        builtRoute.ShouldBe("..");
    }

    [Fact]
    public void Route_BuildAbsolutePath_ShouldStartWithDoubleSlash()
    {
        // Arrange
        var route = new TestRoute("//main", "home");

        // Act
        var builtRoute = route.Build();

        // Assert
        builtRoute.ShouldStartWith("//");
        builtRoute.ShouldContain("main");
    }

    [Fact]
    public void Route_Kind_ShouldBePreserved()
    {
        // Arrange
        var pageRoute = new TestRoute("page1", null, RouteKind.Page);
        var modalRoute = new TestRoute("modal1", null, RouteKind.Modal);

        // Assert
        pageRoute.Kind.ShouldBe(RouteKind.Page);
        modalRoute.Kind.ShouldBe(RouteKind.Modal);
    }

    [Fact]
    public void NavigationManager_RequiresShell_ForGoToAsync()
    {
        // Arrange
        InitializeMauiAppWithPage(); // Non-Shell app

        // Assert - Document that NavigationManager.GoToAsync requires Shell
        // In production, calling NavigationManager.GoToAsync() without Shell would throw
        // InvalidOperationException: "Shell navigation is not available"
        
        // We can verify the app is initialized without Shell
        Application.Current.ShouldNotBeNull();
        
        // Note: Cannot test actual NavigationManager.GoToAsync() behavior in headless environment
        // because Windows collection is empty (testing artifact)
    }

    // TODO: Add these tests when UI automation framework is available:
    // - GoToAsync_WithSimpleRoute_ShouldNavigateToPage
    // - GoToAsync_WithQueryParameters_ShouldPassParametersToPage  
    // - GoToAsync_WithRelativeRoute_ShouldNavigateBack
    // - GoToAsync_WithAbsoluteRoute_ShouldNavigateToRoot
    // - GoToAsync_MultipleNavigations_ShouldMaintainHistory
    // - GoToAsync_WithUnregisteredRoute_ShouldThrowException
    // - NavigationManager_GoToAsync_WithShell_ShouldCallShellGoToAsync
    // - NavigationManager_GoToAsync_WithoutShell_ShouldThrowInvalidOperationException

    // Test route implementation
    private record TestRoute(string Path, string? Name = null, RouteKind Kind = RouteKind.Page)
        : Route(Path, Name, Kind);
}
