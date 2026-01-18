using CoolLibrary.Application.DTO.LoansAndReservations;
using CoolLibrary.Application.UseCases.Loans.Commands.RequestLoan;
using CoolLibrary.Domain.Contracts;
using CoolLibrary.Domain.Entities;
using CoolLibrary.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CoolLibraryTests
{
    [TestClass]
    public sealed class RequestLoanHandlerTests
    {
        private Mock<ILoans> _mockLoansRepository = null!;
        private Mock<IBooks> _mockBooksRepository = null!;
        private Mock<ICustomers> _mockCustomersRepository = null!;
        private Mock<ILogger<RequestLoanHandler>> _mockLogger = null!;
        private RequestLoanHandler _handler = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockLoansRepository = new Mock<ILoans>();
            _mockBooksRepository = new Mock<IBooks>();
            _mockCustomersRepository = new Mock<ICustomers>();
            _mockLogger = new Mock<ILogger<RequestLoanHandler>>();

            _handler = new RequestLoanHandler(
                _mockLoansRepository.Object,
                _mockBooksRepository.Object,
                _mockCustomersRepository.Object,
                _mockLogger.Object
            );
        }

        [TestMethod]
        public async Task Handle_WithValidRequest_ShouldCreateLoan()
        {
            // ARRANGE
            var userId = "user-123";
            var bookId = 1;
            var customer = new Customer { CustomerId = 1, MembershipStatus = MembershipStatus.Active };
            var book = new Book { BookId = bookId, AvailableCopies = 5 };
            var command = new RequestLoanCommand(userId, bookId);

            _mockCustomersRepository.Setup(r => r.GetCustomerByUserIdAsync(userId)).ReturnsAsync(customer);
            _mockBooksRepository.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync(book);
            _mockLoansRepository.Setup(r => r.GetActiveLoanCountForCustomerAsync(customer.CustomerId)).ReturnsAsync(0);
            _mockLoansRepository.Setup(r => r.CreateAsync(It.IsAny<Loan>()))
                .ReturnsAsync(new Loan { LoanId = 100, CustomerId = customer.CustomerId, BookId = bookId, Status = LoanStatus.Active });

            // ACT
            var result = await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            result.Should().NotBeNull();
            result.LoanId.Should().Be(100);
            _mockLoansRepository.Verify(r => r.CreateAsync(It.IsAny<Loan>()), Times.Once);
            _mockBooksRepository.Verify(r => r.UpdateAvailableCopiesAsync(bookId, 4), Times.Once);
        }

        [TestMethod]
        public async Task Handle_WhenBookUnavailable_ShouldThrowException()
        {
            // ARRANGE
            var userId = "user-123";
            var bookId = 1;
            var customer = new Customer { CustomerId = 1 };
            var book = new Book { BookId = bookId, AvailableCopies = 0 };
            var command = new RequestLoanCommand(userId, bookId);

            _mockCustomersRepository.Setup(r => r.GetCustomerByUserIdAsync(userId)).ReturnsAsync(customer);
            _mockBooksRepository.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync(book);

            // ACT
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Book is not available for loan.");
        }
    }
}
