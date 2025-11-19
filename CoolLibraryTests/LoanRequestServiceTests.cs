using AutoMapper;
using CoolLibrary.Application.DTO.LoansAndReservations;
using CoolLibrary.Application.Services.LoansAndReservations;
using CoolLibrary.Domain.Contracts;
using CoolLibrary.Domain.Entities;
using CoolLibrary.Domain.Enums;
using FluentAssertions;
using Moq;

namespace CoolLibraryTests
{
    /// <summary>
    /// Unit tests for LoanRequestService
    /// Demonstrates testing business logic with multiple dependencies and complex scenarios
    /// </summary>
    [TestClass]
    public sealed class LoanRequestServiceTests
    {
        // Mock dependencies
        private Mock<IBooks> _mockBooksRepository = null!;
        private Mock<ICustomers> _mockCustomersRepository = null!;
        private Mock<ILoans> _mockLoansRepository = null!;
        private Mock<IMapper> _mockMapper = null!;

        // Service under test
        private LoanRequestService _loanRequestService = null!;

        /// <summary>
        /// Setup runs before EACH test to ensure test isolation
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            // Create fresh mocks for each test
            _mockBooksRepository = new Mock<IBooks>();
            _mockCustomersRepository = new Mock<ICustomers>();
            _mockLoansRepository = new Mock<ILoans>();
            _mockMapper = new Mock<IMapper>();

            // Create service with mocked dependencies
            _loanRequestService = new LoanRequestService(
                _mockBooksRepository.Object,
                _mockCustomersRepository.Object,
                _mockLoansRepository.Object,
                _mockMapper.Object
            );
        }

        #region GetAvailabilityAsync Tests

        /// <summary>
        /// Test: Get availability for existing book
        /// </summary>
        [TestMethod]
        public async Task GetAvailabilityAsync_WithExistingBook_ShouldReturnAvailability()
        {
            // ARRANGE
            var bookId = 1;
            var book = new Book
            {
                BookId = bookId,
                Title = "Test Book",
                AvailableCopies = 5,
                TotalCopies = 10
            };

            _mockBooksRepository
                .Setup(repo => repo.GetByIdAsync(bookId))
                .ReturnsAsync(book);

            // ACT
            var result = await _loanRequestService.GetAvailabilityAsync(bookId);

            // ASSERT
            result.Should().NotBeNull();
            result!.BookId.Should().Be(bookId);
            result.AvailableCopies.Should().Be(5);
            result.IsAvailable.Should().BeTrue();
        }

        /// <summary>
        /// Test: Get availability for non-existent book
        /// </summary>
        [TestMethod]
        public async Task GetAvailabilityAsync_WithNonExistentBook_ShouldReturnNull()
        {
            // ARRANGE
            var bookId = 999;
            _mockBooksRepository
                .Setup(repo => repo.GetByIdAsync(bookId))
                .ReturnsAsync((Book?)null);

            // ACT
            var result = await _loanRequestService.GetAvailabilityAsync(bookId);

            // ASSERT
            result.Should().BeNull();
        }

        /// <summary>
        /// Test: Get availability when no copies available
        /// </summary>
        [TestMethod]
        public async Task GetAvailabilityAsync_WhenNoCopiesAvailable_ShouldReturnNotAvailable()
        {
            // ARRANGE
            var bookId = 1;
            var book = new Book
            {
                BookId = bookId,
                Title = "Test Book",
                AvailableCopies = 0,  // No copies available!
                TotalCopies = 10
            };

            _mockBooksRepository
                .Setup(repo => repo.GetByIdAsync(bookId))
                .ReturnsAsync(book);

            // ACT
            var result = await _loanRequestService.GetAvailabilityAsync(bookId);

            // ASSERT
            result.Should().NotBeNull();
            result!.IsAvailable.Should().BeFalse();
            result.AvailableCopies.Should().Be(0);
        }

        #endregion

        #region RequestLoanAsync Tests - Happy Path

