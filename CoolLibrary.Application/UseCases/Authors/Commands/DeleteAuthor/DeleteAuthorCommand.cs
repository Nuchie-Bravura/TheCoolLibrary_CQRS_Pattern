using MediatR;

namespace CoolLibrary.Application.UseCases.Authors.Commands.DeleteAuthor
{
    public record DeleteAuthorCommand(int AuthorId) : IRequest<bool>;
}
