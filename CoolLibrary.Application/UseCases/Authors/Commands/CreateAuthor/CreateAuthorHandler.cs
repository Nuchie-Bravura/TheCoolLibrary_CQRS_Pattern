using AutoMapper;
using CoolLibrary.Application.DTO.Author;
using CoolLibrary.Domain.Contracts;
using CoolLibrary.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoolLibrary.Application.UseCases.Authors.Commands.CreateAuthor
{
    public class CreateAuthorHandler : IRequestHandler<CreateAuthorCommand, CreateAuthorResponseDTO>
    {
        private readonly IAuthors _authorsRepository;
        private readonly IArchiveStorage _archiveStorage;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateAuthorHandler> _logger;

        public CreateAuthorHandler(
            IAuthors authorsRepository,
            IArchiveStorage archiveStorage,
            IMapper mapper,
            ILogger<CreateAuthorHandler> logger)
        {
            _authorsRepository = authorsRepository;
            _archiveStorage = archiveStorage;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CreateAuthorResponseDTO> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
        {
            var createAuthorRequestDTO = request.CreateAuthorRequestDto;
            try
            {
                // Map DTO → entity
                var authorEntity = _mapper.Map<Author>(createAuthorRequestDTO);

                // upload file if exists
                if (createAuthorRequestDTO.PhotoFile != null)
                {
                    using var stream = createAuthorRequestDTO.PhotoFile.OpenReadStream();
                    var photoUrl = await _archiveStorage.StoreAsync(
                        stream,
                        createAuthorRequestDTO.PhotoFile.FileName,
                        createAuthorRequestDTO.PhotoFile.ContentType ?? "image/jpeg");
                    authorEntity.PhotoURL = photoUrl;
                }

                // Insert DB
                var createdAuthor = await _authorsRepository.InsertAsync(authorEntity);

                // Map entity → DTO
                var responseDto = _mapper.Map<CreateAuthorResponseDTO>(createdAuthor);
                return responseDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new author with name {FullName}", createAuthorRequestDTO.FullName);
                throw;
            }
        }
    }
}
