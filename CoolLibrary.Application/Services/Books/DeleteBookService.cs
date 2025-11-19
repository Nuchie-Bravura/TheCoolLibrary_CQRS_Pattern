using CoolLibrary.Application.Services.Authors;
using CoolLibrary.Domain.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolLibrary.Application.Services.Books
{

    public class DeleteBookService
    {

        private readonly IBooks _booksRepository;
        private readonly IArchiveStorage _archiveStorage;
        private readonly ILogger<DeleteBookService> _logger;

        public DeleteBookService(
            IBooks booksRepository,
            IArchiveStorage archiveStorage,
            ILogger<DeleteBookService> logger)
        {
            _booksRepository = booksRepository;
            _archiveStorage = archiveStorage;
            _logger = logger;
        }

        public async Task<bool> SafeBookDeleteAsync(int bookId)
        {
            try
            {
                var book = await _booksRepository.GetByIdAsync(bookId);
                if (book == null)
                    return false;
                if (!string.IsNullOrEmpty(book.CoverPhotoURL))
                {
                    await _archiveStorage.DeleteAsync(book.CoverPhotoURL);
                }
                await _booksRepository.DeleteAsync(bookId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting book with ID {BookId}", bookId);
                throw;
            }
        }

    }
}
