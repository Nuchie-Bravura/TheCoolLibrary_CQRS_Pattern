using AutoMapper;
using CoolLibrary.Application.DTO.Author;
using CoolLibrary.Application.DTO.Book;
using CoolLibrary.Application.Services.Cache;
using CoolLibrary.Domain.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolLibrary.Application.Services.Authors
{
    public class GetAllAuthorsService
    {

        private readonly IAuthors _authorsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllAuthorsService> _logger;
        private readonly IBooks _booksRepository;
        private readonly ICacheService _cacheService;

        public GetAllAuthorsService(
            IAuthors authorsRepository, 
            IMapper mapper, 
            ILogger<GetAllAuthorsService> logger, 
            IBooks booksRepository,
            ICacheService cacheService)
        {
            _authorsRepository = authorsRepository;
            _mapper = mapper;
            _logger = logger;
            _booksRepository = booksRepository;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<AuthorDTO>> ExecuteAsync()
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
