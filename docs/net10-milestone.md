# .NET 10 Milestone Roadmap

This document tracks the remaining work to complete the .NET 10 specification for Plugin.Maui.SmartNavigation.

**Current Progress: ~35-40% complete**

## Phase 1: Core Navigation & Routing (Critical Path)

### Issue #1: Implement Route.Build() with Query String Serialization

**Priority:** High  
**Estimate:** 3 points

**Description:**
The `Route.Build()` method currently ignores the `query` parameter. It should serialize object properties to a query string format.

**Acceptance Criteria:**

- [ ] Serialize anonymous objects to query strings (e.g., `new { id = 5, name = "test" }` → `?id=5&name=test`)
- [ ] Handle primitive types, strings, and common value types
- [ ] URL-encode values properly
- [ ] Handle null values (skip or include as empty)
- [ ] Add unit tests for various query object shapes

**Technical Notes:**

- Consider using `System.Reflection` to enumerate properties
- Use `Uri.EscapeDataString()` for encoding
- Reference spec section 6.1

---

### Issue #2: Create IRouteRegistry Implementation

**Priority:** High  
**Estimate:** 5 points

**Description:**
The `IRouteRegistry` interface exists but has no concrete implementation. Need to create `RouteRegistry` class that manages route-to-page-type mappings.

**Acceptance Criteria:**

- [ ] Create `RouteRegistry` class implementing `IRouteRegistry`
- [ ] Implement `Register(Route, Type)` with route path validation
- [ ] Implement `Register(Route, Func<Page>)` for factory-based registration
- [ ] Implement `Resolve(Route)` with efficient lookup
- [ ] Thread-safe dictionary for registrations
- [ ] Register as singleton in DI container
- [ ] Add unit tests for registration and resolution
- [ ] Handle duplicate route registration (throw or overwrite?)

**Technical Notes:**

- Use `ConcurrentDictionary<string, object>` to store registrations (string key = route path)
- Store either `Type` or `Func<Page>` as value
- Reference spec sections 6.3 and 9

---

### Issue #3: Complete NavigationManager Implementation

**Priority:** High  
**Estimate:** 8 points

**Description:**
The `NavigationManager` has several incomplete implementations and missing features.

**Acceptance Criteria:**

- [ ] Implement non-Shell fallback for `GoToAsync()` using `IRouteRegistry`
- [ ] Implement `SmartBackAsync()` logic:
  - Check if modal stack has items → pop modal
  - Else if Shell is available → `GoToAsync("..")`
  - Else → `PopAsync()`
- [ ] Implement `wrapInNav` parameter for `PushModalAsync`
- [ ] Integrate with `IRouteRegistry` for route resolution
- [ ] Add error handling for unregistered routes (throw `InvalidOperationException`)
- [ ] Add unit tests with mocked navigation and Shell
- [ ] Test modal stack detection

**Technical Notes:**

- Use `Application.Current.MainPage.Navigation.ModalStack` to check for modals
- For `wrapInNav`, wrap page in `NavigationPage` when true
- Reference spec sections 6.2 and 9

---

### Issue #4: Implement UseSmartNavigation Extension Method

**Priority:** High  
**Estimate:** 5 points

**Description:**
Replace legacy `UsePageResolver` with new `UseSmartNavigation` extension method as the main entry point.

**Acceptance Criteria:**

- [ ] Create `UseSmartNavigation(this MauiAppBuilder, SmartNavOptions?)` extension
- [ ] Create `SmartNavOptions` record with `PreferShell` property
- [ ] Register `INavigationManager` implementation
- [ ] Register `IRouteRegistry` as singleton
- [ ] Configure based on options
- [ ] Keep `UsePageResolver` as obsolete with migration message
- [ ] Update README and wiki with new API
- [ ] Add integration tests

**Technical Notes:**

- Reference spec section 6.5
- Mark old methods with `[Obsolete("Use UseSmartNavigation instead", false)]`
- Eventually set error=true in future version

---

