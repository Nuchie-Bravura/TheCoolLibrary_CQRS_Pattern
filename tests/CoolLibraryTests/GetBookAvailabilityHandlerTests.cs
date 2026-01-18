using CoolLibrary.Application.UseCases.Loans.Queries.GetBookAvailability;
using CoolLibrary.Domain.Contracts;
using CoolLibrary.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CoolLibraryTests
{
    [TestClass]
    public sealed class GetBookAvailabilityHandlerTests
    {
        private Mock<IBooks> _mockBooksRepository = null!;
        private Mock<ILogger<GetBookAvailabilityHandler>> _mockLogger = null!;
        private GetBookAvailabilityHandler _handler = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockBooksRepository = new Mock<IBooks>();
            _mockLogger = new Mock<ILogger<GetBookAvailabilityHandler>>();
            _handler = new GetBookAvailabilityHandler(_mockBooksRepository.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task Handle_WithExistingBook_ShouldReturnAvailability()
        {
            // ARRANGE
            var bookId = 1;
            var book = new Book { BookId = bookId, AvailableCopies = 5 };
            _mockBooksRepository.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync(book);

            // ACT
            var result = await _handler.Handle(new GetBookAvailabilityQuery(bookId), CancellationToken.None);

            // ASSERT
            result.Should().NotBeNull();
            result!.BookId.Should().Be(bookId);
            result.IsAvailable.Should().BeTrue();
            result.AvailableCopies.Should().Be(5);
        }

        [TestMethod]
        public async Task Handle_WithNonExistentBook_ShouldReturnNull()
        {
            // ARRANGE
            _mockBooksRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Book?)null);

            // ACT
            var result = await _handler.Handle(new GetBookAvailabilityQuery(999), CancellationToken.None);

            // ASSERT
            result.Should().BeNull();
        }
    }
}
