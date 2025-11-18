using AutoMapper;
using CoolLibrary.Application.DTO.Author;
using CoolLibrary.Application.DTO.Book;
using CoolLibrary.Application.DTO.Customer;
using CoolLibrary.Application.DTO.HATEOAS;
using CoolLibrary.Domain.Entities;
using CoolLibrary.Domain.Enums;

namespace CoolLibrary.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Author Mappings
        CreateMap<Author, AuthorDTO>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()))
            .ForMember(dest => dest.Books, opt => opt.MapFrom(src => src.BookAuthors.Select(ba => ba.Book)));

        CreateMap<Book, AuthorBookDTO>();

        // Book Mappings
        CreateMap<Book, BookDTO>()
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.IsAvailable));

        // Customer Mappings (Output)
        // NOTE: FirstName, LastName, and Email are now in ApplicationUser (customer.User)
        CreateMap<Customer, CustomerDTO>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))  // Uses Customer.FullName property which delegates to User
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))  // Uses Customer.Email property which delegates to User
            .ForMember(dest => dest.MembershipStatus, opt => opt.MapFrom(src => src.MembershipStatus.ToString()));

        // Customer Mappings (Input - Create)
        // NOTE: FirstName, LastName, Email now handled separately when creating ApplicationUser
        CreateMap<CreateCustomerDTO, Customer>()
            .ForMember(dest => dest.MembershipDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.MembershipStatus, opt => opt.MapFrom(src => MembershipStatus.Active))
            .ForMember(dest => dest.MaxBooksAllowed, opt => opt.MapFrom(src => src.MaxBooksAllowed ?? 5))
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())  // Set separately when creating ApplicationUser
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Loans, opt => opt.Ignore())
            .ForMember(dest => dest.Reservations, opt => opt.Ignore())
            .ForMember(dest => dest.Fines, opt => opt.Ignore());

        // Author Mappings (Input - Create)

        CreateMap<CreateAuthorRequestDTO, Author>()
            .ForMember(dest => dest.FirstName,
                opt => opt.MapFrom(src => src.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()))
            .ForMember(dest => dest.LastName,
                opt => opt.MapFrom(src =>
                    string.Join(' ',
                        src.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                           .Skip(1))))
            .ForMember(dest => dest.PhotoURL, opt => opt.Ignore())
            .ForMember(dest => dest.Biography, opt => opt.MapFrom(src => src.Biography))
            .ForMember(dest => dest.Nationality, opt => opt.MapFrom(src => src.Nationality))
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.BookAuthors, opt => opt.Ignore());


        // Author Mappings (Output - Update)  HateOAS Implementation

        CreateMap<Author, CreateAuthorResponseDTO>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.Nationality, opt => opt.MapFrom(src => src.Nationality))
            .ForMember(dest => dest.Links, opt => opt.MapFrom(src => new List<LinkDTO>
            {
                new LinkDTO
                {
                    Rel = "self",
                    Href = $"/api/authors/{src.AuthorId}",
                    Method = "GET"
                }
            }));

        //Book Mappings (Input - Create)

        CreateMap<CreateBookRequestDTO, Book>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ISBN, opt => opt.MapFrom(src => src.ISBN))
            .ForMember(dest => dest.PublicationDate, opt => opt.MapFrom(src => src.PublicationDate))
            .ForMember(dest => dest.Publisher, opt => opt.MapFrom(src => src.Publisher))
            .ForMember(dest => dest.PageCount, opt => opt.MapFrom(src => src.PageCount))
            .ForMember(dest => dest.TotalCopies, opt => opt.MapFrom(src => src.TotalCopies))
            .ForMember(dest => dest.AvailableCopies, opt => opt.MapFrom(src => src.AvailableCopies))
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Language) ? src.Language : "English"))
            .ForMember(dest => dest.CoverPhotoURL, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
     
            .ForMember(dest => dest.BookAuthors, opt => opt.MapFrom((src, dest, destMember, context) =>
                src.Authors.Select((authorId, index) => new BookAuthor
                {
                    AuthorId = authorId,
                    AuthorOrder = index + 1
                }).ToList()))

            .ForMember(dest => dest.BookGenres, opt => opt.Ignore())
            .ForMember(dest => dest.Loans, opt => opt.Ignore())
            .ForMember(dest => dest.Reservations, opt => opt.Ignore());


        // Book Mappings (Output - Create)  HateOAS Implementation
        CreateMap<Book, CreateBookResponseDTO>()
            .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.BookId))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.BookAuthors.Select(ba => ba.Author)))
            .ForMember(dest => dest.Links, opt => opt.MapFrom(src => new List<LinkDTO>
            {
                new LinkDTO
                {
                    Rel = "self",
                    Href = $"/api/v1.0/books/{src.BookId}",
                    Method = "GET"
                }
            }));


        // Customer Mappings (Input - Update)
        CreateMap<UpdateCustomerDTO, Customer>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
        CreateMap<Customer, UpdateCustomerDTO>();
    }
}