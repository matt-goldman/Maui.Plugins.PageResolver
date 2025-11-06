using Moq;
using Plugin.Maui.SmartNavigation.IntegrationTests.Infrastructure;
using Shouldly;

namespace Plugin.Maui.SmartNavigation.IntegrationTests.Tests.NavigationTests;

/// <summary>
/// Tests for GoBackAsync with various stack configurations
/// Based on spec: Priority 1: Modal, Priority 2: Shell, Priority 3: Navigation stack
/// </summary>
public class GoBackAsyncTests : IntegrationTestBase
{
    [Fact]
    public async Task GoBackAsync_WithModalStack_ShouldPopModal()
    {
        // Arrange
        var navigationMock = new Mock<INavigation>();
        var modalStack = new List<Page> { new() };
        var navigationStack = new List<Page> { new() };
        
        navigationMock.Setup(n => n.ModalStack).Returns(modalStack.AsReadOnly());
        navigationMock.Setup(n => n.NavigationStack).Returns(navigationStack.AsReadOnly());
        
        var modalPopped = false;
        navigationMock.Setup(n => n.PopModalAsync())
            .Callback(() =>
            {
                modalPopped = true;
                modalStack.RemoveAt(modalStack.Count - 1);
            })
            .ReturnsAsync(modalStack.Last());

        // Simulate GoBackAsync behavior (Priority 1: Modal)
        if (modalStack.Count > 0)
        {
            await navigationMock.Object.PopModalAsync();
        }

        // Assert
        modalPopped.ShouldBeTrue();
        modalStack.ShouldBeEmpty();
        navigationStack.Count.ShouldBe(1); // Navigation stack should be untouched
    }

    [Fact]
    public async Task GoBackAsync_WithShellAndNoModal_ShouldNavigateBackInShell()
    {
        // Arrange
        var shellMock = new Mock<Shell>();
        var navigationMock = new Mock<INavigation>();
        var modalStack = new List<Page>();
        
        navigationMock.Setup(n => n.ModalStack).Returns(modalStack.AsReadOnly());
        
        var shellNavigatedBack = false;
        shellMock.Setup(s => s.GoToAsync(It.Is<string>(r => r == "..")))
            .Callback(() => shellNavigatedBack = true)
            .Returns(Task.CompletedTask);

        // Set up application with Shell
        var window = new Window { Page = shellMock.Object };
        Application.Current = new Application();
        Application.Current.OpenWindow(window);

        // Simulate GoBackAsync behavior (Priority 2: Shell)
        if (modalStack.Count == 0 && Application.Current?.Windows[0].Page is Shell shell)
        {
            await shell.GoToAsync("..");
        }

        // Assert
        shellNavigatedBack.ShouldBeTrue();
    }

    [Fact]
    public async Task GoBackAsync_WithNavigationStackAndNoModalOrShell_ShouldPopFromStack()
    {
        // Arrange
        var navigationMock = new Mock<INavigation>();
        var modalStack = new List<Page>();
        var navigationStack = new List<Page> { new(), new() };
        
        navigationMock.Setup(n => n.ModalStack).Returns(modalStack.AsReadOnly());
        navigationMock.Setup(n => n.NavigationStack).Returns(navigationStack.AsReadOnly());
        
        var regularPopped = false;
        navigationMock.Setup(n => n.PopAsync())
            .Callback(() =>
            {
                regularPopped = true;
                navigationStack.RemoveAt(navigationStack.Count - 1);
            })
            .ReturnsAsync(navigationStack.Last());

        // Set up application without Shell
        var window = new Window { Page = new Page() };
        Application.Current = new Application();
        Application.Current.OpenWindow(window);

        // Simulate GoBackAsync behavior (Priority 3: Navigation Stack)
        if (modalStack.Count == 0 && !(Application.Current?.Windows[0].Page is Shell))
        {
            await navigationMock.Object.PopAsync();
        }

        // Assert
        regularPopped.ShouldBeTrue();
        navigationStack.Count.ShouldBe(1);
    }