## Phase 2: Lifecycle & Behaviors

### Issue #5: Implement ViewModelInitOnLoadedBehavior

**Priority:** Medium  
**Estimate:** 3 points

**Description:**
Create behavior that initializes ViewModel once after the page's `Loaded` event fires.

**Acceptance Criteria:**

- [ ] Create `ViewModelInitOnLoadedBehavior : Behavior<Page>`
- [ ] Wire up to `Page.Loaded` event
- [ ] Check if `BindingContext` implements `IAsyncInitializable`
- [ ] Call `InitializeAsync()` once (track with flag)
- [ ] Handle async void properly (fire and forget or await?)
- [ ] Add unit tests with test pages and VMs
- [ ] Test on iOS, Android, Windows for timing issues

**Technical Notes:**

- Reference spec section 6.4
- Similar pattern to existing `NavigatedInitBehavior`

---

### Issue #6: Implement ViewModelLifecycleBehavior

**Priority:** Medium  
**Estimate:** 5 points

**Description:**
Create behavior that calls lifecycle methods on every page appearance/disappearance.

**Acceptance Criteria:**

- [ ] Create `ViewModelLifecycleBehavior : Behavior<Page>`
- [ ] Wire up to `Page.Appearing` event → call `OnInitAsync()`
- [ ] Wire up to `Page.Disappearing` event → call `OnDeinitAsync()`
- [ ] Check if `BindingContext` implements `IViewModelLifecycle`
- [ ] Call methods every time (not just once)
- [ ] Handle async void properly
- [ ] Add unit tests with appearing/disappearing cycles
- [ ] Test on iOS, Android, Windows

**Technical Notes:**

- Reference spec section 6.4
- Consider using weak event handlers to avoid memory leaks

---

### Issue #7: Create IAsyncInitializable Interface

**Priority:** Medium  
**Estimate:** 1 point

**Description:**
Add the `IAsyncInitializable` interface for one-time ViewModel initialization.

**Acceptance Criteria:**

- [ ] Create interface in `Plugin.Maui.SmartNavigation.Behaviours` namespace
- [ ] Single method: `Task InitializeAsync()`
- [ ] Add XML documentation
- [ ] Update `IViewModelLifecycle` to include `OnDeinitAsync()` method

**Technical Notes:**

- Reference spec section 6.4
- This is used by `ViewModelInitOnLoadedBehavior`

---

### Issue #8: Update IViewModelLifecycle Interface

**Priority:** Medium  
**Estimate:** 1 point

**Description:**
Add missing `OnDeinitAsync()` method to complete the lifecycle interface.

**Acceptance Criteria:**

- [ ] Add `Task OnDeinitAsync()` to interface
- [ ] Add XML documentation
- [ ] Update existing `NavigatedInitBehavior` if needed
- [ ] Update demo project ViewModels

**Technical Notes:**

- Reference spec section 6.4
- Breaking change for existing implementations

---

## Phase 3: Source Generator Updates

### Issue #9: Invert Source Generator to Opt-In Model

**Priority:** High  
**Estimate:** 8 points

**Description:**
Update source generator to use opt-in `[AutoDependencies]` attribute instead of opt-out model.

**Acceptance Criteria:**

- [ ] Create `[AutoDependencies]` attribute for assembly or class level
- [ ] Update generator to look for `[AutoDependencies]` instead of `[UseAutoDependencies]`
- [ ] Support both assembly-level and MauiProgram class-level placement
- [ ] Keep `[UseAutoDependencies]` as obsolete for backward compatibility
- [ ] Generate same output as before when attribute is found
- [ ] Skip generation when attribute is absent (no error)
- [ ] Update demo project to use new attribute
- [ ] Update documentation

**Technical Notes:**

- Reference spec section 7
- Check for both `[assembly: AutoDependencies]` and `[AutoDependencies]` on MauiProgram

---

### Issue #10: Rename IgnoreAttribute to IgnoreAutoDependencyAttribute

**Priority:** Low  
**Estimate:** 2 points