        /// <summary>
        /// Test: Successful loan request with all valid conditions
        /// This is the happy path - everything works correctly
        /// </summary>
        [TestMethod]
        public async Task RequestLoanAsync_WithValidData_ShouldCreateLoanSuccessfully()
        {
            // ARRANGE
            var bookId = 1;
            var customerId = 1;
            var loanRequest = new LoanRequestDTO
            {
                BookId = bookId,
                CustomerId = customerId
            };

            var book = new Book
            {
                BookId = bookId,
                Title = "Clean Code",
                AvailableCopies = 5,
                TotalCopies = 10
            };

            var customer = new Customer
            {
                CustomerId = customerId,
                MembershipStatus = MembershipStatus.Active,
                MaxBooksAllowed = 5
            };

            var createdLoan = new Loan
            {
                LoanId = 1,
                BookId = bookId,
                CustomerId = customerId,
                LoanDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14),
                Status = LoanStatus.Active
            };

            // Setup mocks
            _mockBooksRepository.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync(book);
            _mockCustomersRepository.Setup(repo => repo.GetByIdAsync(customerId)).ReturnsAsync(customer);
            _mockLoansRepository.Setup(repo => repo.GetActiveLoanCountForCustomerAsync(customerId)).ReturnsAsync(0);
            _mockLoansRepository.Setup(repo => repo.HasActiveLoanForBookAsync(customerId, bookId)).ReturnsAsync(false);
            _mockLoansRepository.Setup(repo => repo.CreateAsync(It.IsAny<Loan>())).ReturnsAsync(createdLoan);
            _mockBooksRepository.Setup(repo => repo.UpdateAvailableCopiesAsync(bookId, It.IsAny<int>())).ReturnsAsync(true);

            // ACT
            var (ok, error, loan) = await _loanRequestService.RequestLoanAsync(loanRequest);

            // ASSERT
            ok.Should().BeTrue();
            error.Should().BeNull();
            loan.Should().NotBeNull();
            loan!.LoanId.Should().Be(1);
            loan.BookId.Should().Be(bookId);
            loan.CustomerId.Should().Be(customerId);
            loan.Status.Should().Be("Active");

