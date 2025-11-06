using FluentAssertions;
using Plugin.Maui.SmartNavigation.Behaviours;
using Plugin.Maui.SmartNavigation.IntegrationTests.Infrastructure;
using Plugin.Maui.SmartNavigation.IntegrationTests.Mocks;

namespace Plugin.Maui.SmartNavigation.IntegrationTests.Tests.LifecycleTests;

/// <summary>
/// Tests for IViewModelLifecycle behavior
/// </summary>
public class LifecycleBehaviorTests : IntegrationTestBase
{
    [Fact]
    public async Task OnInitAsync_FirstNavigation_ShouldSetIsFirstNavigationTrue()
    {
        // Arrange
        var viewModel = new MockLifecycleViewModel();

        // Act
        await viewModel.OnInitAsync(isFirstNavigation: true);

        // Assert
        viewModel.OnInitAsyncCallCount.Should().Be(1);
        viewModel.LastIsFirstNavigation.Should().BeTrue();
    }

    [Fact]
    public async Task OnInitAsync_SubsequentNavigation_ShouldSetIsFirstNavigationFalse()
    {
        // Arrange
        var viewModel = new MockLifecycleViewModel();

        // Act
        await viewModel.OnInitAsync(isFirstNavigation: true);
        await viewModel.OnInitAsync(isFirstNavigation: false);

        // Assert
        viewModel.OnInitAsyncCallCount.Should().Be(2);
        viewModel.LastIsFirstNavigation.Should().BeFalse();
    }

    [Fact]
    public async Task OnInitAsync_MultipleNavigations_ShouldTrackHistory()
    {
        // Arrange
        var viewModel = new MockLifecycleViewModel();

        // Act
        await viewModel.OnInitAsync(isFirstNavigation: true);
        await viewModel.OnInitAsync(isFirstNavigation: false);
        await viewModel.OnInitAsync(isFirstNavigation: false);

        // Assert
        viewModel.NavigationHistory.Should().HaveCount(3);
        viewModel.NavigationHistory[0].Should().BeTrue();
        viewModel.NavigationHistory[1].Should().BeFalse();
        viewModel.NavigationHistory[2].Should().BeFalse();
    }

    [Fact]
    public async Task OnInitAsync_CalledMultipleTimes_ShouldIncrementCallCount()
    {
        // Arrange
        var viewModel = new MockLifecycleViewModel();

        // Act
        await viewModel.OnInitAsync(true);
        await viewModel.OnInitAsync(false);
        await viewModel.OnInitAsync(false);
        await viewModel.OnInitAsync(false);

        // Assert
        viewModel.OnInitAsyncCallCount.Should().Be(4);
    }

    [Fact]
    public async Task IViewModelLifecycle_InterfaceImplementation_ShouldBeAsynchronous()
    {
        // Arrange
        IViewModelLifecycle viewModel = new MockLifecycleViewModel();

        // Act
        var task = viewModel.OnInitAsync(true);
        await task;

        // Assert
        task.IsCompleted.Should().BeTrue();
        task.Should().BeAssignableTo<Task>();
    }

    [Fact]
    public async Task OnInitAsync_WithException_ShouldPropagateException()
    {
        // Arrange
        var viewModel = new ExceptionThrowingViewModel();

        // Act
        Func<Task> act = async () => await viewModel.OnInitAsync(true);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Test exception");
    }

    [Fact]
    public async Task OnInitAsync_WithDelay_ShouldCompleteAsynchronously()
    {
        // Arrange
        var viewModel = new DelayedViewModel();

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await viewModel.OnInitAsync(true);
        stopwatch.Stop();

        // Assert
        viewModel.InitializationCompleted.Should().BeTrue();
        stopwatch.ElapsedMilliseconds.Should().BeGreaterThanOrEqualTo(50);
    }

    [Fact]
    public async Task OnInitAsync_MultipleViewModels_ShouldBeIndependent()
    {
        // Arrange
        var viewModel1 = new MockLifecycleViewModel();
        var viewModel2 = new MockLifecycleViewModel();

        // Act
        await viewModel1.OnInitAsync(true);
        await viewModel2.OnInitAsync(true);
        await viewModel1.OnInitAsync(false);

        // Assert
        viewModel1.OnInitAsyncCallCount.Should().Be(2);
        viewModel2.OnInitAsyncCallCount.Should().Be(1);
    }

    [Fact]
    public async Task OnInitAsync_NavigationPattern_ShouldFollowExpectedSequence()
    {
        // Arrange
        var viewModel = new MockLifecycleViewModel();

        // Act - Simulate typical navigation pattern
        // First navigation
        await viewModel.OnInitAsync(isFirstNavigation: true);
        
        // Navigate away and back (re-initialization)
        await viewModel.OnInitAsync(isFirstNavigation: false);
        
        // Navigate away and back again
        await viewModel.OnInitAsync(isFirstNavigation: false);

        // Assert
        viewModel.NavigationHistory.Should().ContainInOrder(true, false, false);
        viewModel.OnInitAsyncCallCount.Should().Be(3);
    }

    // Helper classes for specific test scenarios
    private class ExceptionThrowingViewModel : IViewModelLifecycle
    {
        public Task OnInitAsync(bool isFirstNavigation)
        {
            throw new InvalidOperationException("Test exception");
        }
    }

    private class DelayedViewModel : IViewModelLifecycle
    {
        public bool InitializationCompleted { get; private set; }

        public async Task OnInitAsync(bool isFirstNavigation)
        {
            await Task.Delay(50); // Simulate async initialization work
            InitializationCompleted = true;
        }
    }
}