**Description:**
Rename the attribute to be more explicit about its purpose.

**Acceptance Criteria:**

- [ ] Rename `IgnoreAttribute` to `IgnoreAutoDependencyAttribute`
- [ ] Update source generator to recognize new name
- [ ] Keep old name as type alias for backward compatibility
- [ ] Mark old name as obsolete
- [ ] Update demo project
- [ ] Update documentation

**Technical Notes:**

- Reference spec section 7.4

---

### Issue #11: Implement [SmartRoute] Attribute and Routes.g.cs Generation

**Priority:** Medium  
**Estimate:** 8 points

**Description:**
Add support for `[SmartRoute]` attribute on pages to generate centralized route definitions.

**Acceptance Criteria:**

- [ ] Create `[SmartRoute(string path)]` attribute
- [ ] Add optional `Group` property for organizing routes
- [ ] Add optional `Kind` property for `RouteKind`
- [ ] Update generator to discover pages with `[SmartRoute]`
- [ ] Generate `Routes.g.cs` with static route fields organized by Group
- [ ] Generate route registration calls in `UseAutodependencies()`
- [ ] Support both attributed and non-attributed pages
- [ ] Add unit tests for generator output
- [ ] Update demo project with examples

**Example Output:**

```csharp
public static class Routes
{
    public static class Products
    {
        public static readonly Route List = new("products/list");
        public static readonly Route Details = new("products/details");
    }
}
```

**Technical Notes:**

- Reference spec sections 7.2 and 7.3
- Consider making `Routes.g.cs` generation opt-in via generator option

---

## Phase 4: Parameter Binding & Error Handling

### Issue #12: Implement Spec-Compliant Parameter Binding

**Priority:** High  
**Estimate:** 8 points

**Description:**
Update parameter binding logic to follow the spec's rules for applying parameters to Pages and ViewModels.

**Acceptance Criteria:**

- [ ] Accept anonymous object or record as navigation parameters
- [ ] Try to apply to Page properties first
- [ ] Try to apply to ViewModel properties second
- [ ] If both Page AND ViewModel have matching writable properties, throw with clear message
- [ ] Handle cases where neither has matching properties (no-op)
- [ ] Support dictionary fallback for advanced scenarios
- [ ] Add comprehensive unit tests for all scenarios
- [ ] Test with various parameter types (primitives, objects, collections)

**Technical Notes:**

- Reference spec section 8
- Use reflection to discover writable properties
- Consider caching property info for performance

---

### Issue #13: Implement Error Handling Per Spec

**Priority:** Medium  
**Estimate:** 5 points

**Description:**
Add proper error handling throughout the navigation system per spec requirements.

**Acceptance Criteria:**

- [ ] Unregistered route → `InvalidOperationException` with clear message including route path
- [ ] Shell unavailable for `GoToAsync` → automatically fallback to `IRouteRegistry` + `PushAsync`
- [ ] Null factory result → `InvalidOperationException` with type information
- [ ] Mismatched page type from factory → `InvalidOperationException` with both types
- [ ] Ambiguous parameter binding → `InvalidOperationException` listing conflicting properties
- [ ] Add unit tests for all error scenarios
- [ ] Ensure error messages are helpful and actionable

**Technical Notes:**

- Reference spec section 9
- Include route information in exception messages
- Consider custom exception types for better catch scenarios

---

## Phase 5: Analyzers (Optional Package)

### Issue #14: Create Analyzer Package Infrastructure

**Priority:** Low  
**Estimate:** 5 points

**Description:**
Set up separate analyzer package project and infrastructure.

**Acceptance Criteria:**

- [ ] Create `Plugin.Maui.SmartNavigation.Analyzers` project
- [ ] Configure as Roslyn analyzer project
- [ ] Set up test project with analyzer test infrastructure
- [ ] Configure NuGet packaging
- [ ] Set up CI/CD for analyzer package
- [ ] Add README for analyzer package

**Technical Notes:**

