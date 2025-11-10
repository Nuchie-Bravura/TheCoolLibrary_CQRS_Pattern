using Asp.Versioning;
using CoolLibrary.Domain.Entities;
using CoolLibrary.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoolLibrary.API.Controllers;

/// <summary>
/// Diagnostic controller for troubleshooting
/// ⚠️ REMOVE IN PRODUCTION!
/// </summary>
[ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
[Authorize(Roles = "Admin")]
public class DiagnosticsController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly LibraryDbContext _dbContext;
    private readonly ILogger<DiagnosticsController> _logger;

    public DiagnosticsController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        LibraryDbContext dbContext,
        ILogger<DiagnosticsController> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Check database status and seed if needed
    /// </summary>
    [HttpGet("check-and-seed")]
    public async Task<IActionResult> CheckAndSeed()
    {
        var result = new
        {
            Database = new
            {
                CanConnect = await _dbContext.Database.CanConnectAsync(),
                ConnectionString = _dbContext.Database.GetConnectionString()
            },
            Roles = new
            {
                Count = await _roleManager.Roles.CountAsync(),
                List = await _roleManager.Roles.Select(r => r.Name).ToListAsync()
            },
            Users = new
            {
                Count = await _userManager.Users.CountAsync(),
                List = await _userManager.Users.Select(u => new
                {
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.EmailConfirmed
                }).ToListAsync()
            },
            AdminUser = new
            {
                Exists = await _userManager.FindByEmailAsync("admin@fake.com") != null,
                Details = (await _userManager.FindByEmailAsync("admin@fake.com")) != null
                    ? new
                    {
                        Email = "admin@fake.com",
                        HasAdminRole = await _userManager.IsInRoleAsync(
                            await _userManager.FindByEmailAsync("admin@fake.com"), "Admin")
                    }
                    : null
            }
        };

        return Ok(result);
    }

    /// <summary>
    /// Force re-seed the database
    /// </summary>
    [HttpPost("force-seed")]
    public async Task<IActionResult> ForceSeed()
    {
        try
        {
            _logger.LogInformation("🔄 Force seeding triggered via API...");
            
            await DatabaseSeeder.SeedAsync(HttpContext.RequestServices);
            
            return Ok(new { message = "✅ Database seeded successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Force seed failed");
            return StatusCode(500, new
            {
                message = "❌ Seeding failed",
                error = ex.Message,
                stackTrace = ex.StackTrace
            });
        }
    }

    /// <summary>
    /// Check if we can query AspNetUsers table
    /// </summary>
    [HttpGet("check-identity-tables")]
    public async Task<IActionResult> CheckIdentityTables()
    {
        try
        {
            var users = await _dbContext.Users.ToListAsync();
            var roles = await _dbContext.Roles.ToListAsync();
            var userRoles = await _dbContext.UserRoles.ToListAsync();

            return Ok(new
            {
                AspNetUsers = new
                {
                    Count = users.Count,
                    Users = users.Select(u => new
                    {
                        u.Id,
                        u.Email,
                        u.UserName,
                        u.FirstName,
                        u.LastName,
                        u.EmailConfirmed
                    })
                },
                AspNetRoles = new
                {
                    Count = roles.Count,
                    Roles = roles.Select(r => new { r.Id, r.Name })
                },
                AspNetUserRoles = new
                {
                    Count = userRoles.Count,
                    UserRoles = userRoles.Select(ur => new { ur.UserId, ur.RoleId })
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "❌ Error querying Identity tables",
                error = ex.Message,
                innerError = ex.InnerException?.Message
            });
        }
    }

    /// <summary>
    /// Manually create admin user (for testing)
    /// </summary>
    [HttpPost("create-admin-now")]
    public async Task<IActionResult> CreateAdminNow()
    {
        try
        {
            const string adminEmail = "admin@fake.com";
            const string adminPassword = "admin$123!";

            // Check if already exists
            var existingUser = await _userManager.FindByEmailAsync(adminEmail);
            if (existingUser != null)
            {
                return BadRequest(new { message = "❌ Admin user already exists" });
            }

            // Create admin role if it doesn't exist
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole("Admin"));
                if (!roleResult.Succeeded)
                {
                    return StatusCode(500, new
                    {
                        message = "❌ Failed to create Admin role",
                        errors = roleResult.Errors.Select(e => e.Description)
                    });
                }
            }

            // Create User role if it doesn't exist
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Create admin user
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(adminUser, adminPassword);
            
            if (!createResult.Succeeded)
            {
                return StatusCode(500, new
                {
                    message = "❌ Failed to create admin user",
                    errors = createResult.Errors.Select(e => e.Description)
                });
            }

            // Assign Admin role
            var addRoleResult = await _userManager.AddToRoleAsync(adminUser, "Admin");
            
            if (!addRoleResult.Succeeded)
            {
                return StatusCode(500, new
                {
                    message = "✅ User created but ❌ Failed to assign Admin role",
                    errors = addRoleResult.Errors.Select(e => e.Description)
                });
            }

            return Ok(new
            {
                message = "✅ Admin user created successfully!",
                email = adminEmail,
                password = adminPassword,
                warning = "⚠️ Change these credentials in production!"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "❌ Error creating admin user",
                error = ex.Message,
                innerError = ex.InnerException?.Message,
                stackTrace = ex.StackTrace
            });
        }
    }
}
