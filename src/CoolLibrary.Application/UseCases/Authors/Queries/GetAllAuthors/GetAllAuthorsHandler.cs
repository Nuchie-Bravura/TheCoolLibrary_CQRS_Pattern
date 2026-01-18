using AutoMapper;
using CoolLibrary.Application.DTO.Author;
using CoolLibrary.Application.Services.Cache;
using CoolLibrary.Domain.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoolLibrary.Application.UseCases.Authors.Queries.GetAllAuthors
{
    public class GetAllAuthorsHandler : IRequestHandler<GetAllAuthorsQuery, IEnumerable<AuthorDTO>>
    {
        private readonly IAuthors _authorsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllAuthorsHandler> _logger;
        private readonly ICacheService _cacheService;

        public GetAllAuthorsHandler(
            IAuthors authorsRepository,
            IMapper mapper,
            ILogger<GetAllAuthorsHandler> logger,
            ICacheService cacheService)
        {
            _authorsRepository = authorsRepository;
            _mapper = mapper;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<AuthorDTO>> Handle(GetAllAuthorsQuery request, CancellationToken cancellationToken)
        {
            const string cacheKey = "authors:all";

            try
            {
                return await _cacheService.GetOrSetAsync(cacheKey, async () =>
                {
                    _logger.LogInformation("Cache miss for {CacheKey}, fetching from database", cacheKey);
                    var authors = await _authorsRepository.GetAllAsync();
                    return _mapper.Map<IEnumerable<AuthorDTO>>(authors);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all authors");
                throw;
            }
        }
    }
}
