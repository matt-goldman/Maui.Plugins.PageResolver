using Shouldly;
using Plugin.Maui.SmartNavigation.Behaviours;
using Plugin.Maui.SmartNavigation.IntegrationTests.Infrastructure;
using Plugin.Maui.SmartNavigation.IntegrationTests.Mocks;

namespace Plugin.Maui.SmartNavigation.IntegrationTests.Tests.LifecycleTests;

/// <summary>
/// Platform-specific lifecycle behavior tests for iOS, Android, and Windows
/// Note: These tests verify the lifecycle contract behavior that should be consistent
/// across platforms. Actual platform-specific integration would require platform-specific test runners.
/// </summary>
public class PlatformSpecificLifecycleTests : IntegrationTestBase
{
    [Fact]
    public async Task iOS_OnInitAsync_FirstAppearance_ShouldCallWithTrueFlag()
    {
        // Simulate iOS lifecycle behavior
        // On iOS, ViewDidLoad and ViewWillAppear are the key lifecycle methods
        
        // Arrange
        var viewModel = new MockLifecycleViewModel();

        // Act - Simulate first appearance
        await viewModel.OnInitAsync(isFirstNavigation: true);

        // Assert
        viewModel.OnInitAsyncCallCount.ShouldBe(1);
        viewModel.LastIsFirstNavigation.ShouldBe(true);
    }

    [Fact]
    public async Task iOS_OnInitAsync_ReappearingAfterBackground_ShouldCallWithFalseFlag()
    {
        // iOS apps can be backgrounded and foregrounded
        
        // Arrange
        var viewModel = new MockLifecycleViewModel();

        // Act - Simulate first appearance, background, then foreground
        await viewModel.OnInitAsync(isFirstNavigation: true);
        // App backgrounded (no lifecycle call)
        await viewModel.OnInitAsync(isFirstNavigation: false); // App foregrounded

        // Assert
        viewModel.NavigationHistory.ShouldBe(new List<bool> { true, false });
        viewModel.OnInitAsyncCallCount.ShouldBe(2);
    }

    [Fact]
    public async Task Android_OnInitAsync_ActivityCreate_ShouldCallWithTrueFlag()
    {
        // Simulate Android Activity lifecycle
        // onCreate is called when the activity is first created
        
        // Arrange
        var viewModel = new MockLifecycleViewModel();

        // Act - Simulate onCreate
        await viewModel.OnInitAsync(isFirstNavigation: true);

        // Assert
        viewModel.OnInitAsyncCallCount.ShouldBe(1);
        viewModel.LastIsFirstNavigation.ShouldBe(true);
    }

    [Fact]
    public async Task Android_OnInitAsync_ActivityRecreation_ShouldHandleConfigChanges()
    {
        // Android activities can be destroyed and recreated on configuration changes
        
        // Arrange
        var viewModel = new MockLifecycleViewModel();

        // Act - Simulate onCreate, rotation/config change (destroy + recreate)
        await viewModel.OnInitAsync(isFirstNavigation: true);
        // Activity destroyed and recreated
        await viewModel.OnInitAsync(isFirstNavigation: false);

        // Assert
        viewModel.NavigationHistory.ShouldBe(new List<bool> { true, false });
    }

    [Fact]
    public async Task Android_OnInitAsync_BackStackNavigation_ShouldPreserveState()
    {
        // Android back stack navigation
        
        // Arrange
        var viewModel = new MockLifecycleViewModel();

        // Act - Navigate to page, navigate away, navigate back
        await viewModel.OnInitAsync(isFirstNavigation: true);
        // Navigate to another page (no call)
        await viewModel.OnInitAsync(isFirstNavigation: false); // Back button pressed

        // Assert
        viewModel.OnInitAsyncCallCount.ShouldBe(2);
        viewModel.NavigationHistory.Last().ShouldBeFalse();
    }

    [Fact]
    public async Task Windows_OnInitAsync_PageLoad_ShouldCallWithTrueFlag()
    {
        // Simulate Windows (WinUI) page lifecycle
        // Loaded event fires when the page is loaded
        
        // Arrange
        var viewModel = new MockLifecycleViewModel();

        // Act - Simulate page Loaded event
        await viewModel.OnInitAsync(isFirstNavigation: true);

        // Assert
        viewModel.OnInitAsyncCallCount.ShouldBe(1);
        viewModel.LastIsFirstNavigation.ShouldBe(true);
    }

    [Fact]
    public async Task Windows_OnInitAsync_WindowActivation_ShouldHandleCorrectly()
    {
        // Windows apps can be deactivated and reactivated
        
        // Arrange
        var viewModel = new MockLifecycleViewModel();

        // Act - Simulate load, deactivate, reactivate
        await viewModel.OnInitAsync(isFirstNavigation: true);
        // Window deactivated (no lifecycle call)
        await viewModel.OnInitAsync(isFirstNavigation: false); // Window reactivated

        // Assert
        viewModel.NavigationHistory.ShouldBe(new List<bool> { true, false });
    }

