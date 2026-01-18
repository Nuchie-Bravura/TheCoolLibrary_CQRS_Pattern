using CoolLibrary.Application.DTO.LoansAndReservations;
using CoolLibrary.Domain.Contracts;
using CoolLibrary.Domain.Entities;
using CoolLibrary.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoolLibrary.Application.UseCases.Loans.Commands.RequestLoan
{
    public class RequestLoanHandler : IRequestHandler<RequestLoanCommand, LoanResponseDTO>
    {
        private readonly ILoans _loansRepository;
        private readonly IBooks _booksRepository;
        private readonly ICustomers _customersRepository;
        private readonly ILogger<RequestLoanHandler> _logger;

        public RequestLoanHandler(
            ILoans loansRepository,
            IBooks booksRepository,
            ICustomers customersRepository,
            ILogger<RequestLoanHandler> logger)
        {
            _loansRepository = loansRepository;
            _booksRepository = booksRepository;
            _customersRepository = customersRepository;
            _logger = logger;
        }

        public async Task<LoanResponseDTO> Handle(RequestLoanCommand request, CancellationToken cancellationToken)
        {
            var userId = request.UserId;
            var bookId = request.BookId;

            try
            {
                // 1. Get Customer Profile
                var customer = await _customersRepository.GetCustomerByUserIdAsync(userId);
                if (customer == null)
                {
                    throw new KeyNotFoundException("Customer profile not found for authenticated user.");
                }

                // 2. Validate Book
                var book = await _booksRepository.GetByIdAsync(bookId);
                if (book == null)
                {
                    throw new ArgumentException("Book not found.");
                }

                if (book.AvailableCopies <= 0)
                {
                    throw new InvalidOperationException("Book is not available for loan.");
                }

                // 3. Check active loans limit (e.g. max 3)
                var activeLoanCount = await _loansRepository.GetActiveLoanCountForCustomerAsync(customer.CustomerId);
                if (activeLoanCount >= 3)
                {
                    throw new InvalidOperationException("User has reached maximum active loans limit (3).");
                }

                // 4. Create Loan
                var now = DateTime.UtcNow;
                var due = now.AddDays(14);

                var loan = new Loan
                {
                    CustomerId = customer.CustomerId,
                    BookId = book.BookId,
                    LoanDate = now,
                    DueDate = due,
                    Status = LoanStatus.Active,
                    RenewalCount = 0,
                    LateFee = 0,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                var createdLoan = await _loansRepository.CreateAsync(loan);

                // 5. Update Book Copies
                await _booksRepository.UpdateAvailableCopiesAsync(book.BookId, book.AvailableCopies - 1);

                return new LoanResponseDTO
                {
                    LoanId = createdLoan.LoanId,
                    CustomerId = createdLoan.CustomerId,
                    BookId = createdLoan.BookId,
                    LoanDate = createdLoan.LoanDate,
                    DueDate = createdLoan.DueDate,
                    Status = createdLoan.Status.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing loan request for User {UserId} and Book {BookId}", userId, bookId);
                throw;
            }
        }
    }
}
