using CoolLibrary.Domain.Contracts;
using CoolLibrary.Domain.Entities;
using CoolLibrary.Domain.Enums;
using CoolLibrary.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CoolLibrary.Infrastructure.Repositories;

public class LoansRepository : ILoans
{
    private readonly LibraryDbContext _context;

    public LoansRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<Loan> CreateAsync(Loan loan)
    {
        loan.CreatedAt = DateTime.UtcNow;
        loan.UpdatedAt = DateTime.UtcNow;
        await _context.Loans.AddAsync(loan);
        await _context.SaveChangesAsync();
        return loan;
    }

    public async Task<Loan?> GetActiveByIdAsync(int loanId)
    {
        return await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Customer)
            .FirstOrDefaultAsync(l => l.LoanId == loanId && l.Status == LoanStatus.Active);
    }

    public async Task<int> GetActiveLoanCountForCustomerAsync(int customerId)
    {
        return await _context.Loans.CountAsync(l => l.CustomerId == customerId && l.Status == LoanStatus.Active);
    }

    public async Task<bool> HasActiveLoanForBookAsync(int customerId, int bookId)
    {
        return await _context.Loans.AnyAsync(l => l.CustomerId == customerId && l.BookId == bookId && l.Status == LoanStatus.Active);
    }

    public async Task<bool> ReturnAsync(int loanId, DateTime returnDate)
    {
        var loan = await _context.Loans.FindAsync(loanId);
        if (loan == null || loan.Status != LoanStatus.Active) return false;

        loan.Status = LoanStatus.Returned;
        loan.ReturnDate = returnDate;
        loan.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
}
