# Unit Testing Guide for CoolLibrary

## Overview
This project demonstrates **best practices for unit testing in .NET 9** using MSTest, Moq, and FluentAssertions.

## ✅ What We've Set Up

### 1. **Project References**
Your test project references:
- ✅ **CoolLibrary.Application** - The services you're testing
- ✅ **CoolLibrary.Domain** - Entities and contracts needed for testing

**Important**: You do NOT need to reference ALL projects. Only reference what you need to test!

### 2. **Testing Packages**
- **MSTest** (v3.6.4) - The testing framework
- **Moq** (v4.20.72) - For creating mock objects
- **FluentAssertions** (v8.8.0) - For more readable assertions

## 🧪 Test Structure Explained

### The AAA Pattern
Every test follows **Arrange-Act-Assert**:

```csharp
[TestMethod]
public async Task ExecuteAsync_WithValidData_ShouldCreateBookSuccessfully()
{
    // ARRANGE - Set up test data and mock behavior
    var createBookDto = new CreateBookRequestDTO { /* ... */ };
    _mockBooksRepository.Setup(/* ... */);
    
    // ACT - Execute the method you're testing
    var result = await _createBookService.ExecuteAsync(createBookDto);
    
    // ASSERT - Verify the results
    result.Should().NotBeNull();
    result.Title.Should().Be("Clean Code");
}
```

### Key Concepts

#### 1. **TestInitialize**
Runs before EACH test to ensure clean state:
```csharp
[TestInitialize]
public void Setup()
{
    // Create fresh mocks for each test
    _mockBooksRepository = new Mock<IBooks>();
    // ...
}
```

#### 2. **Mocking with Moq**
Simulate dependencies without real implementations:
```csharp
// Configure what should be returned when method is called
_mockAuthorsRepository
    .Setup(repo => repo.GetByIdAsync(authorId))
    .ReturnsAsync(new Author { AuthorId = authorId });
```

#### 3. **Verification**
Confirm methods were called:
```csharp
// Verify method was called exactly once
_mockBooksRepository.Verify(
    repo => repo.InsertAsync(It.IsAny<Book>()), 
    Times.Once
);
```

#### 4. **FluentAssertions**
More readable than traditional asserts:
```csharp
// ❌ Old way:
Assert.AreEqual("Clean Code", result.Title);

// ✅ New way:
result.Title.Should().Be("Clean Code");
```

#### 5. **Testing Exceptions**
Test error scenarios:
```csharp
var act = async () => await _createBookService.ExecuteAsync(invalidDto);

await act.Should().ThrowAsync<ApplicationException>()
    .WithMessage("*error message pattern*");
```

## 📁 Should You Reorganize Into src/test?

**YES, it's a best practice!** Your current structure:
```
TheCoolLibrary_RepositoryPattern/
├── CoolLibrary.API/
├── CoolLibrary.Application/
├── CoolLibrary.Domain/
├── CoolLibrary.Infrastructure/
└── CoolLibraryTests/
```

**Better structure:**
```
TheCoolLibrary_RepositoryPattern/
├── src/
│   ├── CoolLibrary.API/
│   ├── CoolLibrary.Application/
│   ├── CoolLibrary.Domain/
│   └── CoolLibrary.Infrastructure/
└── test/
    └── CoolLibraryTests/
```

**Benefits:**
- ✅ Clear separation of production code and tests
- ✅ Industry standard (used by Microsoft and most .NET projects)
- ✅ Easier to configure build pipelines
- ✅ Cleaner repository structure

## 🎯 Test Coverage Strategy

### What to Test in Each Layer

#### ✅ Application Layer (Services)
**YES - This is your primary focus!**
- Business logic validation
- Service orchestration
- DTO mapping
- Error handling

Example: `CreateBookServiceTests`

#### ✅ Domain Layer (Entities)
**YES - For complex business rules**
- Entity validation methods
- Calculated properties
- Business rule enforcement

Example:
```csharp
[TestMethod]
public void Book_IsValidCopyCount_ShouldBeFalse_WhenAvailableExceedsTotal()
{
    var book = new Book 
    { 
        AvailableCopies = 15, 
        TotalCopies = 10 
    };
    
    book.IsValidCopyCount.Should().BeFalse();
}
```

#### ⚠️ Infrastructure Layer
**SOMETIMES - Usually integration tests**
- Only test custom logic in repositories
- Don't test Entity Framework directly
- Consider integration tests for database operations

#### ❌ API Layer
**NO - Use Integration Tests Instead**
- Controller logic should be minimal
- Use `WebApplicationFactory` for integration tests
- Test actual HTTP requests/responses

## 🚀 Running Tests

### Visual Studio
1. Open **Test Explorer** (Test → Test Explorer)
2. Click "Run All Tests"
3. View results with code coverage

