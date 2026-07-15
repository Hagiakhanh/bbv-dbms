# C# .NET TDD & Code Conventions

## 1. Standard Testing Stack

- **Test Framework:** Use **xUnit** (avoid MSTest or NUnit unless explicitly requested). Define test cases using:
  - `[Fact]` for standard unit tests.
  - `[Theory]` with `[InlineData]` for data-driven tests.
- **Assertions:** Prefer **FluentAssertions** to make assertions more readable and expressive.
  - Examples:
    - `result.Should().NotBeNull();`
    - `action.Should().Throw<InvalidOperationException>();`
  - Avoid using traditional assertions such as:
    - `Assert.NotNull(result);`
- **Mocking:** Use **Moq** or **NSubstitute** whenever mocking dependencies or interfaces is required.

---

## 2. Skeleton Structure During the Red Phase (C# Specific)

- To ensure the test project compiles successfully, generate the required classes and interfaces in the appropriate folders and namespaces.
- Every method inside a Skeleton **must only throw** `NotImplementedException`.
- Do **not** implement any business logic during the Red Phase.

**Example:**

```csharp
public class DiskManager : IDiskManager
{
    public Page ReadPage(int pageId)
    {
        throw new NotImplementedException("Implement in Green Phase");
    }
}
```

---

## 3. Project Organization & Naming Conventions

### Test Project

- All unit tests should reside in a dedicated test project.
- The project name should end with the `.Tests` suffix.
- Example:

```text
BbvDbms.Foundation.Tests
```

### Test Class

- Every test class should end with the `Tests` suffix.

Example:

```text
DiskManagerTests
```

### Test Method

- Follow the naming convention:

```text
MethodName_StateUnderTest_ExpectedBehavior
```

Example:

```text
ReadPage_WithInvalidPageId_ShouldThrowArgumentException()
```

### Interfaces

- Every core Foundation component should expose an interface.
- Interface names must begin with the `I` prefix.

Examples:

```text
IDiskManager
IBufferPool
IPageManager
IRecordManager
```

This enables:
- Dependency Injection
- Mocking
- Loose coupling
- Easier testing

---

## 4. Clearly Separate Arrange, Act, and Assert

Use comments to explicitly separate the three sections inside every test method. This improves readability and reinforces the AAA testing pattern.

```csharp
[Fact]
public void AllocatePage_WhenSpaceAvailable_ShouldReturnNewPageId()
{
    // Arrange
    var diskManager = new DiskManager();

    // Act
    var pageId = diskManager.AllocatePage();

    // Assert
    pageId.Should().BeGreaterThan(0);
}
```