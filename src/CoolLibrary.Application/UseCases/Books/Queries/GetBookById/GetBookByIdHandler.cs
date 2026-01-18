using AutoMapper;
using CoolLibrary.Application.DTO.Book;
using CoolLibrary.Application.Services.Cache;
using CoolLibrary.Domain.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoolLibrary.Application.UseCases.Books.Queries.GetBookById
{
    public class GetBookByIdHandler : IRequestHandler<GetBookByIdQuery, BookDTO?>
    {
        private readonly IBooks _booksRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetBookByIdHandler> _logger;
        private readonly ICacheService _cacheService;

        public GetBookByIdHandler(
            IBooks booksRepository,
            IMapper mapper,
            ILogger<GetBookByIdHandler> logger,
            ICacheService cacheService)
        {
            _booksRepository = booksRepository;
            _mapper = mapper;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<BookDTO?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = $"books:{request.BookId}";

            try
            {
                return await _cacheService.GetOrSetAsync(cacheKey, async () =>
                {
                    _logger.LogInformation("Cache miss for {CacheKey}, fetching from database", cacheKey);
                    var book = await _booksRepository.GetByIdAsync(request.BookId);
                    if (book == null)
                    {
                        return null;
                    }
                    return _mapper.Map<BookDTO>(book);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving book with ID {BookId}", request.BookId);
                throw;
            }
        }
    }
}