- Reference spec section 12
- Separate package allows opt-in analyzer usage

---

### Issue #15: Implement SN0001 Analyzer - Disallow Raw String Routes

**Priority:** Low  
**Estimate:** 5 points

**Description:**
Create analyzer to detect raw string routes when a `Route` constant exists.

**Acceptance Criteria:**

- [ ] Detect calls to `GoToAsync(string)` with string literals
- [ ] Check if a matching `Route` exists in the workspace
- [ ] Report diagnostic when Route constant should be used
- [ ] Provide code fix to replace string with Route field
- [ ] Handle false positives gracefully
- [ ] Add unit tests for various scenarios

**Technical Notes:**

- Reference spec section 12
- Analyzer ID: SN0001
- Severity: Warning

---

### Issue #16: Implement SN0002 Analyzer - Route Registration Location

**Priority:** Low  
**Estimate:** 3 points

**Description:**
Create analyzer to detect routes registered outside the central registry.

**Acceptance Criteria:**

- [ ] Detect `Routing.RegisterRoute()` calls outside designated locations
- [ ] Detect route registrations in random places
- [ ] Report diagnostic suggesting central registration
- [ ] Add configuration for allowed registration locations
- [ ] Add unit tests

**Technical Notes:**

- Reference spec section 12
- Analyzer ID: SN0002
- Severity: Info/Warning

---

### Issue #17: Implement SN0003 Analyzer - Behavior Interface Mismatch

**Priority:** Low  
**Estimate:** 3 points

**Description:**
Create analyzer to detect behaviors used without implementing the required ViewModel interface.

**Acceptance Criteria:**

- [ ] Detect `ViewModelInitOnLoadedBehavior` without `IAsyncInitializable`
- [ ] Detect `ViewModelLifecycleBehavior` without `IViewModelLifecycle`
- [ ] Report diagnostic with interface name to implement
- [ ] Provide code fix to add interface to ViewModel
- [ ] Add unit tests

**Technical Notes:**

- Reference spec section 12
- Analyzer ID: SN0003
- Severity: Warning

---

## Phase 6: Testing & Documentation

### Issue #18: Comprehensive Integration Tests

**Priority:** High  
**Estimate:** 8 points

**Description:**
Create integration tests covering all major scenarios across platforms.

**Acceptance Criteria:**

- [ ] Route resolution and registration tests
- [ ] Shell and non-Shell navigation tests
- [ ] Modal navigation tests
- [ ] Parameter binding tests (all scenarios from spec)
- [ ] Lifecycle behavior tests on iOS, Android, Windows
- [ ] SmartBackAsync tests with various stack configurations
- [ ] Error handling tests
- [ ] Test on physical devices where possible
- [ ] CI/CD integration

**Technical Notes:**

- Reference spec section 13
- Consider using UITest or Appium for cross-platform testing

---

### Issue #19: Update Documentation and Samples

**Priority:** High  
**Estimate:** 5 points

**Description:**
Update all documentation to reflect .NET 10 changes and new APIs.

**Acceptance Criteria:**

- [ ] Update README with new API examples
- [ ] Update wiki with migration guide
- [ ] Add Shell vs non-Shell navigation guide
- [ ] Add behaviors overview and usage guide
- [ ] Add recipe examples (common scenarios)
- [ ] Update demo project to showcase all features
- [ ] Create migration checklist from PageResolver 2.x
- [ ] Update NuGet package description
- [ ] Add/update icon

**Technical Notes:**

- Reference spec section 15
- Include code samples for all major features

---

### Issue #20: Write Blog Post and Release Announcement

**Priority:** Medium  
**Estimate:** 3 points

**Description:**
Create announcement content for the .NET 10 release.

**Acceptance Criteria:**

- [ ] Blog post covering:
  - Rename from PageResolver to SmartNavigation
  - New navigation service
  - Route system
  - Lifecycle behaviors
  - Source generator improvements
  - Migration guide
- [ ] Release notes on GitHub
- [ ] Update social media / dev.to / Medium
- [ ] Update project website if applicable

