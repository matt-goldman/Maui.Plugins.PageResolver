# MAUI Application Testing Pattern - Solution Documentation

## Problem
Integration tests needed to work with MAUI Application instances to test navigation scenarios, but creating testable Application/Window instances was failing because:
1. Directly instantiating `Application` and calling `ActivateWindow()` doesn't populate the `Windows` collection
2. The `Windows` collection requires platform-specific infrastructure that's not available in headless unit tests
3. Previous attempts to mock or workaround this led to "testing the test" rather than testing actual code

## Solution: MauiApp Host Builder Pattern

The solution mirrors how platform entry points (AppDelegate on iOS, MainApplication on Android, etc.) initialize MAUI applications. These entry points call `CreateMauiApp()` which returns a properly configured `MauiApp` instance with:
- Fully initialized dependency injection container
- Service registrations
- MAUI handlers and infrastructure
- Properly configured Application instance

## Implementation

### 1. TestMauiProgram (Infrastructure/TestMauiProgram.cs)
```csharp
public static class TestMauiProgram
{
    public static MauiApp CreateMauiApp(Page? mainPage = null)
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<MockApplication>()
               .ConfigureFonts(/*...*/);
        // Configure services as needed
        return builder.Build();
    }
}
```

This provides test-specific variants:
- `CreateMauiApp()` - Generic with optional page
- `CreateMauiAppWithShell()` - Pre-configured with Shell
- `CreateMauiAppWithPage()` - Pre-configured with ContentPage

### 2. Updated MockApplication (Mocks/MockApplication.cs)
```csharp
public class MockApplication : Application
{
    // Parameterless constructor required for host builder
    public MockApplication() { }
    
    // Optional legacy constructor for backward compatibility
    public MockApplication(Page page) : this() { /*...*/ }
    
    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Returns window with configured page
    }
}
```

Key change: Added parameterless constructor to support `UseMauiApp<MockApplication>()` pattern.

### 3. Enhanced IntegrationTestBase (Infrastructure/IntegrationTestBase.cs)
```csharp
public abstract class IntegrationTestBase : IDisposable
{
    protected MauiApp? MauiApp { get; private set; }
    protected Application? App { get; private set; }
    
    protected void InitializeMauiApp(Page? mainPage = null)
    {
        MauiApp = TestMauiProgram.CreateMauiApp(mainPage);
        App = MauiApp.Services.GetRequiredService<IApplication>() as Application;
        Application.Current = App;
    }
    
    // Similar methods for Shell and Page variants
}
```

## Usage Pattern

### In Test Methods:
```csharp
[Fact]
public void MyNavigationTest()
{
    // Arrange - Initialize MAUI app
    InitializeMauiAppWithPage();  // or InitializeMauiAppWithShell()
    
    // Application.Current is now properly set
    Application.Current.ShouldNotBeNull();
    
    // Get services from DI container
    var navService = MauiApp.Services.GetRequiredService<INavigationService>();
    
    // Act & Assert - test actual plugin code
    // ...
}
```

## Benefits

1. **Proper Initialization**: Application is initialized through the same path as production code
2. **DI Container**: Full access to service provider for resolving dependencies
3. **No Platform Dependencies**: Works in headless test environment
4. **Matches Production**: Same pattern as platform entry points
5. **Testable**: Can inject test services and mocks into the builder
6. **Extensible**: Easy to add configuration for different test scenarios

## Current Limitations

- Windows collection may still be empty in headless environment (platform activation required)
- UI-specific handlers and features may not be available
- Platform-specific lifecycle events won't fire

## For UI-Level Testing

For tests that require actual platform UI handlers, window management, or lifecycle events, use:
- Xamarin.UITest / Appium for full UI automation
- Platform-specific test frameworks (XCTest, Espresso, etc.)

This pattern is ideal for:
- Navigation service logic
- Dependency injection
- Service layer testing
- Business logic that interacts with MAUI infrastructure

## Future Error Handling Tests

With this pattern, real error handling tests can now be written:

```csharp
[Fact]
public async Task NavigateToUnregisteredRoute_ShouldThrowInvalidOperationException()
{
    // Arrange
    InitializeMauiAppWithShell();
    var navigationService = MauiApp.Services.GetRequiredService<ISmartNavigationService>();
    
    // Act & Assert - testing ACTUAL plugin code
    var ex = await Should.ThrowAsync<InvalidOperationException>(() =>
        navigationService.GoToAsync("unregistered/route"));
    ex.Message.ShouldContain("not registered");
}
```

This tests real navigation service behavior, not mock setup code.
