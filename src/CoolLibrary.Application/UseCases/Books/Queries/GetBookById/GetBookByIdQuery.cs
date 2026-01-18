using CoolLibrary.Application.DTO.Book;
using MediatR;

namespace CoolLibrary.Application.UseCases.Books.Queries.GetBookById
{
    public record GetBookByIdQuery(int BookId) : IRequest<BookDTO?>;
}
