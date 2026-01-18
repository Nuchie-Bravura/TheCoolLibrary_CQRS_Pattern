using CoolLibrary.Application.DTO.Authentication;
using CoolLibrary.Domain.Contracts;
using CoolLibrary.Domain.Entities;
using CoolLibrary.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CoolLibrary.Application.UseCases.Auth.Commands.Register;

public record RegisterCommand(RegisterDTO RegisterDto) : IRequest<RegistrationResultDTO>;

public record RegistrationResultDTO(string Message, string Email, int CustomerId, string Role);

public class RegisterHandler : IRequestHandler<RegisterCommand, RegistrationResultDTO>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICustomers _customerRepository;
    private readonly ILogger<RegisterHandler> _logger;

    public RegisterHandler(
        UserManager<ApplicationUser> userManager,
        ICustomers customerRepository,
        ILogger<RegisterHandler> logger)
    {
        _userManager = userManager;
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<RegistrationResultDTO> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var registerDto = request.RegisterDto;

        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            throw new ArgumentException("User with this email already exists");
        }

        var newUser = new ApplicationUser
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            EmailConfirmed = true,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(newUser, registerDto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"User registration failed: {errors}");
        }

        await _userManager.AddToRoleAsync(newUser, "User");

        var customer = new Customer
        {
            UserId = newUser.Id,
            Phone = registerDto.Phone,
            Address = registerDto.Address,
            City = registerDto.City,
            PostalCode = registerDto.PostalCode,
            MembershipDate = DateTime.UtcNow,
            MembershipStatus = MembershipStatus.Active,
            MaxBooksAllowed = 5,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _customerRepository.InsertAsync(customer);
        await _customerRepository.SaveChangesAsync();

        _logger.LogInformation("✅ New user and customer created: {Email} (Customer ID: {CustomerId})",
            registerDto.Email, customer.CustomerId);

        return new RegistrationResultDTO(
            "User and customer profile created successfully",
            newUser.Email!,
            customer.CustomerId,
            "User"
        );
    }
}
