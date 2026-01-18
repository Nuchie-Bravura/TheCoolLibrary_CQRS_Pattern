using MediatR;

namespace CoolLibrary.Application.UseCases.Books.Commands.DeleteBook
{
    public record DeleteBookCommand(int BookId) : IRequest<bool>;
}
