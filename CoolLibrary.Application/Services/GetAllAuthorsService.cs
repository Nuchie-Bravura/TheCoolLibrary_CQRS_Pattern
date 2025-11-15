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
    public class GetAllAuthorsService
    {

        private readonly IAuthors _authorsRepository;
        private readonly IMapper _mapper;

        public GetAllAuthorsService(IAuthors authorsRepository, IMapper mapper)
        {
            _authorsRepository = authorsRepository;
            _mapper = mapper;

        }

        public async Task<IEnumerable<AuthorDTO>> ExecuteAsync()
        {
            var authors = await _authorsRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<AuthorDTO>>(authors);
        }
    }
}
