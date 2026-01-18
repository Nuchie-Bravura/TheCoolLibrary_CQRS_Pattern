using CoolLibrary.Application.DTO.Book;
using MediatR;

namespace CoolLibrary.Application.UseCases.Books.Queries.GetAllBooks
{
    public record GetAllBooksQuery : IRequest<IEnumerable<BookDTO>>;
}
