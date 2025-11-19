using AutoMapper;
using CoolLibrary.Application.DTO.Book;
using CoolLibrary.Application.Services.Books;
using CoolLibrary.Domain.Contracts;
using CoolLibrary.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace CoolLibraryTests
{
    /// <summary>
    /// Unit tests for CreateBookService
    /// This demonstrates best practices for unit testing in .NET 9 with MSTest
    /// </summary>
    [TestClass]
    public sealed class CreateBookServiceTests
    {
        // Mock objects - these simulate dependencies without needing real implementations
        private Mock<IBooks> _mockBooksRepository = null!;
        private Mock<IAuthors> _mockAuthorsRepository = null!;
        private Mock<IArchiveStorage> _mockArchiveStorage = null!;
        private Mock<IMapper> _mockMapper = null!;
        private Mock<ILogger<CreateBookService>> _mockLogger = null!;

        // The service we're testing
        private CreateBookService _createBookService = null!;

        /// <summary>
        /// This runs before EACH test method
        /// Sets up fresh mock objects for each test to ensure test isolation
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            // Create fresh mocks for each test
            _mockBooksRepository = new Mock<IBooks>();
            _mockAuthorsRepository = new Mock<IAuthors>();
            _mockArchiveStorage = new Mock<IArchiveStorage>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<CreateBookService>>();

            // Create the service with mocked dependencies
            _createBookService = new CreateBookService(
                _mockBooksRepository.Object,
                _mockAuthorsRepository.Object,
                _mockArchiveStorage.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );
        }

        /// <summary>
        /// Test: Successful book creation with valid data
        /// This follows the AAA pattern: Arrange, Act, Assert
        /// </summary>
        [TestMethod]
        public async Task ExecuteAsync_WithValidData_ShouldCreateBookSuccessfully()
        {
            // ARRANGE - Set up test data and configure mock behavior
            var authorId = 1;  // Using int as per your actual implementation
            var createBookDto = new CreateBookRequestDTO
            {
                Title = "Clean Code",
                ISBN = "978-0132350884",
                AvailableCopies = 5,
                TotalCopies = 10,
                Authors = new List<int> { authorId }  // int, not Guid
            };

            var bookEntity = new Book
            {
                BookId = 1,  // int BookId, not Guid Id
                Title = "Clean Code",
                ISBN = "978-0132350884",
                AvailableCopies = 5,
                TotalCopies = 10
            };

            var expectedResponse = new CreateBookResponseDTO
            {
                BookId = bookEntity.BookId,  // BookId property
                Title = "Clean Code",
                CreatedAt = DateTime.UtcNow
            };

            // Configure mock: When GetByIdAsync is called with authorId, return an author
            _mockAuthorsRepository
                .Setup(repo => repo.GetByIdAsync(authorId))
                .ReturnsAsync(new Author 
                { 
                    AuthorId = authorId,  // AuthorId property
                    FirstName = "Robert",
                    LastName = "Martin"
                });

            // Configure mock: Map CreateBookRequestDTO to Book entity
            _mockMapper
                .Setup(m => m.Map<Book>(createBookDto))
                .Returns(bookEntity);

            // Configure mock: InsertAsync returns the created book
            _mockBooksRepository
                .Setup(repo => repo.InsertAsync(It.IsAny<Book>()))
                .ReturnsAsync(bookEntity);

            // Configure mock: Map Book entity to CreateBookResponseDTO
            _mockMapper
                .Setup(m => m.Map<CreateBookResponseDTO>(bookEntity))
                .Returns(expectedResponse);

            // ACT - Execute the method we're testing
            var result = await _createBookService.ExecuteAsync(createBookDto);

            // ASSERT - Verify the results using FluentAssertions (more readable than standard assertions)
            result.Should().NotBeNull();
            result.BookId.Should().Be(expectedResponse.BookId);
            result.Title.Should().Be("Clean Code");

            // Verify that the repository methods were called exactly once
            _mockAuthorsRepository.Verify(repo => repo.GetByIdAsync(authorId), Times.Once);
            _mockBooksRepository.Verify(repo => repo.InsertAsync(It.IsAny<Book>()), Times.Once);
        }

        /// <summary>
        /// Test: Should throw exception when available copies exceed total copies
        /// This tests validation logic
        /// </summary>
        [TestMethod]
        public async Task ExecuteAsync_WhenAvailableCopiesExceedTotal_ShouldThrowArgumentException()
        {
            // ARRANGE
            var createBookDto = new CreateBookRequestDTO
            {
                Title = "Test Book",
                AvailableCopies = 15,  // More than total!
                TotalCopies = 10,
                Authors = new List<int> { 1 }
            };

            // ACT & ASSERT - The service wraps exceptions in ApplicationException
            var act = async () => await _createBookService.ExecuteAsync(createBookDto);

            await act.Should().ThrowAsync<ApplicationException>()
                .WithMessage("*Available copies cannot exceed total copies*");

            // Verify that repository was never called (validation failed first)
            _mockBooksRepository.Verify(repo => repo.InsertAsync(It.IsAny<Book>()), Times.Never);
        }

        /// <summary>
        /// Test: Should throw exception when no authors are provided
        /// </summary>
        [TestMethod]
        public async Task ExecuteAsync_WhenNoAuthorsProvided_ShouldThrowArgumentException()
        {
            // ARRANGE
            var createBookDto = new CreateBookRequestDTO
            {
                Title = "Test Book",
                AvailableCopies = 5,
                TotalCopies = 10,
                Authors = new List<int>()  // Empty authors list
            };

            // ACT & ASSERT - The service wraps exceptions in ApplicationException
            var act = async () => await _createBookService.ExecuteAsync(createBookDto);

            await act.Should().ThrowAsync<ApplicationException>()
                .WithMessage("*At least one author must be specified*");
        }

        /// <summary>
        /// Test: Should throw exception when author doesn't exist
        /// </summary>
        [TestMethod]
        public async Task ExecuteAsync_WhenAuthorDoesNotExist_ShouldThrowArgumentException()
        {
            // ARRANGE
            var nonExistentAuthorId = 999;
            var createBookDto = new CreateBookRequestDTO
            {
                Title = "Test Book",
                AvailableCopies = 5,
                TotalCopies = 10,
                Authors = new List<int> { nonExistentAuthorId }
            };

            // Configure mock to return null (author not found)
            _mockAuthorsRepository
                .Setup(repo => repo.GetByIdAsync(nonExistentAuthorId))
                .ReturnsAsync((Author?)null);

            // ACT & ASSERT - The service wraps exceptions in ApplicationException
            var act = async () => await _createBookService.ExecuteAsync(createBookDto);

            await act.Should().ThrowAsync<ApplicationException>()
                .WithMessage($"*Author with ID {nonExistentAuthorId} does not exist*");
        }

        /// <summary>
        /// Test: Should throw exception when copies are negative
        /// </summary>
        [TestMethod]
        public async Task ExecuteAsync_WhenCopiesAreNegative_ShouldThrowArgumentException()
        {
            // ARRANGE
            var createBookDto = new CreateBookRequestDTO
            {
                Title = "Test Book",
                AvailableCopies = -5,  // Negative!
                TotalCopies = 10,
                Authors = new List<int> { 1 }
            };

            // ACT & ASSERT - The service wraps exceptions in ApplicationException
            var act = async () => await _createBookService.ExecuteAsync(createBookDto);

            await act.Should().ThrowAsync<ApplicationException>()
                .WithMessage("*Available copies and total copies must be greater than or equal to 0*");
        }

        /// <summary>
        /// Clean up resources after each test (optional in this case)
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            // In this case, mocks are garbage collected automatically
            // But you can add cleanup logic here if needed
        }
    }
}
