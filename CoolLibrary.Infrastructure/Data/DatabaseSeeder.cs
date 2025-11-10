using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CoolLibrary.Domain.Entities;
using CoolLibrary.Domain.Enums;

namespace CoolLibrary.Infrastructure.Data;

/// <summary>
/// Database seeding service for initial data population
/// Responsible for creating roles, default admin user, and sample customers
/// </summary>
public class DatabaseSeeder
{
    /// <summary>
    /// Seeds the database with essential roles and admin user
    /// This method is called from the API startup to ensure required data exists
    /// </summary>
    /// <param name="serviceProvider">Service provider to resolve dependencies</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<DatabaseSeeder>>();
        
        try
        {
            logger.LogInformation("🌱 Starting database seeding...");
            
            // Resolve scoped services (RoleManager, UserManager)
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = serviceProvider.GetRequiredService<LibraryDbContext>();

            // STEP 1: Seed Roles
            logger.LogInformation("📌 Step 1: Seeding roles...");
            await SeedRolesAsync(roleManager, logger);

            // STEP 2: Seed Admin User
            logger.LogInformation("📌 Step 2: Seeding admin user...");
            await SeedAdminUserAsync(userManager, logger);

            // STEP 3: Seed Sample Customers (with ApplicationUser relationship)
            logger.LogInformation("📌 Step 3: Seeding sample customers...");
            await SeedSampleCustomersAsync(userManager, dbContext, logger);
            
            logger.LogInformation("✅ Database seeding completed successfully!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ CRITICAL ERROR: Database seeding failed!");
            logger.LogError("Error Message: {Message}", ex.Message);
            logger.LogError("Stack Trace: {StackTrace}", ex.StackTrace);
            
            // Re-throw to ensure startup fails if seeding fails
            throw;
        }
    }

    /// <summary>
    /// Creates the default roles: Admin and User
    /// </summary>
    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
    {
        // Define the roles we want in our system
        string[] roleNames = { "Admin", "User" };

        foreach (var roleName in roleNames)
        {
            try
            {
                // Check if the role already exists
                var roleExists = await roleManager.RoleExistsAsync(roleName);

                if (!roleExists)
                {
                    logger.LogInformation("Creating role: {RoleName}...", roleName);
                    
                    // Create the role if it doesn't exist
                    var result = await roleManager.CreateAsync(new IdentityRole(roleName));

                    if (result.Succeeded)
                    {
                        logger.LogInformation("✅ Role '{RoleName}' created successfully", roleName);
                    }
                    else
                    {
                        logger.LogError("❌ Failed to create role '{RoleName}': {Errors}",
                            roleName,
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    logger.LogInformation("ℹ️  Role '{RoleName}' already exists", roleName);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Error creating role '{RoleName}'", roleName);
                throw;
            }
        }
    }

    /// <summary>
    /// Creates a default admin user for testing and initial setup
    /// Email: admin@fake.com
    /// Password: admin$123!
    /// ⚠️ These credentials should be changed in production!
    /// </summary>
    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, ILogger logger)
    {
        try
        {
            // Admin user credentials (these should be changed in production!)
            const string adminEmail = "admin@fake.com";
            const string adminPassword = "Aadmin$123!4444";

            logger.LogInformation("Checking if admin user exists: {Email}...", adminEmail);
            
            // Check if admin user already exists
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                logger.LogInformation("Admin user does not exist. Creating...");
                
                // Create the admin user
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,  // Skip email confirmation for seeded admin
                    FirstName = "Admin",
                    LastName = "User",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Create user in database with password
                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    logger.LogInformation("✅ Admin user created successfully in AspNetUsers table");
                    
                    // Assign Admin role to the user
                    var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                    
                    if (roleResult.Succeeded)
                    {
                        logger.LogInformation("✅ Admin role assigned successfully");
                    }
                    else
                    {
                        logger.LogError("❌ Failed to assign Admin role: {Errors}",
                            string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    }

                    logger.LogWarning("⚠️  DEFAULT ADMIN CREDENTIALS:");
                    logger.LogWarning("   Email: {Email}", adminEmail);
                    logger.LogWarning("   Password: {Password}", adminPassword);
                    logger.LogWarning("   ⚠️  CHANGE THESE IN PRODUCTION!");
                }
                else
                {
                    logger.LogError("❌ Failed to create admin user: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                logger.LogInformation("ℹ️  Admin user already exists: {Email}", adminEmail);

                // Ensure admin user has Admin role (in case it was removed)
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    logger.LogInformation("Admin user exists but doesn't have Admin role. Assigning...");
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    logger.LogInformation("✅ Admin role assigned to existing user: {Email}", adminEmail);
                }
                else
                {
                    logger.LogInformation("ℹ️  Admin user already has Admin role");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Error in SeedAdminUserAsync");
            throw;
        }
    }

    /// <summary>
    /// Seeds sample customers with their ApplicationUser accounts
    /// Creates 2 sample customers for testing purposes
    /// </summary>
    private static async Task SeedSampleCustomersAsync(
        UserManager<ApplicationUser> userManager,
        LibraryDbContext dbContext,
        ILogger logger)
    {
        // Sample customer 1
        await CreateCustomerIfNotExistsAsync(
            userManager,
            dbContext,
            logger,
            email: "john.smith@email.com",
            password: "Customer$123!",
            firstName: "John",
            lastName: "Smith",
            phone: "+1-555-0101",
            address: "123 Main Street",
            city: "New York",
            postalCode: "10001",
            maxBooksAllowed: 5
        );

        // Sample customer 2
        await CreateCustomerIfNotExistsAsync(
            userManager,
            dbContext,
            logger,
            email: "emily.johnson@email.com",
            password: "Customer$123!",
            firstName: "Emily",
            lastName: "Johnson",
            phone: "+1-555-0102",
            address: "456 Oak Avenue",
            city: "Los Angeles",
            postalCode: "90210",
            maxBooksAllowed: 3
        );
    }

    /// <summary>
    /// Helper method to create a customer with an associated ApplicationUser
    /// </summary>
    private static async Task CreateCustomerIfNotExistsAsync(
        UserManager<ApplicationUser> userManager,
        LibraryDbContext dbContext,
        ILogger logger,
        string email,
        string password,
        string firstName,
        string lastName,
        string? phone,
        string? address,
        string? city,
        string? postalCode,
        int maxBooksAllowed)
    {
        // Check if user already exists
        var existingUser = await userManager.FindByEmailAsync(email);

        if (existingUser == null)
        {
            // Create ApplicationUser
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FirstName = firstName,
                LastName = lastName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Assign User role
                await userManager.AddToRoleAsync(user, "User");

                // Create Customer profile
                var customer = new Customer
                {
                    UserId = user.Id,  // Link to ApplicationUser
                    Phone = phone,
                    Address = address,
                    City = city,
                    PostalCode = postalCode,
                    MembershipDate = DateTime.UtcNow.AddMonths(-6), // Member for 6 months
                    MembershipStatus = MembershipStatus.Active,
                    MaxBooksAllowed = maxBooksAllowed,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                dbContext.Customers.Add(customer);
                await dbContext.SaveChangesAsync();

                logger.LogInformation("✅ Sample customer created: {Email} (Customer ID will be auto-generated)", email);
            }
            else
            {
                logger.LogError("❌ Failed to create sample customer user '{Email}': {Errors}",
                    email,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            logger.LogInformation("ℹ️  Customer user already exists: {Email}", email);
        }
    }
}