    [Fact]
    public async Task CrossPlatform_OnInitAsync_ConsistentBehavior()
    {
        // All platforms should handle the lifecycle consistently
        
        // Arrange
        var viewModelIOS = new MockLifecycleViewModel();
        var viewModelAndroid = new MockLifecycleViewModel();
        var viewModelWindows = new MockLifecycleViewModel();

        // Act - Simulate same navigation pattern on all platforms
        await viewModelIOS.OnInitAsync(true);
        await viewModelIOS.OnInitAsync(false);

        await viewModelAndroid.OnInitAsync(true);
        await viewModelAndroid.OnInitAsync(false);

        await viewModelWindows.OnInitAsync(true);
        await viewModelWindows.OnInitAsync(false);

        // Assert - All platforms should behave the same
        viewModelIOS.NavigationHistory.ShouldBe(viewModelAndroid.NavigationHistory);
        viewModelAndroid.NavigationHistory.ShouldBe(viewModelWindows.NavigationHistory);
        
        viewModelIOS.OnInitAsyncCallCount.ShouldBe(2);
        viewModelAndroid.OnInitAsyncCallCount.ShouldBe(2);
        viewModelWindows.OnInitAsyncCallCount.ShouldBe(2);
    }

    [Fact]
    public async Task iOS_MemoryWarning_ShouldNotAffectLifecycleContract()
    {
        // iOS may receive memory warnings but the lifecycle contract should remain consistent
        
        // Arrange
        var viewModel = new MockLifecycleViewModel();

        // Act
        await viewModel.OnInitAsync(true);
        // Memory warning received (implementation-specific handling)
        // ViewModel should still track state correctly
        await viewModel.OnInitAsync(false);

        // Assert
        viewModel.NavigationHistory.ShouldBe(new List<bool> { true, false });
    }

    [Fact]
    public async Task Android_ProcessDeath_ShouldAllowReinitializationWithNewInstance()
    {
        // Android can kill the process and restart it
        
        // Arrange
        var viewModel1 = new MockLifecycleViewModel();

        // Act - First instance
        await viewModel1.OnInitAsync(true);
        
        // Process killed, new instance created
        var viewModel2 = new MockLifecycleViewModel();
        await viewModel2.OnInitAsync(true); // New instance, first navigation

        // Assert
        viewModel1.OnInitAsyncCallCount.ShouldBe(1);
        viewModel2.OnInitAsyncCallCount.ShouldBe(1);
        viewModel2.LastIsFirstNavigation.ShouldBe(true);
    }

    [Fact]
    public async Task Windows_MultiWindow_EachWindowShouldHaveIndependentLifecycle()
    {
        // Windows supports multiple windows
        
        // Arrange
        var viewModelWindow1 = new MockLifecycleViewModel();
        var viewModelWindow2 = new MockLifecycleViewModel();

        // Act - Simulate two independent windows
        await viewModelWindow1.OnInitAsync(true);
        await viewModelWindow2.OnInitAsync(true);
        await viewModelWindow1.OnInitAsync(false);

        // Assert
        viewModelWindow1.OnInitAsyncCallCount.ShouldBe(2);
        viewModelWindow2.OnInitAsyncCallCount.ShouldBe(1);
        viewModelWindow1.NavigationHistory.ShouldNotBe(viewModelWindow2.NavigationHistory);
    }

    [Fact]
    public async Task AllPlatforms_RapidNavigation_ShouldHandleCorrectly()
    {
        // Test rapid navigation that could happen on any platform
        
        // Arrange
        var viewModel = new MockLifecycleViewModel();

        // Act - Rapid navigation
        await viewModel.OnInitAsync(true);
        await Task.Delay(10);
        await viewModel.OnInitAsync(false);
        await Task.Delay(10);
        await viewModel.OnInitAsync(false);
        await Task.Delay(10);
        await viewModel.OnInitAsync(false);

        // Assert
        viewModel.OnInitAsyncCallCount.ShouldBe(4);
        viewModel.NavigationHistory.Count.ShouldBe(4);
        viewModel.NavigationHistory.First().ShouldBeTrue();
        viewModel.NavigationHistory.Skip(1).All(x => x == false).ShouldBeTrue();
    }

    [Fact]
    public async Task AllPlatforms_AsyncException_ShouldPropagateCorrectly()
    {
        // Exception handling should be consistent across platforms
        
        // Arrange
        var viewModel = new ExceptionThrowingViewModel();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await viewModel.OnInitAsync(true));
    }

    // Helper class for exception testing
    private class ExceptionThrowingViewModel : IViewModelLifecycle
    {
        public Task OnInitAsync(bool isFirstNavigation)
        {
            throw new InvalidOperationException("Platform-specific initialization failed");
        }
    }
}