**Technical Notes:**

- Reference spec section 15

---

## Phase 7: Polish & Compatibility

### Issue #21: Backward Compatibility & Deprecation Warnings

**Priority:** Medium  
**Estimate:** 3 points

**Description:**
Ensure smooth migration path from old PageResolver to SmartNavigation.

**Acceptance Criteria:**

- [ ] Mark old `UsePageResolver` methods as obsolete with helpful messages
- [ ] Mark old attributes as obsolete
- [ ] Provide type forwards where possible
- [ ] Create migration analyzer/code fix (optional)
- [ ] Test that old code works with warnings
- [ ] Document breaking changes clearly

**Technical Notes:**

- Reference spec section 10 (now marked as "not maintained")
- Balance between clean API and migration pain

---

### Issue #22: Performance Optimization

**Priority:** Low  
**Estimate:** 5 points

**Description:**
Optimize performance-critical paths.

**Acceptance Criteria:**

- [ ] Cache reflection results for parameter binding
- [ ] Optimize route lookup in registry
- [ ] Minimize allocations in hot paths
- [ ] Profile startup time with and without source generator
- [ ] Benchmark navigation operations
- [ ] Document performance characteristics

**Technical Notes:**

- Use BenchmarkDotNet for measurements
- Consider compiled expressions instead of reflection

---

### Issue #23: API Review and Finalization

**Priority:** High  
**Estimate:** 3 points

**Description:**
Final review of public API surface before stable release.

**Acceptance Criteria:**

- [ ] Review all public interfaces, classes, and methods
- [ ] Ensure naming consistency
- [ ] Verify XML documentation on all public members
- [ ] Check for missing nullability annotations
- [ ] Validate against .NET design guidelines
- [ ] Get community feedback on API
- [ ] Lock API for v2.0 release

**Technical Notes:**

- Use PublicAPI analyzer to track changes
- Consider API review with community/maintainers

---

## Summary

**Total Issues:** 23  
**Estimated Points:** 110

### By Priority

- **High Priority:** 10 issues (56 points) - Critical path items
- **Medium Priority:** 8 issues (38 points) - Important but not blocking
- **Low Priority:** 5 issues (16 points) - Nice to have

### By Phase

1. **Core Navigation & Routing:** 4 issues (21 points)
2. **Lifecycle & Behaviors:** 4 issues (10 points)
3. **Source Generator Updates:** 3 issues (18 points)
4. **Parameter Binding & Error Handling:** 2 issues (13 points)
5. **Analyzers:** 4 issues (16 points)
6. **Testing & Documentation:** 3 issues (16 points)
7. **Polish & Compatibility:** 3 issues (11 points)

### Recommended Sprint Plan

**Sprint 1 (Weeks 1-2):** Issues #1, #2, #3, #4 - Core Navigation  
**Sprint 2 (Week 3):** Issues #5, #6, #7, #8, #12 - Behaviors & Parameters  
**Sprint 3 (Week 4):** Issues #9, #10, #11, #13 - Generator & Error Handling  
**Sprint 4 (Week 5):** Issues #18, #19, #23 - Testing & API Lock  
**Sprint 5 (Week 6):** Issues #20, #21, #22 - Release Prep  
**Future:** Issues #14-17 - Analyzers (post v2.0)

---

## Open Questions from Spec

Per section 17 of the spec, these design decisions need to be made:

1. **Route.Kind Behavior:** Should `Route.Kind` automatically influence navigation style, or remain advisory only?
   - **Recommendation:** Start advisory only, consider auto-switching in v2.1

2. **Routes Generation:** Emit `Routes.g.cs` by default when `[SmartRoute]` is present, or behind a flag?
   - **Recommendation:** Auto-generate by default, add opt-out flag if needed

3. **Mopups Integration:** Provide Mopups helper in core or via optional extension package?
   - **Recommendation:** Optional extension package to avoid core dependency

---

*Last Updated: November 1, 2025*