### Command Line
```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run specific test
dotnet test --filter "FullyQualifiedName~CreateBookServiceTests"

# Generate coverage report
dotnet test --collect:"XPlat Code Coverage"
```

## 📚 Current Test Examples

### Test 1: Valid Book Creation
✅ Tests the happy path - everything works correctly

### Test 2: Available Copies Exceed Total
✅ Tests validation - catches business rule violations

### Test 3: No Authors Provided
✅ Tests required field validation

### Test 4: Author Doesn't Exist
✅ Tests dependency validation (author must exist)

### Test 5: Negative Copies
✅ Tests data validation (copies can't be negative)

## 🎓 Best Practices

### ✅ DO:
- **Use descriptive test names**: `ExecuteAsync_WhenAuthorDoesNotExist_ShouldThrowArgumentException`
- **Test one thing per test**: Each test validates a single behavior
- **Use mocks for dependencies**: Don't use real databases or external services
- **Keep tests independent**: Each test should work in isolation
- **Use FluentAssertions**: More readable and maintainable
- **Follow AAA pattern**: Arrange, Act, Assert

### ❌ DON'T:
- **Don't test framework code**: Don't test Entity Framework, AutoMapper, etc.
- **Don't use real databases**: Use mocks or in-memory databases
- **Don't share state between tests**: Each test should be independent
- **Don't test private methods**: Test public behavior instead
- **Don't ignore failing tests**: Fix them or remove them

## 📖 Learning Resources

### Official Documentation
- [MSTest Documentation](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-mstest)
- [Moq Quickstart](https://github.com/devlooped/moq)
- [FluentAssertions Documentation](https://fluentassertions.com/)

### Testing Principles
- **FIRST Principles**: Fast, Independent, Repeatable, Self-validating, Timely
- **AAA Pattern**: Arrange, Act, Assert
- **Test Pyramid**: More unit tests, fewer integration tests, even fewer E2E tests

## 🔄 Next Steps

1. **Add more service tests**:
   - Test `GetAllBooksService`
   - Test `DeleteBookService`
   - Test `CreateAuthorService`

2. **Test edge cases**:
   - Null values
   - Empty strings
   - Boundary values

3. **Set up code coverage**:
   ```bash
   dotnet add package coverlet.collector
   dotnet test --collect:"XPlat Code Coverage"
   ```

4. **Add integration tests** (separate project):
   - Test actual database operations
   - Test API endpoints
   - Use `WebApplicationFactory`

5. **Consider reorganizing** into `src/` and `test/` folders

## 📝 Example: Adding Your Own Test

```csharp
/// <summary>
/// Test: Should handle books with multiple authors
/// </summary>
[TestMethod]
public async Task ExecuteAsync_WithMultipleAuthors_ShouldCreateBookSuccessfully()
{
    // ARRANGE
    var authorIds = new List<int> { 1, 2, 3 };
    var createBookDto = new CreateBookRequestDTO
    {
        Title = "Design Patterns",
        ISBN = "978-0201633612",
        AvailableCopies = 5,
        TotalCopies = 10,
        Authors = authorIds
    };

    // Set up mocks for each author
    foreach (var authorId in authorIds)
    {
        _mockAuthorsRepository
            .Setup(repo => repo.GetByIdAsync(authorId))
            .ReturnsAsync(new Author 
            { 
                AuthorId = authorId,
                FirstName = $"Author{authorId}",
                LastName = "LastName"
            });
    }

    var bookEntity = new Book { BookId = 1, Title = "Design Patterns" };
    _mockMapper.Setup(m => m.Map<Book>(createBookDto)).Returns(bookEntity);
    _mockBooksRepository.Setup(repo => repo.InsertAsync(It.IsAny<Book>())).ReturnsAsync(bookEntity);
    _mockMapper.Setup(m => m.Map<CreateBookResponseDTO>(bookEntity))
        .Returns(new CreateBookResponseDTO { BookId = 1, Title = "Design Patterns" });

    // ACT
    var result = await _createBookService.ExecuteAsync(createBookDto);

    // ASSERT
    result.Should().NotBeNull();
    result.Title.Should().Be("Design Patterns");
    
    // Verify each author was validated
    foreach (var authorId in authorIds)
    {
        _mockAuthorsRepository.Verify(repo => repo.GetByIdAsync(authorId), Times.Once);
    }
}
```

## 🤝 Contributing
When adding new tests:
1. Follow the naming convention: `MethodName_Scenario_ExpectedResult`
2. Add XML documentation to explain what you're testing
3. Include both happy path and error scenarios
4. Run all tests before committing

---

**Happy Testing! 🎉**

Remember: Good tests are your safety net. They let you refactor with confidence and catch bugs before they reach production!
