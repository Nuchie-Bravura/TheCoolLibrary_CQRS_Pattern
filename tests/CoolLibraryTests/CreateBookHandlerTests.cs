using AutoMapper;
using CoolLibrary.Application.DTO.Book;
using CoolLibrary.Application.UseCases.Books.Commands.CreateBook;
using CoolLibrary.Domain.Contracts;
using CoolLibrary.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace CoolLibraryTests
{
    /// <summary>
    /// Unit tests for CreateBookHandler
    /// </summary>
    [TestClass]
    public sealed class CreateBookHandlerTests
    {
        // Mock objects
        private Mock<IBooks> _mockBooksRepository = null!;
        private Mock<IAuthors> _mockAuthorsRepository = null!;
        private Mock<IArchiveStorage> _mockArchiveStorage = null!;
        private Mock<IMapper> _mockMapper = null!;
        private Mock<ILogger<CreateBookHandler>> _mockLogger = null!;

        // The handler we're testing
        private CreateBookHandler _createBookHandler = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockBooksRepository = new Mock<IBooks>();
            _mockAuthorsRepository = new Mock<IAuthors>();
            _mockArchiveStorage = new Mock<IArchiveStorage>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<CreateBookHandler>>();

            _createBookHandler = new CreateBookHandler(
                _mockBooksRepository.Object,
                _mockAuthorsRepository.Object,
                _mockArchiveStorage.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );
        }

        [TestMethod]
        public async Task Handle_WithValidData_ShouldCreateBookSuccessfully()
        {
            // ARRANGE
            var authorId = 1;
            var createBookDto = new CreateBookRequestDTO
            {
                Title = "Clean Code",
                ISBN = "978-0132350884",
                AvailableCopies = 5,
                TotalCopies = 10,
                Authors = new List<int> { authorId }
            };

            var bookEntity = new Book
            {
                BookId = 1,
                Title = "Clean Code",
                ISBN = "978-0132350884",
                AvailableCopies = 5,
                TotalCopies = 10
            };

            var expectedResponse = new CreateBookResponseDTO
            {
                BookId = bookEntity.BookId,
                Title = "Clean Code",
                CreatedAt = DateTime.UtcNow
            };

            _mockAuthorsRepository
                .Setup(repo => repo.GetByIdAsync(authorId))
                .ReturnsAsync(new Author 
                { 
                    AuthorId = authorId,
                    FirstName = "Robert",
                    LastName = "Martin"
                });

            _mockMapper
                .Setup(m => m.Map<Book>(createBookDto))
                .Returns(bookEntity);

            _mockBooksRepository
                .Setup(repo => repo.InsertAsync(It.IsAny<Book>()))
                .ReturnsAsync(bookEntity);

            _mockMapper
                .Setup(m => m.Map<CreateBookResponseDTO>(bookEntity))
                .Returns(expectedResponse);

            var command = new CreateBookCommand(createBookDto);

            // ACT
            var result = await _createBookHandler.Handle(command, CancellationToken.None);

            // ASSERT
            result.Should().NotBeNull();
            result.BookId.Should().Be(expectedResponse.BookId);
            result.Title.Should().Be("Clean Code");

            _mockAuthorsRepository.Verify(repo => repo.GetByIdAsync(authorId), Times.Once);
            _mockBooksRepository.Verify(repo => repo.InsertAsync(It.IsAny<Book>()), Times.Once);
        }

        [TestMethod]
        public async Task Handle_WhenAvailableCopiesExceedTotal_ShouldThrowArgumentException()
        {
            // ARRANGE
            var createBookDto = new CreateBookRequestDTO
            {
                Title = "Test Book",
                AvailableCopies = 15,
                TotalCopies = 10,
                Authors = new List<int> { 1 }
            };

            var command = new CreateBookCommand(createBookDto);

            // ACT & ASSERT
            var act = async () => await _createBookHandler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<ApplicationException>()
                .WithMessage("*Available copies cannot exceed total copies*");

            _mockBooksRepository.Verify(repo => repo.InsertAsync(It.IsAny<Book>()), Times.Never);
        }

        [TestMethod]
        public async Task Handle_WhenNoAuthorsProvided_ShouldThrowArgumentException()
        {
            // ARRANGE
            var createBookDto = new CreateBookRequestDTO
            {
                Title = "Test Book",
                AvailableCopies = 5,
                TotalCopies = 10,
                Authors = new List<int>()
            };

            var command = new CreateBookCommand(createBookDto);

            // ACT & ASSERT
            var act = async () => await _createBookHandler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<ApplicationException>()
                .WithMessage("*At least one author must be specified*");
        }

        [TestMethod]
        public async Task Handle_WhenAuthorDoesNotExist_ShouldThrowArgumentException()
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

            _mockAuthorsRepository
                .Setup(repo => repo.GetByIdAsync(nonExistentAuthorId))
                .ReturnsAsync((Author?)null);

            var command = new CreateBookCommand(createBookDto);

            // ACT & ASSERT
            var act = async () => await _createBookHandler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<ApplicationException>()
                .WithMessage($"*Author with ID {nonExistentAuthorId} does not exist*");
        }

        [TestMethod]
        public async Task Handle_WhenCopiesAreNegative_ShouldThrowArgumentException()
        {
            // ARRANGE
            var createBookDto = new CreateBookRequestDTO
            {
                Title = "Test Book",
                AvailableCopies = -5,
                TotalCopies = 10,
                Authors = new List<int> { 1 }
            };

            var command = new CreateBookCommand(createBookDto);

            // ACT & ASSERT
            var act = async () => await _createBookHandler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<ApplicationException>()
                .WithMessage("*Available copies and total copies must be greater than or equal to 0*");
        }
    }
}
