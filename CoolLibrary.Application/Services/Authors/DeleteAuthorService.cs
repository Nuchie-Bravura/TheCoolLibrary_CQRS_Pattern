using CoolLibrary.Application.Services.Books;
using CoolLibrary.Domain.Contracts;
using Microsoft.Extensions.Logging;

namespace CoolLibrary.Application.Services.Authors;

public class DeleteAuthorService
{
    private readonly IAuthors _authorsRepository;
    private readonly IBooks _booksRepository;
    private readonly DeleteBookService _deleteBookService;
    private readonly IArchiveStorage _archiveStorage;
    private readonly ILogger<DeleteAuthorService> _logger;

    public DeleteAuthorService(
        IAuthors authorsRepository,
        IBooks booksRepository,
        DeleteBookService deleteBookService,
        IArchiveStorage archiveStorage,
        ILogger<DeleteAuthorService> logger)
    {
        _authorsRepository = authorsRepository;
        _booksRepository = booksRepository;
        _deleteBookService = deleteBookService;
        _archiveStorage = archiveStorage;
        _logger = logger;
    }

    public async Task<bool> SafeAuthorDeleteAsync(int authorId)
    {
        try
        {
            var author = await _authorsRepository.GetByIdAsync(authorId);
            if (author == null)
                return false;

            var books = await _booksRepository.GetByAuthorAsync(authorId);
            foreach (var book in books)
            {
                await _deleteBookService.SafeBookDeleteAsync(book.BookId);
            }

            if (!string.IsNullOrEmpty(author.PhotoURL))
            {
                await _archiveStorage.DeleteAsync(author.PhotoURL);
            }

            await _authorsRepository.DeleteAsync(authorId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting author with ID {AuthorId}", authorId);
            throw;
        }
    }
}
