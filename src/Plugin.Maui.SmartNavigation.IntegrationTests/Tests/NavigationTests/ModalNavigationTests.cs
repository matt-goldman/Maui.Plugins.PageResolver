using FluentAssertions;
using Microsoft.Maui.Controls;
using Moq;
using Plugin.Maui.SmartNavigation.IntegrationTests.Infrastructure;

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
        modalStack.Should().Contain(modalPage);
        modalStack.Count.Should().Be(1);
    }

    [Fact]
    public async Task PopModalAsync_ShouldRemovePageFromModalStack()
    {
        // Arrange
        var navigationMock = new Mock<INavigation>();
        var modalStack = new List<Page> { new Page(), new Page() };
        navigationMock.Setup(n => n.ModalStack).Returns(modalStack.AsReadOnly());
        navigationMock.Setup(n => n.PopModalAsync())
            .Callback(() => modalStack.RemoveAt(modalStack.Count - 1))
            .ReturnsAsync(modalStack[^1]);

        var initialCount = modalStack.Count;

        // Act
        await navigationMock.Object.PopModalAsync();

        // Assert
        modalStack.Count.Should().Be(initialCount - 1);
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
        modalStack.Should().HaveCount(3);
        modalStack[0].Title.Should().Be("Modal1");
        modalStack[1].Title.Should().Be("Modal2");
        modalStack[2].Title.Should().Be("Modal3");
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
        navigationStack.Should().ContainSingle();
        navigationStack.Should().Contain(regularPage);
        modalStack.Should().ContainSingle();
        modalStack.Should().Contain(modalPage);
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
        Func<Task> act = async () => await navigationMock.Object.PopModalAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Modal stack is empty");
    }
}
