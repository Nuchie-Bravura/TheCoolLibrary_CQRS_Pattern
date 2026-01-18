using CoolLibrary.Application.UseCases.Books.Commands.DeleteBook;
using CoolLibrary.Domain.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoolLibrary.Application.UseCases.Authors.Commands.DeleteAuthor
{
    public class DeleteAuthorHandler : IRequestHandler<DeleteAuthorCommand, bool>
    {
        private readonly IAuthors _authorsRepository;
        private readonly IBooks _booksRepository;
        private readonly IMediator _mediator;
        private readonly IArchiveStorage _archiveStorage;
        private readonly ILogger<DeleteAuthorHandler> _logger;

        public DeleteAuthorHandler(
            IAuthors authorsRepository,
            IBooks booksRepository,
            IMediator mediator,
            IArchiveStorage archiveStorage,
            ILogger<DeleteAuthorHandler> logger)
        {
            _authorsRepository = authorsRepository;
            _booksRepository = booksRepository;
            _mediator = mediator;
            _archiveStorage = archiveStorage;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
        {
            var authorId = request.AuthorId;
            try
            {
                var author = await _authorsRepository.GetByIdAsync(authorId);
                if (author == null)
                    return false;

                var books = await _booksRepository.GetByAuthorAsync(authorId);
                foreach (var book in books)
                {
                    await _mediator.Send(new DeleteBookCommand(book.BookId));
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
}
