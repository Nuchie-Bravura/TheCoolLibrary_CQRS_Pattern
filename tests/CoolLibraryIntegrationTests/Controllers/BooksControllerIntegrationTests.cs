using System.Net;
using System.Net.Http.Json;
using CoolLibrary.Application.DTO.Book;
using CoolLibrary.Domain.Entities;
using CoolLibrary.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CoolLibraryIntegrationTests.Controllers;

[Collection("Sequential")]
public class BooksControllerIntegrationTests : IClassFixture<CoolLibraryWebApplicationFactory>
{
    private readonly HttpClient _httpClient;
    private readonly CoolLibraryWebApplicationFactory _webApplicationFactory;

    public BooksControllerIntegrationTests(CoolLibraryWebApplicationFactory webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
        _httpClient = webApplicationFactory.CreateClient();
    }

    [Fact]
    public async Task GetAllBooks_ShouldReturnSuccessStatusCode()
    {
        // Act
        var response = await _httpClient.GetAsync("/api/v1/Books/ListBooks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAllBooks_ShouldReturnEmptyList_WhenNoBooksExist()
    {
        // Arrange - Clean database
        using (var scope = _webApplicationFactory.Services.CreateScope())
        {
            var databaseContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
            databaseContext.Books.RemoveRange(databaseContext.Books);
            databaseContext.Authors.RemoveRange(databaseContext.Authors);
            await databaseContext.SaveChangesAsync();
        }

        // Act
        var response = await _httpClient.GetAsync("/api/v1/Books/ListBooks");
        var booksList = await response.Content.ReadFromJsonAsync<List<BookDTO>>();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        booksList.Should().NotBeNull();
        booksList.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllBooks_ShouldReturnBooks_WhenBooksExistInDatabase()
    {
        // Arrange
        await SeedBooksIntoDatabase();

        // Act
        var response = await _httpClient.GetAsync("/api/v1/Books/ListBooks");
        var booksList = await response.Content.ReadFromJsonAsync<List<BookDTO>>();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        booksList.Should().NotBeNull();
        booksList.Should().HaveCountGreaterThan(0);
        booksList!.First().Title.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetAllBooks_ShouldUseCacheOnSecondRequest()
    {
        // Arrange
        await SeedBooksIntoDatabase();

        // Act - First call (should hit database)
        var firstResponse = await _httpClient.GetAsync("/api/v1/Books/ListBooks");
        var firstBooksList = await firstResponse.Content.ReadFromJsonAsync<List<BookDTO>>();

        // Act - Second call (should hit cache)
        var secondResponse = await _httpClient.GetAsync("/api/v1/Books/ListBooks");
        var secondBooksList = await secondResponse.Content.ReadFromJsonAsync<List<BookDTO>>();

        // Assert
        firstResponse.IsSuccessStatusCode.Should().BeTrue();
        secondResponse.IsSuccessStatusCode.Should().BeTrue();
        firstBooksList.Should().BeEquivalentTo(secondBooksList);
    }

    private async Task SeedBooksIntoDatabase()
    {
        using var scope = _webApplicationFactory.Services.CreateScope();
        var databaseContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

        // Clean existing data
        databaseContext.Books.RemoveRange(databaseContext.Books);
        databaseContext.Authors.RemoveRange(databaseContext.Authors);
        await databaseContext.SaveChangesAsync();

        // Create test author
        var testAuthor = new Author
        {
            FirstName = "Integration",
            LastName = "Test Author",
            NormalizedFullName = "INTEGRATION TEST AUTHOR",
            Biography = "Biography for integration testing",
            Nationality = "Test Country"
        };
        databaseContext.Authors.Add(testAuthor);
        await databaseContext.SaveChangesAsync();

        // Create test books
        var testBook1 = new Book
        {
            Title = "Integration Test Book One",
            ISBN = "TEST-ISBN-001",
            PublicationDate = DateTime.Now.AddYears(-1),
            AvailableCopies = 5,
            TotalCopies = 5
        };

        var testBook2 = new Book
        {
            Title = "Integration Test Book Two",
            ISBN = "TEST-ISBN-002",
            PublicationDate = DateTime.Now.AddYears(-2),
            AvailableCopies = 3,
            TotalCopies = 3
        };

        databaseContext.Books.AddRange(testBook1, testBook2);
        await databaseContext.SaveChangesAsync();

        // Create BookAuthor relationships
        var bookAuthor1 = new BookAuthor
        {
            BookId = testBook1.BookId,
            AuthorId = testAuthor.AuthorId
        };

        var bookAuthor2 = new BookAuthor
        {
            BookId = testBook2.BookId,
            AuthorId = testAuthor.AuthorId
        };

        databaseContext.Set<BookAuthor>().AddRange(bookAuthor1, bookAuthor2);
        await databaseContext.SaveChangesAsync();
    }
}