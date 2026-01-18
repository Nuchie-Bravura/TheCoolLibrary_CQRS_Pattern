using AutoMapper;
using CoolLibrary.Application.DTO.Book;
using CoolLibrary.Application.Services.Cache;
using CoolLibrary.Domain.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoolLibrary.Application.UseCases.Books.Queries.GetAllBooks
{
    public class GetAllBooksHandler : IRequestHandler<GetAllBooksQuery, IEnumerable<BookDTO>>
    {
        private readonly IBooks _booksRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllBooksHandler> _logger;
        private readonly ICacheService _cacheService;

        public GetAllBooksHandler(
            IBooks booksRepository,
            IMapper mapper,
            ILogger<GetAllBooksHandler> logger,
            ICacheService cacheService)
        {
            _booksRepository = booksRepository;
            _mapper = mapper;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<BookDTO>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
        {
            const string cacheKey = "books:all";

            try
            {
                return await _cacheService.GetOrSetAsync(cacheKey, async () =>
                {
                    _logger.LogInformation("Cache miss for {CacheKey}, fetching from database", cacheKey);
                    var books = await _booksRepository.GetAllAsync();
                    return _mapper.Map<IEnumerable<BookDTO>>(books);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all books: {Message}", ex.Message);
                throw;
            }
        }
    }
}
