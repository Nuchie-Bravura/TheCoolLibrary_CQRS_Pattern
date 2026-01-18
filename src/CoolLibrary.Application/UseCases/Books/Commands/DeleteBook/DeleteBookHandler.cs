using CoolLibrary.Domain.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoolLibrary.Application.UseCases.Books.Commands.DeleteBook
{
    public class DeleteBookHandler : IRequestHandler<DeleteBookCommand, bool>
    {
        private readonly IBooks _booksRepository;
        private readonly IArchiveStorage _archiveStorage;
        private readonly ILogger<DeleteBookHandler> _logger;

        public DeleteBookHandler(
            IBooks booksRepository,
            IArchiveStorage archiveStorage,
            ILogger<DeleteBookHandler> logger)
        {
            _booksRepository = booksRepository;
            _archiveStorage = archiveStorage;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            var bookId = request.BookId;
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
