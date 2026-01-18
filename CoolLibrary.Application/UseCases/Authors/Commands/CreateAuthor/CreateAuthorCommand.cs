using CoolLibrary.Application.DTO.Author;
using MediatR;

namespace CoolLibrary.Application.UseCases.Authors.Commands.CreateAuthor
{
    public record CreateAuthorCommand(CreateAuthorRequestDTO CreateAuthorRequestDto) : IRequest<CreateAuthorResponseDTO>;
}
