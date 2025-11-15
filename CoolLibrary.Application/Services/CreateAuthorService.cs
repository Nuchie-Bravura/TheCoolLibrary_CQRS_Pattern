using AutoMapper;
using CoolLibrary.Application.DTO;
using CoolLibrary.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolLibrary.Application.Services
{
    public class CreateAuthorService
    {
        private readonly IAuthors _authorsRepository;
        private readonly IArchiveStorage _archiveStorage;
        private readonly IMapper _mapper;

        public CreateAuthorService(IAuthors authorsRepository, IArchiveStorage archiveStorage, IMapper mapper)
        {
            _authorsRepository = authorsRepository;
            _archiveStorage = archiveStorage;
            _mapper = mapper;
        }

        public async Task<Domain.Entities.Author> ExecuteAsync(Domain.Entities.Author author, Stream? photoStream = null, string? photoFileName = null, string? photoContentType = null)
        {
            // If photo is provided, upload it to Azure storage
            if (photoStream != null && !string.IsNullOrEmpty(photoFileName))
            {
                var photoUrl = await _archiveStorage.StoreAsync(photoStream, photoFileName, photoContentType ?? "image/jpeg");
                author.PhotoURL = photoUrl;
            }

            var createdAuthor = await _authorsRepository.InsertAsync(author);
            return createdAuthor;
        }
    }
}
