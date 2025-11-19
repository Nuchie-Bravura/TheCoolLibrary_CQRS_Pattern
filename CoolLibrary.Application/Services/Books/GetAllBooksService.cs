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

namespace CoolLibrary.Application.Services.Books
{
    public class GetAllBooksService
    {
        private readonly IBooks _booksRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllBooksService> _logger;
        public GetAllBooksService(IBooks booksRepository, IMapper mapper, ILogger<GetAllBooksService> logger)
        {
            _booksRepository = booksRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<BookDTO>> ExecuteAsync()
        {
            try
            {
                var books = await _booksRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<BookDTO>>(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all books"+ex.Message);
                throw;
            }
        }

        public async Task<BookDTO?> GetByIdAsync(int BookId)
        {
            try
            {
                var book = await _booksRepository.GetByIdAsync(BookId);
                if (book == null)
                {
                    return null;
                }
                return _mapper.Map<BookDTO>(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving book with ID {BookId}", BookId);
                throw;
            }
        }
    }
}   


