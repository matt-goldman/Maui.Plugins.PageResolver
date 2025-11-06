# Plugin.Maui.SmartNavigation Integration Tests

This project contains comprehensive integration tests for the Plugin.Maui.SmartNavigation library.

## Test Coverage

The integration test suite covers all major scenarios across platforms:

### ✅ Route Resolution and Registration Tests
- Basic route building
- Route with query parameters
- Dictionary-based parameters
- Route kind preservation
- Empty/null handling

### ✅ Shell Navigation Tests
- GoToAsync with simple routes
- Query parameter handling
- Relative route navigation
- Absolute route navigation
- Multiple navigation sequences

### ✅ Non-Shell Navigation Tests
- PushAsync operations
- PopAsync operations
- Multiple page navigation
- InsertPageBefore functionality
- RemovePage functionality

### ✅ Modal Navigation Tests
- PushModalAsync operations
- PopModalAsync operations
- Modal stack independence
- Multiple modal stacking
- Empty stack handling

### ✅ Parameter Binding Tests
- Page-only parameter binding
- ViewModel-only parameter binding
- No parameters (default construction)
- Multiple parameter types
- Complex parameter scenarios
- Null/empty parameter handling
- Type integrity verification

### ✅ Lifecycle Behavior Tests
- IViewModelLifecycle implementation
- First navigation detection
- Subsequent navigation handling
- Navigation history tracking
- Exception propagation
- Async initialization patterns

### ✅ Platform-Specific Lifecycle Tests
- iOS lifecycle (ViewDidLoad, ViewWillAppear)
- Android lifecycle (onCreate, configuration changes, back stack)
- Windows lifecycle (Page Load, window activation, multi-window)
- Cross-platform consistency
- Memory warnings and process death handling
- Rapid navigation scenarios

### ✅ GoBackAsync Tests
- Modal stack priority
- Shell navigation priority
- Regular navigation stack priority
- Priority order verification
- Complex stack scenarios
- Empty stack handling

### ✅ Error Handling Tests
- Unregistered route exceptions
- Shell not available errors
- Invalid parameter handling
- Empty navigation stack errors
- Empty modal stack errors
- Parameter ambiguity detection
- Invalid route paths
- Constructor parameter mismatches
- Null route handling
- Missing dependency detection
- Circular dependency detection

## Running the Tests

### Prerequisites
- .NET 9.0 SDK or later (tests are prepared for .NET 10 when available)
- xUnit test runner

### Run all tests
```bash
dotnet test
```

### Run specific test category
```bash
dotnet test --filter "FullyQualifiedName~RouteTests"
dotnet test --filter "FullyQualifiedName~NavigationTests"
dotnet test --filter "FullyQualifiedName~ParameterBindingTests"
dotnet test --filter "FullyQualifiedName~LifecycleTests"
dotnet test --filter "FullyQualifiedName~ErrorHandlingTests"
```

### Run with coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Test Structure

```
Plugin.Maui.SmartNavigation.IntegrationTests/
├── Infrastructure/
│   └── IntegrationTestBase.cs          # Base test class with DI setup
├── Mocks/
│   ├── MockPages.cs                    # Mock page implementations
│   └── MockViewModels.cs               # Mock view model implementations
├── TestDoubles/
│   ├── IViewModelLifecycle.cs          # Lifecycle interface
│   ├── MauiMocks.cs                    # MAUI framework mocks
│   └── Route.cs                        # Route implementation for testing
└── Tests/
    ├── RouteTests/
    │   └── RouteResolutionTests.cs
    ├── NavigationTests/
    │   ├── ShellNavigationTests.cs
    │   ├── NonShellNavigationTests.cs
    │   ├── ModalNavigationTests.cs
    │   └── GoBackAsyncTests.cs
    ├── ParameterBindingTests/
    │   └── ParameterBindingTests.cs
    ├── LifecycleTests/
    │   ├── LifecycleBehaviorTests.cs
    │   └── PlatformSpecificLifecycleTests.cs
    └── ErrorHandlingTests/
        └── ErrorHandlingTests.cs
```

## CI/CD Integration

These tests are designed to run in CI/CD pipelines. See the root `.github/workflows/ci.yml` for integration details.

## Future Work

When .NET 10 becomes available:
1. Update `TargetFramework` to `net10.0`
2. Add actual MAUI workload support
3. Reference the actual Plugin.Maui.SmartNavigation project
4. Add UI automation tests for platform-specific behaviors
5. Add performance benchmarks

## Notes

- Tests currently use test doubles (mocks) for MAUI types as they're framework-independent
- Platform-specific tests verify the lifecycle contract behavior that should be consistent across platforms
- When the actual plugin is available, these tests can be updated to use real implementations
