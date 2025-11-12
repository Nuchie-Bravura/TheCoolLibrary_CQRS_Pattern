using CoolLibrary.Domain.Contracts;
using CoolLibrary.Domain.Entities;
using CoolLibrary.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CoolLibrary.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing Author entities
/// </summary>
public class AuthorsRepository : IAuthors
{
    private readonly LibraryDbContext _context;

    public AuthorsRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Author>> GetAllAsync()
    {
        return await _context.Authors
            .Include(a => a.BookAuthors)
                .ThenInclude(ba => ba.Book)
            .ToListAsync();
    }

    public async Task<IEnumerable<Author>> GetByNameAsync(string name)
    {
        return await _context.Authors
            .Where(a => a.FirstName.Contains(name) || 
                       a.LastName.Contains(name) || 
                       (a.FirstName + " " + a.LastName).Contains(name))
            .Include(a => a.BookAuthors)
                .ThenInclude(ba => ba.Book)
            .ToListAsync();
    }

    public async Task<Author> InsertAsync(Author author)
    {
        author.CreatedAt = DateTime.UtcNow;
        author.UpdatedAt = DateTime.UtcNow;
        
        await _context.Authors.AddAsync(author);
        await _context.SaveChangesAsync();
        
        return author;
    }

    public async Task<Author> UpdateAsync(Author author)
    {
        author.UpdatedAt = DateTime.UtcNow;
        
        _context.Authors.Update(author);
        await _context.SaveChangesAsync();
        
        return author;
    }

    public async Task<Author?> PatchAsync(int authorId, Dictionary<string, object> updates)
    {
        var author = await _context.Authors.FindAsync(authorId);
        if (author == null)
            return null;

        foreach (var update in updates)
        {
            var property = typeof(Author).GetProperty(update.Key);
            if (property != null && property.CanWrite)
            {
                property.SetValue(author, Convert.ChangeType(update.Value, property.PropertyType));
            }
        }

        author.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        
        return author;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<Author?> GetByIdAsync(int authorId)
    {
        return await _context.Authors.FirstOrDefaultAsync(a => a.AuthorId == authorId);
    }
}
