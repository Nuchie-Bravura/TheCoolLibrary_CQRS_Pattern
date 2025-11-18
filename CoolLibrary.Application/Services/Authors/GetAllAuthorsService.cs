using AutoMapper;
using CoolLibrary.Application.DTO.Author;
using CoolLibrary.Application.DTO.Book;
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

        public GetAllAuthorsService(IAuthors authorsRepository, IMapper mapper, ILogger<GetAllAuthorsService> logger, IBooks booksRepository)
        {
            _authorsRepository = authorsRepository;
            _mapper = mapper;
            _logger = logger;
            _booksRepository = booksRepository;
        }

        public async Task<IEnumerable<AuthorDTO>> ExecuteAsync()
        {
            try
            {
                var authors = await _authorsRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<AuthorDTO>>(authors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all authors");
                throw;
            }
        }
    }
}
