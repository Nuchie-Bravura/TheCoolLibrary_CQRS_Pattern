using CoolLibrary.Application.DTO.Authentication;
using MediatR;

namespace CoolLibrary.Application.UseCases.Auth.Queries.Login;

/// <summary>
/// Query to authenticate a user and retrieve JWT token
/// Login is a Query because it only reads/validates data, doesn't modify state
/// </summary>
public record LoginQuery(LoginDTO LoginDto) : IRequest<AuthResponseDTO>;
