using Shouldly;
using Plugin.Maui.SmartNavigation.Routing;
using Plugin.Maui.SmartNavigation.IntegrationTests.Infrastructure;

namespace Plugin.Maui.SmartNavigation.IntegrationTests.Tests.RouteTests;

/// <summary>
/// Tests for Route resolution and registration
/// </summary>
public class RouteResolutionTests : IntegrationTestBase
{
    [Fact]
    public void Route_ShouldBuildBasicPath()
    {
        // Arrange
        var route = new TestRoute("products/list");

        // Act
        var result = route.Build();

        // Assert
        result.ShouldBe("products/list");
    }

    [Fact]
    public void Route_ShouldBuildPathWithName()
    {
        // Arrange
        var route = new TestRoute("products", "details");

        // Act
        var result = route.Build();

        // Assert
        result.ShouldBe("products/details");
    }

    [Fact]
    public void Route_ShouldBuildPathWithQueryString()
    {
        // Arrange
        var route = new TestRoute("products/details");

        // Act
        var result = route.Build("id=123&category=books");

        // Assert
        result.ShouldBe("products/details?id=123&category=books");
    }

    [Fact]
    public void Route_ShouldBuildPathWithDictionaryParameters()
    {
        // Arrange
        var route = new TestRoute("products/details");
        var parameters = new Dictionary<string, string>
        {
            { "id", "123" },
            { "category", "books" }
        };

        // Act
        var result = route.Build(parameters);

        // Assert
        result.ShouldContain("products/details?");
        result.ShouldContain("id=123");
        result.ShouldContain("category=books");
    }

    [Fact]
    public void Route_ShouldHandleNullOrEmptyQuery()
    {
        // Arrange
        var route = new TestRoute("products/list");

        // Act
        var resultNull = route.Build((string?)null);
        var resultEmpty = route.Build("");
        var resultWhitespace = route.Build("   ");

        // Assert
        resultNull.ShouldBe("products/list");
        resultEmpty.ShouldBe("products/list");
        resultWhitespace.ShouldBe("products/list");
    }

    [Fact]
    public void Route_ShouldHandleEmptyDictionary()
    {
        // Arrange
        var route = new TestRoute("products/list");
        var emptyParams = new Dictionary<string, string>();

        // Act
        var result = route.Build(emptyParams);

        // Assert
        result.ShouldBe("products/list");
    }

    [Theory]
    [InlineData(RouteKind.Page)]
    [InlineData(RouteKind.Modal)]
    [InlineData(RouteKind.Popup)]
    [InlineData(RouteKind.External)]
    public void Route_ShouldPreserveRouteKind(RouteKind kind)
    {
        // Arrange & Act
        var route = new TestRoute("test", Kind: kind);

        // Assert
        route.Kind.ShouldBe(kind);
    }

    [Fact]
    public void Route_DefaultKindShouldBePage()
    {
        // Arrange & Act
        var route = new TestRoute("test");

        // Assert
        route.Kind.ShouldBe(RouteKind.Page);
    }

    // Test route implementation for testing
    private record TestRoute(string Path, string? Name = null, RouteKind Kind = RouteKind.Page) 
        : Route(Path, Name, Kind);
}
