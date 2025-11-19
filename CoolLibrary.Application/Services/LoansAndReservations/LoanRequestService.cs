using AutoMapper;
using CoolLibrary.Application.DTO.LoansAndReservations;
using CoolLibrary.Domain.Contracts;
using CoolLibrary.Domain.Entities;
using CoolLibrary.Domain.Enums;

namespace CoolLibrary.Application.Services.LoansAndReservations;

public class LoanRequestService
{
    private readonly IBooks _books;
    private readonly ICustomers _customers;
    private readonly ILoans _loans;
    private readonly IMapper _mapper;

    public LoanRequestService(IBooks books, ICustomers customers, ILoans loans, IMapper mapper)
    {
        _books = books;
        _customers = customers;
        _loans = loans;
        _mapper = mapper;
    }

    public async Task<AvailabilityDTO?> GetAvailabilityAsync(int bookId)
    {
        var book = await _books.GetByIdAsync(bookId);
        if (book == null) return null;
        return new AvailabilityDTO
        {
            BookId = bookId,
            AvailableCopies = book.AvailableCopies,
            IsAvailable = book.AvailableCopies > 0
        };
    }

    public async Task<(bool ok, string? error, LoanResponseDTO? loan)> RequestLoanAsync(LoanRequestDTO request)
    {
        // Load book and customer
        var book = await _books.GetByIdAsync(request.BookId);
        if (book == null) return (false, "Book not found", null);

        
        var customer = await _customers.GetByIdAsync(request.CustomerId);
        if (customer == null) return (false, "Customer not found", null);

        // Business rules
        if (book.AvailableCopies <= 0)
            return (false, "Book is not available", null);

        if (customer.MembershipStatus != MembershipStatus.Active)
            return (false, "Customer is not active", null);

        var activeCount = await _loans.GetActiveLoanCountForCustomerAsync(customer.CustomerId);
        if (activeCount >= customer.MaxBooksAllowed)
            return (false, "Customer reached the maximum number of active loans", null);

        var alreadyHas = await _loans.HasActiveLoanForBookAsync(customer.CustomerId, book.BookId);
        if (alreadyHas)
            return (false, "Customer already has an active loan for this book", null);

        // Create loan
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

        // Persist
        var created = await _loans.CreateAsync(loan);

        // Decrement copies using books repository method already present
        await _books.UpdateAvailableCopiesAsync(book.BookId, book.AvailableCopies - 1);

        var response = new LoanResponseDTO
        {
            LoanId = created.LoanId,
            CustomerId = created.CustomerId,
            BookId = created.BookId,
            LoanDate = created.LoanDate,
            DueDate = created.DueDate,
            Status = created.Status.ToString()
        };

        return (true, null, response);
    }

    /// <summary>
    /// ? MÉTODO SEGURO: Crea préstamo usando UserId del JWT
    /// El CustomerId se obtiene internamente, no del cliente
    /// Previene spoofing attacks
    /// </summary>
    /// <param name="userId">UserId from JWT token (ClaimTypes.NameIdentifier)</param>
    /// <param name="bookId">Book ID to borrow</param>
    /// <returns>Result tuple with success flag, error message, and loan response</returns>
    public async Task<(bool ok, string? error, LoanResponseDTO? loan)> RequestLoanSecureAsync(string userId, int bookId)
    {
        // 1. Load customer by UserId (from JWT)
        var customer = await _customers.GetCustomerByUserIdAsync(userId);
        if (customer == null)
            return (false, "Customer profile not found for this user", null);

        // 2. Load book
        var book = await _books.GetByIdAsync(bookId);
        if (book == null)
            return (false, "Book not found", null);

        // 3. Business rules validation
        if (book.AvailableCopies <= 0)
            return (false, "Book is not available", null);

        if (customer.MembershipStatus != MembershipStatus.Active)
            return (false, "Customer is not active", null);

        var activeCount = await _loans.GetActiveLoanCountForCustomerAsync(customer.CustomerId);
        if (activeCount >= customer.MaxBooksAllowed)
            return (false, $"Customer reached the maximum number of active loans ({customer.MaxBooksAllowed})", null);

        var alreadyHas = await _loans.HasActiveLoanForBookAsync(customer.CustomerId, book.BookId);
        if (alreadyHas)
            return (false, "Customer already has an active loan for this book", null);

        // 4. Create loan
        var now = DateTime.UtcNow;
        var due = now.AddDays(14);

        var loan = new Loan
        {
            CustomerId = customer.CustomerId,  // ? UserId del JWT, no del cliente
            BookId = book.BookId,
            LoanDate = now,
            DueDate = due,
            Status = LoanStatus.Active,
            RenewalCount = 0,
            LateFee = 0,
            CreatedAt = now,
            UpdatedAt = now
        };

        // 5. Persist
        var created = await _loans.CreateAsync(loan);

        // 6. Decrement available copies
        await _books.UpdateAvailableCopiesAsync(book.BookId, book.AvailableCopies - 1);

        // 7. Create response
        var response = new LoanResponseDTO
        {
            LoanId = created.LoanId,
            CustomerId = created.CustomerId,
            BookId = created.BookId,
            LoanDate = created.LoanDate,
            DueDate = created.DueDate,
            Status = created.Status.ToString()
        };

        return (true, null, response);
    }
}
