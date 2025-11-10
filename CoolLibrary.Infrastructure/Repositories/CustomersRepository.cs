using CoolLibrary.Domain.Contracts;
using CoolLibrary.Domain.Entities;
using CoolLibrary.Domain.Enums;
using CoolLibrary.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CoolLibrary.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing Customer entities
/// </summary>
public class CustomersRepository : ICustomers
{
    private readonly LibraryDbContext _context;

    public CustomersRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _context.Customers
            .Include(c => c.User)  // ? Include ApplicationUser
            .Include(c => c.Loans)
            .Include(c => c.Reservations)
            .Include(c => c.Fines)
            .ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(int customerId)
    {
        return await _context.Customers
            .Include(c => c.User)  // ? Include ApplicationUser
            .Include(c => c.Loans)
            .ThenInclude(l => l.Book)
            .Include(c => c.Reservations)
            .ThenInclude(r => r.Book)
            .Include(c => c.Fines)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);
    }

    public async Task<IEnumerable<Customer>> GetByNameAsync(string name)
    {
        // Search in ApplicationUser.FirstName and LastName instead of Customer
        return await _context.Customers
            .Include(c => c.User)  // ? Include ApplicationUser
            .Where(c => c.User.FirstName.Contains(name) || 
                       c.User.LastName.Contains(name) || 
                       (c.User.FirstName + " " + c.User.LastName).Contains(name))
            .Include(c => c.Loans)
            .Include(c => c.Reservations)
            .Include(c => c.Fines)
            .ToListAsync();
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        // Search in ApplicationUser.Email instead of Customer.Email
        return await _context.Customers
            .Include(c => c.User)  // ? Include ApplicationUser
            .Include(c => c.Loans)
            .Include(c => c.Reservations)
            .Include(c => c.Fines)
            .FirstOrDefaultAsync(c => c.User.Email == email);
    }

    public async Task<IEnumerable<Customer>> GetByMembershipStatusAsync(MembershipStatus status)
    {
        return await _context.Customers
            .Include(c => c.User)  // ? Include ApplicationUser
            .Where(c => c.MembershipStatus == status)
            .Include(c => c.Loans)
            .Include(c => c.Reservations)
            .Include(c => c.Fines)
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> GetByCityAsync(string city)
    {
        return await _context.Customers
            .Include(c => c.User)  // ? Include ApplicationUser
            .Where(c => c.City != null && c.City.Contains(city))
            .Include(c => c.Loans)
            .Include(c => c.Reservations)
            .Include(c => c.Fines)
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> GetCustomersWithActiveLoansAsync()
    {
        return await _context.Customers
            .Include(c => c.User)  // ? Include ApplicationUser
            .Include(c => c.Loans)
            .ThenInclude(l => l.Book)
            .Include(c => c.Reservations)
            .Include(c => c.Fines)
            .Where(c => c.Loans.Any(l => l.Status == LoanStatus.Active || l.Status == LoanStatus.Overdue))
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> GetCustomersWithUnpaidFinesAsync()
    {
        return await _context.Customers
            .Include(c => c.User)  // ? Include ApplicationUser
            .Include(c => c.Loans)
            .Include(c => c.Reservations)
            .Include(c => c.Fines)
            .Where(c => c.Fines.Any(f => f.Status == FineStatus.Pending))
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> GetCustomersWithOverdueLoansAsync()
    {
        return await _context.Customers
            .Include(c => c.User)  // ? Include ApplicationUser
            .Include(c => c.Loans)
            .ThenInclude(l => l.Book)
            .Include(c => c.Reservations)
            .Include(c => c.Fines)
            .Where(c => c.Loans.Any(l => l.Status == LoanStatus.Overdue))
            .ToListAsync();
    }

    public async Task<Customer> InsertAsync(Customer customer)
    {
        customer.CreatedAt = DateTime.UtcNow;
        customer.UpdatedAt = DateTime.UtcNow;
        
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
        
        return customer;
    }

    public async Task<Customer> UpdateAsync(Customer customer)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
        
        return customer;
    }

    public async Task<Customer?> PatchAsync(int customerId, Dictionary<string, object> updates)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer == null)
            return null;

        foreach (var update in updates)
        {
            var property = typeof(Customer).GetProperty(update.Key);
            if (property != null && property.CanWrite)
            {
                // Handle enum conversion for MembershipStatus
                if (property.PropertyType == typeof(MembershipStatus) && update.Value is int enumValue)
                {
                    property.SetValue(customer, (MembershipStatus)enumValue);
                }
                else
                {
                    property.SetValue(customer, Convert.ChangeType(update.Value, property.PropertyType));
                }
            }
        }

        customer.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        
        return customer;
    }

    public async Task<bool> DeleteAsync(int customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer == null)
            return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> ExistsAsync(int customerId)
    {
        return await _context.Customers.AnyAsync(c => c.CustomerId == customerId);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        // Check in ApplicationUser.Email instead of Customer.Email
        return await _context.Customers
            .Include(c => c.User)
            .AnyAsync(c => c.User.Email == email);
    }

    public async Task<bool> UpdateMembershipStatusAsync(int customerId, MembershipStatus status)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer == null)
            return false;

        customer.MembershipStatus = status;
        customer.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<IEnumerable<Loan>> GetCustomerLoansAsync(int customerId)
    {
        return await _context.Loans
            .Include(l => l.Book)
            .ThenInclude(b => b.BookAuthors)
            .ThenInclude(ba => ba.Author)
            .Where(l => l.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetCustomerReservationsAsync(int customerId)
    {
        return await _context.Reservations
            .Include(r => r.Book)
            .ThenInclude(b => b.BookAuthors)
            .ThenInclude(ba => ba.Author)
            .Where(r => r.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Fine>> GetCustomerFinesAsync(int customerId)
    {
        return await _context.Fines
            .Include(f => f.Loan)
            .ThenInclude(l => l!.Book)
            .Where(f => f.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