            // Verify all dependencies were called correctly
            _mockLoansRepository.Verify(repo => repo.CreateAsync(It.IsAny<Loan>()), Times.Once);
            _mockBooksRepository.Verify(repo => repo.UpdateAvailableCopiesAsync(bookId, It.Is<int>(copies => copies == 4)), Times.Once);
        }

        #endregion

        #region RequestLoanAsync Tests - Error Scenarios

        /// <summary>
        /// Test: Should fail when book doesn't exist
        /// </summary>
        [TestMethod]
        public async Task RequestLoanAsync_WhenBookNotFound_ShouldReturnError()
        {
            // ARRANGE
            var loanRequest = new LoanRequestDTO
            {
                BookId = 999,
                CustomerId = 1
            };

            _mockBooksRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((Book?)null);

            // ACT
            var (ok, error, loan) = await _loanRequestService.RequestLoanAsync(loanRequest);

            // ASSERT
            ok.Should().BeFalse();
            error.Should().Be("Book not found");
            loan.Should().BeNull();

            // Verify no loan was created
            _mockLoansRepository.Verify(repo => repo.CreateAsync(It.IsAny<Loan>()), Times.Never);
        }

        /// <summary>
        /// Test: Should fail when customer doesn't exist
        /// </summary>
        [TestMethod]
        public async Task RequestLoanAsync_WhenCustomerNotFound_ShouldReturnError()
        {
            // ARRANGE
            var bookId = 1;
            var loanRequest = new LoanRequestDTO
            {
                BookId = bookId,
                CustomerId = 999
            };

            var book = new Book { BookId = bookId, AvailableCopies = 5 };

            _mockBooksRepository.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync(book);
            _mockCustomersRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((Customer?)null);

            // ACT
            var (ok, error, loan) = await _loanRequestService.RequestLoanAsync(loanRequest);

            // ASSERT
            ok.Should().BeFalse();
            error.Should().Be("Customer not found");
            loan.Should().BeNull();
        }

        /// <summary>
        /// Test: Should fail when book has no available copies
        /// </summary>
        [TestMethod]
        public async Task RequestLoanAsync_WhenNoAvailableCopies_ShouldReturnError()
        {
            // ARRANGE
            var bookId = 1;
            var customerId = 1;
            var loanRequest = new LoanRequestDTO
            {
                BookId = bookId,
                CustomerId = customerId
            };

            var book = new Book
            {
                BookId = bookId,
                AvailableCopies = 0,  // No copies!
                TotalCopies = 10
            };

            var customer = new Customer
            {
                CustomerId = customerId,
                MembershipStatus = MembershipStatus.Active
            };

            _mockBooksRepository.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync(book);
            _mockCustomersRepository.Setup(repo => repo.GetByIdAsync(customerId)).ReturnsAsync(customer);

            // ACT
            var (ok, error, loan) = await _loanRequestService.RequestLoanAsync(loanRequest);

            // ASSERT
            ok.Should().BeFalse();
            error.Should().Be("Book is not available");
            loan.Should().BeNull();
        }

        /// <summary>
        /// Test: Should fail when customer is not active
        /// </summary>
        [TestMethod]
        public async Task RequestLoanAsync_WhenCustomerNotActive_ShouldReturnError()
        {
            // ARRANGE
            var bookId = 1;
            var customerId = 1;
            var loanRequest = new LoanRequestDTO
            {
                BookId = bookId,
                CustomerId = customerId
            };

            var book = new Book { BookId = bookId, AvailableCopies = 5 };
            var customer = new Customer
            {
                CustomerId = customerId,
                MembershipStatus = MembershipStatus.Suspended  // Not active!
            };

            _mockBooksRepository.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync(book);
            _mockCustomersRepository.Setup(repo => repo.GetByIdAsync(customerId)).ReturnsAsync(customer);

            // ACT
            var (ok, error, loan) = await _loanRequestService.RequestLoanAsync(loanRequest);

            // ASSERT
            ok.Should().BeFalse();
            error.Should().Be("Customer is not active");
            loan.Should().BeNull();
        }

        /// <summary>
        /// Test: Should fail when customer reached max books allowed
        /// </summary>
        [TestMethod]
        public async Task RequestLoanAsync_WhenMaxBooksReached_ShouldReturnError()
        {
            // ARRANGE
            var bookId = 1;
            var customerId = 1;
            var loanRequest = new LoanRequestDTO
            {
                BookId = bookId,
                CustomerId = customerId
            };

            var book = new Book { BookId = bookId, AvailableCopies = 5 };
            var customer = new Customer
            {
                CustomerId = customerId,
                MembershipStatus = MembershipStatus.Active,
                MaxBooksAllowed = 3  // Max is 3
            };

            _mockBooksRepository.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync(book);
            _mockCustomersRepository.Setup(repo => repo.GetByIdAsync(customerId)).ReturnsAsync(customer);
            _mockLoansRepository
                .Setup(repo => repo.GetActiveLoanCountForCustomerAsync(customerId))
                .ReturnsAsync(3);  // Already has 3 active loans!

            // ACT
            var (ok, error, loan) = await _loanRequestService.RequestLoanAsync(loanRequest);

            // ASSERT
            ok.Should().BeFalse();
            error.Should().Be("Customer reached the maximum number of active loans");
            loan.Should().BeNull();
        }

        /// <summary>
        /// Test: Should fail when customer already has this book
        /// </summary>
        [TestMethod]
        public async Task RequestLoanAsync_WhenCustomerAlreadyHasBook_ShouldReturnError()
        {
            // ARRANGE
            var bookId = 1;
            var customerId = 1;
            var loanRequest = new LoanRequestDTO
            {
                BookId = bookId,
                CustomerId = customerId
            };

            var book = new Book { BookId = bookId, AvailableCopies = 5 };
            var customer = new Customer
            {
                CustomerId = customerId,
                MembershipStatus = MembershipStatus.Active,
                MaxBooksAllowed = 5
            };

            _mockBooksRepository.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync(book);
            _mockCustomersRepository.Setup(repo => repo.GetByIdAsync(customerId)).ReturnsAsync(customer);
            _mockLoansRepository.Setup(repo => repo.GetActiveLoanCountForCustomerAsync(customerId)).ReturnsAsync(1);
            _mockLoansRepository
                .Setup(repo => repo.HasActiveLoanForBookAsync(customerId, bookId))
                .ReturnsAsync(true);  // Already has this book!

            // ACT
            var (ok, error, loan) = await _loanRequestService.RequestLoanAsync(loanRequest);

            // ASSERT
            ok.Should().BeFalse();
            error.Should().Be("Customer already has an active loan for this book");
            loan.Should().BeNull();
        }

        #endregion

        /// <summary>
        /// Cleanup after each test
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            // Mocks are automatically garbage collected
        }
    }
}