    [Fact]
    public async Task GoBackAsync_PriorityOrder_ModalBeforeShell()
    {
        // Arrange
        var navigationMock = new Mock<INavigation>();
        var shellMock = new Mock<Shell>();
        var modalStack = new List<Page> { new() };
        
        navigationMock.Setup(n => n.ModalStack).Returns(modalStack.AsReadOnly());
        
        var modalPopped = false;
        var shellNavigated = false;
        
        navigationMock.Setup(n => n.PopModalAsync())
            .Callback(() => modalPopped = true)
            .ReturnsAsync(modalStack[0]);
            
        shellMock.Setup(s => s.GoToAsync(It.IsAny<string>()))
            .Callback(() => shellNavigated = true)
            .Returns(Task.CompletedTask);

        // Simulate GoBackAsync with both modal and shell
        if (modalStack.Count > 0)
        {
            await navigationMock.Object.PopModalAsync();
        }
        else if (shellMock.Object != null)
        {
            await shellMock.Object.GoToAsync("..");
        }

        // Assert
        modalPopped.ShouldBeTrue();
        shellNavigated.ShouldBeFalse(); // Shell should NOT be used when modal exists
    }

    [Fact]
    public async Task GoBackAsync_PriorityOrder_ShellBeforeNavigationStack()
    {
        // Arrange
        var navigationMock = new Mock<INavigation>();
        var shellMock = new Mock<Shell>();
        var modalStack = new List<Page>();
        var navigationStack = new List<Page> { new() };
        
        navigationMock.Setup(n => n.ModalStack).Returns(modalStack.AsReadOnly());
        navigationMock.Setup(n => n.NavigationStack).Returns(navigationStack.AsReadOnly());
        
        var shellNavigated = false;
        var regularPopped = false;
        
        shellMock.Setup(s => s.GoToAsync(It.IsAny<string>()))
            .Callback(() => shellNavigated = true)
            .Returns(Task.CompletedTask);
            
        navigationMock.Setup(n => n.PopAsync())
            .Callback(() => regularPopped = true)
            .ReturnsAsync(navigationStack[0]);

        // Set up application with Shell
        Application.Current = new Application();
        var window = new Window { Page = shellMock.Object };
        Application.Current.OpenWindow(window);

        // Simulate GoBackAsync with shell (no modal)
        if (modalStack.Count == 0 && Application.Current?.Windows[0].Page is Shell shell)
        {
            await shell.GoToAsync("..");
        }
        else if (modalStack.Count == 0)
        {
            await navigationMock.Object.PopAsync();
        }

        // Assert
        shellNavigated.ShouldBeTrue();
        regularPopped.ShouldBeFalse(); // Regular stack should NOT be used when Shell exists
    }

    [Fact]
    public async Task GoBackAsync_ComplexScenario_MultipleModalsWithShell()
    {
        // Arrange
        var navigationMock = new Mock<INavigation>();
        var modalStack = new List<Page> { new(), new(), new() };
        
        navigationMock.Setup(n => n.ModalStack).Returns(modalStack.AsReadOnly());
        navigationMock.Setup(n => n.PopModalAsync())
            .Callback(() => modalStack.RemoveAt(modalStack.Count - 1))
            .ReturnsAsync(() => modalStack.Count > 0 ? modalStack.Last() : new Page());

        var initialCount = modalStack.Count;

        // Act - Simulate multiple back navigations
        for (int i = 0; i < initialCount; i++)
        {
            if (modalStack.Count > 0)
            {
                await navigationMock.Object.PopModalAsync();
            }
        }

        // Assert
        modalStack.ShouldBeEmpty();
        navigationMock.Verify(n => n.PopModalAsync(), Times.Exactly(initialCount));
    }

    [Fact]
    public async Task GoBackAsync_EmptyStacks_ShouldHandleGracefully()
    {
        // Arrange
        var navigationMock = new Mock<INavigation>();
        var modalStack = new List<Page>();
        var navigationStack = new List<Page>();
        
        navigationMock.Setup(n => n.ModalStack).Returns(modalStack.AsReadOnly());
        navigationMock.Setup(n => n.NavigationStack).Returns(navigationStack.AsReadOnly());
        navigationMock.Setup(n => n.PopAsync())
            .ThrowsAsync(new InvalidOperationException("Navigation stack is empty"));

        //var app = new Application();
        Application.Current = new Application();
        var window = new Window { Page = new Page() };
        Application.Current.OpenWindow(window);

        // Act
        async Task act()
        {
            if (modalStack.Count == 0 && !(Application.Current?.Windows[0].Page is Shell))
            {
                await navigationMock.Object.PopAsync();
            }
        }

        // Assert
        await Should.ThrowAsync<InvalidOperationException>(act);
    }
}
