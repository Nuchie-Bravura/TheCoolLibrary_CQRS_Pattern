using AutoMapper;
using CoolLibrary.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CoolLibrary.Application.Services
{
    public class DeleteAuthorService
    {
        private readonly IAuthors _authorsRepository;
        private readonly IMapper _mapper;
        private readonly IBooks _booksRepository;
        private readonly IArchiveStorage _archiveStorage;

        public DeleteAuthorService(IAuthors authorsRepository, IMapper mapper, IBooks booksRepository, IArchiveStorage archiveStorage)
        {
            _authorsRepository = authorsRepository;
            _mapper = mapper;
            _booksRepository = booksRepository;
            _archiveStorage = archiveStorage;
        }
        public async Task<bool> SafeAuthorDeleteAsync(int authorId)
        {
            // Check if the author exists
            var author = await _authorsRepository.GetByIdAsync(authorId);
            if (author == null)
            {
                return false; // Author not found
            }
            // Retrieve all books by the author
            var books = await _booksRepository.GetByAuthorAsync(authorId);

            // Archive each book before deletion
            foreach (var book in books)
            {
                //await _archiveStorage.ArchiveBookAsync(book); not yet 
                await _booksRepository.DeleteAsync(book.BookId);
            }

            //check PhotoURL and delete photo from storage if exists
            if (!string.IsNullOrEmpty(author.PhotoURL))
            {
                await _archiveStorage.DeleteAsync(author.PhotoURL);
            }

            // Finally, delete the author
            await _authorsRepository.DeleteAsync(authorId);
            return true; // Deletion successful
        }

    }
}
