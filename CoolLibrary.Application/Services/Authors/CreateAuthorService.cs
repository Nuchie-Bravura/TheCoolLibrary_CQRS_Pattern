using AutoMapper;
using CoolLibrary.Application.DTO.Author;
using CoolLibrary.Domain.Contracts;
using CoolLibrary.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace CoolLibrary.Application.Services.Authors;

public class CreateAuthorService
{
    private readonly IAuthors _authorsRepository;
    private readonly IArchiveStorage _archiveStorage;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateAuthorService> _logger;

    public CreateAuthorService(
        IAuthors authorsRepository,
        IArchiveStorage archiveStorage,
        IMapper mapper,
        ILogger<CreateAuthorService> logger)
    {
        _authorsRepository = authorsRepository;
        _archiveStorage = archiveStorage;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateAuthorResponseDTO> ExecuteAsync(CreateAuthorRequestDTO createAuthorRequestDTO)
    {
        try
        {
            // Map DTO → entidad
            var authorEntity = _mapper.Map<Author>(createAuthorRequestDTO);


            // upload file if exits
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

            // Map entidad → DTO 
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
