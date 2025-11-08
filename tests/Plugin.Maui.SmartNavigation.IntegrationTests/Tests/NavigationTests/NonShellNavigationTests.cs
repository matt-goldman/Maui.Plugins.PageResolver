using Moq;
using Plugin.Maui.SmartNavigation.IntegrationTests.Infrastructure;
using Shouldly;

namespace Plugin.Maui.SmartNavigation.IntegrationTests.Tests.NavigationTests;

/// <summary>
/// Tests for non-Shell navigation (PushAsync, PopAsync)
/// </summary>
public class NonShellNavigationTests : IntegrationTestBase
{
    [Fact]
    public async Task PushAsync_ShouldAddPageToNavigationStack()
    {
        // Arrange
        var navigationMock = new Mock<INavigation>();
        var navigationStack = new List<Page>();
        navigationMock.Setup(n => n.NavigationStack).Returns(navigationStack.AsReadOnly());
        navigationMock.Setup(n => n.PushAsync(It.IsAny<Page>()))
            .Callback<Page>(p => navigationStack.Add(p))
            .Returns(Task.CompletedTask);

        var page = new Page();

        // Act
        await navigationMock.Object.PushAsync(page);

        // Assert
        navigationStack.ShouldContain(page);
        navigationStack.Count.ShouldBe(1);
    }

    [Fact]
    public async Task PopAsync_ShouldRemovePageFromNavigationStack()
    {
        // Arrange
        var navigationMock = new Mock<INavigation>();
        var navigationStack = new List<Page> { new(), new() };
        navigationMock.Setup(n => n.NavigationStack).Returns(navigationStack.AsReadOnly());
        navigationMock.Setup(n => n.PopAsync())
            .Callback(() => navigationStack.RemoveAt(navigationStack.Count - 1))
            .ReturnsAsync(navigationStack[^1]);

        var initialCount = navigationStack.Count;

        // Act
        await navigationMock.Object.PopAsync();

        // Assert
        navigationStack.Count.ShouldBe(initialCount - 1);
    }

    [Fact]
    public async Task PushAsync_MultiplePages_ShouldMaintainOrder()
    {
        // Arrange
        var navigationMock = new Mock<INavigation>();
        var navigationStack = new List<Page>();
        navigationMock.Setup(n => n.NavigationStack).Returns(navigationStack.AsReadOnly());
        navigationMock.Setup(n => n.PushAsync(It.IsAny<Page>()))
            .Callback<Page>(p => navigationStack.Add(p))
            .Returns(Task.CompletedTask);

        var page1 = new Page { Title = "Page1" };
        var page2 = new Page { Title = "Page2" };
        var page3 = new Page { Title = "Page3" };

        // Act
        await navigationMock.Object.PushAsync(page1);
        await navigationMock.Object.PushAsync(page2);
        await navigationMock.Object.PushAsync(page3);

        // Assert
        navigationStack.Count.ShouldBe(3);
        navigationStack[0].Title.ShouldBe("Page1");
        navigationStack[1].Title.ShouldBe("Page2");
        navigationStack[2].Title.ShouldBe("Page3");
    }

    [Fact]
    public void InsertPageBefore_ShouldInsertAtCorrectPosition()
    {
        // Arrange
        var navigationMock = new Mock<INavigation>();
        var navigationStack = new List<Page>();
        var page1 = new Page { Title = "Page1" };
        var page2 = new Page { Title = "Page2" };
        var pageToInsert = new Page { Title = "InsertedPage" };
        
        navigationStack.Add(page1);
        navigationStack.Add(page2);

        navigationMock.Setup(n => n.InsertPageBefore(It.IsAny<Page>(), It.IsAny<Page>()))
            .Callback<Page, Page>((newPage, beforePage) =>
            {
                var index = navigationStack.IndexOf(beforePage);
                if (index >= 0)
                {
                    navigationStack.Insert(index, newPage);
                }
            });

        // Act
        navigationMock.Object.InsertPageBefore(pageToInsert, page2);

        // Assert
        navigationStack.Count.ShouldBe(3);
        navigationStack[0].Title.ShouldBe("Page1");
        navigationStack[1].Title.ShouldBe("InsertedPage");
        navigationStack[2].Title.ShouldBe("Page2");
    }

    [Fact]
    public void RemovePage_ShouldRemoveSpecificPage()
    {
        // Arrange
        var navigationMock = new Mock<INavigation>();
        var navigationStack = new List<Page>();
        var page1 = new Page { Title = "Page1" };
        var page2 = new Page { Title = "Page2" };
        var page3 = new Page { Title = "Page3" };
        
        navigationStack.AddRange([page1, page2, page3]);

        navigationMock.Setup(n => n.RemovePage(It.IsAny<Page>()))
            .Callback<Page>(p => navigationStack.Remove(p));

        // Act
        navigationMock.Object.RemovePage(page2);

        // Assert
        navigationStack.Count.ShouldBe(2);
        navigationStack.ShouldContain(page1);
        navigationStack.ShouldNotContain(page2);
        navigationStack.ShouldContain(page3);
    }
}
