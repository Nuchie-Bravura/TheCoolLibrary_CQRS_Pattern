using CoolLibrary.Application.DTO.LoansAndReservations;
using CoolLibrary.Domain.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoolLibrary.Application.UseCases.Loans.Queries.GetBookAvailability
{
    public class GetBookAvailabilityHandler : IRequestHandler<GetBookAvailabilityQuery, AvailabilityDTO?>
    {
        private readonly IBooks _booksRepository;
        private readonly ILogger<GetBookAvailabilityHandler> _logger;

        public GetBookAvailabilityHandler(IBooks booksRepository, ILogger<GetBookAvailabilityHandler> logger)
        {
            _booksRepository = booksRepository;
            _logger = logger;
        }

        public async Task<AvailabilityDTO?> Handle(GetBookAvailabilityQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var book = await _booksRepository.GetByIdAsync(request.BookId);
                if (book == null)
                {
                    return null;
                }

                return new AvailabilityDTO
                {
                    BookId = book.BookId,
                    AvailableCopies = book.AvailableCopies,
                    IsAvailable = book.AvailableCopies > 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking availability for Book {BookId}", request.BookId);
                throw;
            }
        }
    }
}
