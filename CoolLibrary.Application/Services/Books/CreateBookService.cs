using AutoMapper;
using CoolLibrary.Application.DTO.Author;
using CoolLibrary.Application.DTO.Book;
using CoolLibrary.Domain.Contracts;
using CoolLibrary.Domain.Entities;
using Microsoft.Extensions.Logging;


namespace CoolLibrary.Application.Services.Books
{
    public class CreateBookService
    {
        private readonly IBooks _booksRepository;
        private readonly IAuthors _authorsRepository;
        private readonly IArchiveStorage _archiveStorage;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateBookService> _logger;
        public CreateBookService(IBooks booksRepository, IAuthors authorsRepository, IArchiveStorage archiveStorage, IMapper mapper, ILogger<CreateBookService> logger)
        {
            _booksRepository = booksRepository;
            _authorsRepository = authorsRepository;
            _archiveStorage = archiveStorage;
            _mapper = mapper;
            _logger = logger;
        }


        public async Task<CreateBookResponseDTO> ExecuteAsync(CreateBookRequestDTO createBookRequestDTO)
        {
            try
            {
                // Validate copy counts
                if (createBookRequestDTO.AvailableCopies < 0 || createBookRequestDTO.TotalCopies < 0)
                {
                    throw new ArgumentException("Available copies and total copies must be greater than or equal to 0.");
                }

                if (createBookRequestDTO.AvailableCopies > createBookRequestDTO.TotalCopies)
                {
                    throw new ArgumentException("Available copies cannot exceed total copies.");
                }

                // Validate authors exist
                if (createBookRequestDTO.Authors != null && createBookRequestDTO.Authors.Any())
                {
                    foreach (var authorId in createBookRequestDTO.Authors)
                    {
                        var existingAuthor = await _authorsRepository.GetByIdAsync(authorId);
                        if (existingAuthor == null)
                        {
                            throw new ArgumentException($"Author with ID {authorId} does not exist.");
                        }
                    }
                }
                else
                { 
                    throw new ArgumentException("At least one author must be specified.");
                }

                // Map DTO to entity (AutoMapper now handles BookAuthors creation)
                var bookEntity = _mapper.Map<Domain.Entities.Book>(createBookRequestDTO);

                // If cover image is provided, upload it to Azure storage
                if (createBookRequestDTO.PhotoFile != null)
                {
                    using var stream = createBookRequestDTO.PhotoFile.OpenReadStream();
                    var photoUrl = await _archiveStorage.StoreAsync(
                        stream,
                        createBookRequestDTO.PhotoFile.FileName,
                        createBookRequestDTO.PhotoFile.ContentType ?? "image/jpeg");
                    bookEntity.CoverPhotoURL = photoUrl;
                }

                var createdBook = await _booksRepository.InsertAsync(bookEntity);

                return  _mapper.Map<CreateBookResponseDTO>(createdBook);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new book");
                throw new ApplicationException($"An error occurred while creating the book: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }
       
    }
}
