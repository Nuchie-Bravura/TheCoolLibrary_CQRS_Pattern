using CoolLibrary.Application.DTO.Author;
using MediatR;

namespace CoolLibrary.Application.UseCases.Authors.Queries.GetAllAuthors
{
    public record GetAllAuthorsQuery : IRequest<IEnumerable<AuthorDTO>>;
}
