using CoolLibrary.Application.DTO.Book;
using MediatR;

namespace CoolLibrary.Application.UseCases.Books.Commands.CreateBook
{
    public record CreateBookCommand(CreateBookRequestDTO CreateBookRequestDTO) : IRequest<CreateBookResponseDTO>;
}
