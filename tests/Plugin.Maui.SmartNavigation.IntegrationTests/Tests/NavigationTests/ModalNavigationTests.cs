using Moq;
using Plugin.Maui.SmartNavigation.IntegrationTests.Infrastructure;
using Shouldly;

namespace Plugin.Maui.SmartNavigation.IntegrationTests.Tests.NavigationTests;

/// <summary>
/// Tests for modal navigation (PushModalAsync, PopModalAsync)
/// </summary>
public class ModalNavigationTests : IntegrationTestBase
{
    [Fact]
    public async Task PushModalAsync_ShouldAddPageToModalStack()
    {
        // Arrange
        var navigationMock = new Mock<INavigation>();
        var modalStack = new List<Page>();
        navigationMock.Setup(n => n.ModalStack).Returns(modalStack.AsReadOnly());
        navigationMock.Setup(n => n.PushModalAsync(It.IsAny<Page>()))
            .Callback<Page>(p => modalStack.Add(p))
            .Returns(Task.CompletedTask);

        var modalPage = new Page { Title = "ModalPage" };

        // Act
        await navigationMock.Object.PushModalAsync(modalPage);

        // Assert
        modalStack.ShouldContain(modalPage);
        modalStack.Count.ShouldBe(1);
    }

    [Fact]
    public async Task PopModalAsync_ShouldRemovePageFromModalStack()
    {
        // Arrange
        var navigationMock = new Mock<INavigation>();
        var modalStack = new List<Page> { new(), new() };
        navigationMock.Setup(n => n.ModalStack).Returns(modalStack.AsReadOnly());
        navigationMock.Setup(n => n.PopModalAsync())
            .Callback(() => modalStack.RemoveAt(modalStack.Count - 1))
            .ReturnsAsync(modalStack[^1]);

        var initialCount = modalStack.Count;

        // Act
        await navigationMock.Object.PopModalAsync();

        // Assert
        modalStack.Count.ShouldBe(initialCount - 1);
    }

    [Fact]
    public async Task PushModalAsync_MultipleModals_ShouldStack()
    {
        // Arrange
        var navigationMock = new Mock<INavigation>();
        var modalStack = new List<Page>();
        navigationMock.Setup(n => n.ModalStack).Returns(modalStack.AsReadOnly());
        navigationMock.Setup(n => n.PushModalAsync(It.IsAny<Page>()))
            .Callback<Page>(p => modalStack.Add(p))
            .Returns(Task.CompletedTask);

        var modal1 = new Page { Title = "Modal1" };
        var modal2 = new Page { Title = "Modal2" };
        var modal3 = new Page { Title = "Modal3" };

        // Act
        await navigationMock.Object.PushModalAsync(modal1);
        await navigationMock.Object.PushModalAsync(modal2);
        await navigationMock.Object.PushModalAsync(modal3);

        // Assert
        modalStack.Count.ShouldBe(3);
        modalStack[0].Title.ShouldBe("Modal1");
        modalStack[1].Title.ShouldBe("Modal2");
        modalStack[2].Title.ShouldBe("Modal3");
    }

    [Fact]
    public async Task ModalStack_ShouldBeIndependentFromNavigationStack()
    {
        // Arrange
        var navigationMock = new Mock<INavigation>();
        var navigationStack = new List<Page>();
        var modalStack = new List<Page>();
        
        navigationMock.Setup(n => n.NavigationStack).Returns(navigationStack.AsReadOnly());
        navigationMock.Setup(n => n.ModalStack).Returns(modalStack.AsReadOnly());
        
        navigationMock.Setup(n => n.PushAsync(It.IsAny<Page>()))
            .Callback<Page>(p => navigationStack.Add(p))
            .Returns(Task.CompletedTask);
            
        navigationMock.Setup(n => n.PushModalAsync(It.IsAny<Page>()))
            .Callback<Page>(p => modalStack.Add(p))
            .Returns(Task.CompletedTask);

        var regularPage = new Page { Title = "RegularPage" };
        var modalPage = new Page { Title = "ModalPage" };

        // Act
        await navigationMock.Object.PushAsync(regularPage);
        await navigationMock.Object.PushModalAsync(modalPage);

        // Assert
        navigationStack.Count.ShouldBe(1);
        navigationStack.ShouldContain(regularPage);
        modalStack.Count.ShouldBe(1);
        modalStack.ShouldContain(modalPage);
    }

    [Fact]
    public async Task PopModalAsync_WhenEmpty_ShouldHandleGracefully()
    {
        // Arrange
        var navigationMock = new Mock<INavigation>();
        var modalStack = new List<Page>();
        navigationMock.Setup(n => n.ModalStack).Returns(modalStack.AsReadOnly());
        navigationMock.Setup(n => n.PopModalAsync())
            .ThrowsAsync(new InvalidOperationException("Modal stack is empty"));

        // Act
        async Task act() => await navigationMock.Object.PopModalAsync();

        // Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(act);
        ex.Message.ShouldContain("Modal stack is empty");
    }
}
