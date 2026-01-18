using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CoolLibrary.Domain.Entities;
using CoolLibrary.Domain.Enums;

namespace CoolLibrary.Infrastructure.Data;

/// <summary>
/// Database seeding service for initial data population
/// Responsible for creating roles, default admin user, sample customers, books, authors, genres, and loans
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

            // STEP 3: Seed Authors
            logger.LogInformation("📌 Step 3: Seeding authors...");
            await SeedAuthorsAsync(dbContext, logger);

            // STEP 4: Seed Genres
            logger.LogInformation("📌 Step 4: Seeding genres...");
            await SeedGenresAsync(dbContext, logger);

            // STEP 5: Seed Books
            logger.LogInformation("📌 Step 5: Seeding books...");
            await SeedBooksAsync(dbContext, logger);

            // STEP 6: Seed Sample Customers (with ApplicationUser relationship)
            logger.LogInformation("📌 Step 6: Seeding sample customers...");
            await SeedSampleCustomersAsync(userManager, dbContext, logger);

            // STEP 7: Seed Additional Customers (20 more)
            logger.LogInformation("📌 Step 7: Seeding additional customers...");
            await SeedAdditionalCustomersAsync(userManager, dbContext, logger);

            // STEP 8: Seed Loans
            logger.LogInformation("📌 Step 8: Seeding loans...");
            await SeedLoansAsync(dbContext, logger);
            
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
    /// Seeds authors into the database
    /// </summary>
    private static async Task SeedAuthorsAsync(LibraryDbContext dbContext, ILogger logger)
    {
        if (dbContext.Authors.Any())
        {
            logger.LogInformation("ℹ️  Authors already exist, skipping...");
            return;
        }

        var authors = new List<Author>
        {
            new Author { FirstName = "J.K.", LastName = "Rowling", NormalizedFullName = "J.K. ROWLING", Nationality = "British", Biography = "British author, best known for the Harry Potter series", BirthDate = new DateTime(1965, 7, 31), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Author { FirstName = "George", LastName = "Orwell", NormalizedFullName = "GEORGE ORWELL", Nationality = "British", Biography = "English novelist and essayist, journalist and critic", BirthDate = new DateTime(1903, 6, 25), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Author { FirstName = "Jane", LastName = "Austen", NormalizedFullName = "JANE AUSTEN", Nationality = "British", Biography = "English novelist known primarily for her six major novels", BirthDate = new DateTime(1775, 12, 16), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Author { FirstName = "F. Scott", LastName = "Fitzgerald", NormalizedFullName = "F. SCOTT FITZGERALD", Nationality = "American", Biography = "American novelist and short story writer", BirthDate = new DateTime(1896, 9, 24), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Author { FirstName = "Harper", LastName = "Lee", NormalizedFullName = "HARPER LEE", Nationality = "American", Biography = "American novelist widely known for To Kill a Mockingbird", BirthDate = new DateTime(1926, 4, 28), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Author { FirstName = "Gabriel", LastName = "García Márquez", NormalizedFullName = "GABRIEL GARCÍA MÁRQUEZ", Nationality = "Colombian", Biography = "Colombian novelist and Nobel Prize winner", BirthDate = new DateTime(1927, 3, 6), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Author { FirstName = "Leo", LastName = "Tolstoy", NormalizedFullName = "LEO TOLSTOY", Nationality = "Russian", Biography = "Russian writer regarded as one of the greatest authors of all time", BirthDate = new DateTime(1828, 9, 9), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Author { FirstName = "Agatha", LastName = "Christie", NormalizedFullName = "AGATHA CHRISTIE", Nationality = "British", Biography = "English writer known for her detective novels", BirthDate = new DateTime(1890, 9, 15), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Author { FirstName = "J.R.R.", LastName = "Tolkien", NormalizedFullName = "J.R.R. TOLKIEN", Nationality = "British", Biography = "English writer, poet, and philologist, author of The Lord of the Rings", BirthDate = new DateTime(1892, 1, 3), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Author { FirstName = "Ernest", LastName = "Hemingway", NormalizedFullName = "ERNEST HEMINGWAY", Nationality = "American", Biography = "American novelist, short-story writer, and journalist", BirthDate = new DateTime(1899, 7, 21), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Author { FirstName = "Virginia", LastName = "Woolf", NormalizedFullName = "VIRGINIA WOOLF", Nationality = "British", Biography = "English writer, considered one of the most important modernist authors", BirthDate = new DateTime(1882, 1, 25), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Author { FirstName = "Mark", LastName = "Twain", NormalizedFullName = "MARK TWAIN", Nationality = "American", Biography = "American writer, humorist, entrepreneur, and lecturer", BirthDate = new DateTime(1835, 11, 30), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Author { FirstName = "Stephen", LastName = "King", NormalizedFullName = "STEPHEN KING", Nationality = "American", Biography = "American author of horror, supernatural fiction, suspense, and fantasy", BirthDate = new DateTime(1947, 9, 21), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Author { FirstName = "Margaret", LastName = "Atwood", NormalizedFullName = "MARGARET ATWOOD", Nationality = "Canadian", Biography = "Canadian poet, novelist, literary critic, and essayist", BirthDate = new DateTime(1939, 11, 18), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Author { FirstName = "Paulo", LastName = "Coelho", NormalizedFullName = "PAULO COELHO", Nationality = "Brazilian", Biography = "Brazilian lyricist and novelist, best known for The Alchemist", BirthDate = new DateTime(1947, 8, 24), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        dbContext.Authors.AddRange(authors);
        await dbContext.SaveChangesAsync();
        logger.LogInformation($"✅ {authors.Count} authors seeded successfully");
    }

    /// <summary>
    /// Seeds genres into the database
    /// </summary>
    private static async Task SeedGenresAsync(LibraryDbContext dbContext, ILogger logger)
    {
        if (dbContext.Genres.Any())
        {
            logger.LogInformation("ℹ️  Genres already exist, skipping...");
            return;
        }

        var genres = new List<Genre>
        {
            new Genre { Name = "Fiction", Description = "Literary works of imaginative narration", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Genre { Name = "Fantasy", Description = "Fiction involving magic and adventure", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Genre { Name = "Science Fiction", Description = "Fiction based on imagined future scientific or technological advances", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Genre { Name = "Mystery", Description = "Fiction dealing with solving a crime or unraveling secrets", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Genre { Name = "Romance", Description = "Fiction focused on romantic relationships", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Genre { Name = "Horror", Description = "Fiction intended to frighten or unsettle", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Genre { Name = "Classic", Description = "Works of enduring excellence and cultural significance", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Genre { Name = "Literary Fiction", Description = "Character-driven fiction with artistic merit", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Genre { Name = "Historical Fiction", Description = "Fiction set in a historical period", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Genre { Name = "Dystopian", Description = "Fiction depicting an undesirable society", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        dbContext.Genres.AddRange(genres);
        await dbContext.SaveChangesAsync();
        logger.LogInformation($"✅ {genres.Count} genres seeded successfully");
    }

    /// <summary>
    /// Seeds books with their author and genre relationships
    /// </summary>
    private static async Task SeedBooksAsync(LibraryDbContext dbContext, ILogger logger)
    {
        if (dbContext.Books.Any())
        {
            logger.LogInformation("ℹ️  Books already exist, skipping...");
            return;
        }

        var authors = dbContext.Authors.ToList();
        var genres = dbContext.Genres.ToList();

        var books = new List<Book>
        {
            new Book { ISBN = "978-0439708180", Title = "Harry Potter and the Sorcerer's Stone", Description = "The first adventure of young wizard Harry Potter", Publisher = "Scholastic", PublicationDate = new DateTime(1997, 6, 26), PageCount = 309, Language = "English", AvailableCopies = 5, TotalCopies = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Book { ISBN = "978-0451524935", Title = "1984", Description = "A dystopian social science fiction novel and cautionary tale", Publisher = "Secker & Warburg", PublicationDate = new DateTime(1949, 6, 8), PageCount = 328, Language = "English", AvailableCopies = 3, TotalCopies = 3, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Book { ISBN = "978-0141439518", Title = "Pride and Prejudice", Description = "A romantic novel of manners", Publisher = "T. Egerton", PublicationDate = new DateTime(1813, 1, 28), PageCount = 432, Language = "English", AvailableCopies = 4, TotalCopies = 4, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Book { ISBN = "978-0743273565", Title = "The Great Gatsby", Description = "A portrait of the Jazz Age in all of its decadence", Publisher = "Scribner", PublicationDate = new DateTime(1925, 4, 10), PageCount = 180, Language = "English", AvailableCopies = 3, TotalCopies = 3, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Book { ISBN = "978-0060935467", Title = "To Kill a Mockingbird", Description = "A novel about racial injustice in the American South", Publisher = "J.B. Lippincott & Co.", PublicationDate = new DateTime(1960, 7, 11), PageCount = 324, Language = "English", AvailableCopies = 4, TotalCopies = 4, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Book { ISBN = "978-0060883287", Title = "One Hundred Years of Solitude", Description = "The multi-generational story of the Buendía family", Publisher = "Harper & Row", PublicationDate = new DateTime(1967, 5, 30), PageCount = 417, Language = "Spanish", AvailableCopies = 2, TotalCopies = 2, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Book { ISBN = "978-0143035008", Title = "War and Peace", Description = "A literary work mixed with chapters on history and philosophy", Publisher = "The Russian Messenger", PublicationDate = new DateTime(1869, 1, 1), PageCount = 1225, Language = "Russian", AvailableCopies = 2, TotalCopies = 2, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Book { ISBN = "978-0062073488", Title = "And Then There Were None", Description = "A mystery novel about ten strangers trapped on an island", Publisher = "Collins Crime Club", PublicationDate = new DateTime(1939, 11, 6), PageCount = 272, Language = "English", AvailableCopies = 3, TotalCopies = 3, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Book { ISBN = "978-0547928227", Title = "The Hobbit", Description = "The adventures of hobbit Bilbo Baggins", Publisher = "George Allen & Unwin", PublicationDate = new DateTime(1937, 9, 21), PageCount = 310, Language = "English", AvailableCopies = 4, TotalCopies = 4, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Book { ISBN = "978-0684801223", Title = "The Old Man and the Sea", Description = "The story of an aging Cuban fisherman", Publisher = "Charles Scribner's Sons", PublicationDate = new DateTime(1952, 9, 1), PageCount = 127, Language = "English", AvailableCopies = 3, TotalCopies = 3, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Book { ISBN = "978-0156907392", Title = "Mrs. Dalloway", Description = "A novel detailing a day in the life of Clarissa Dalloway", Publisher = "Hogarth Press", PublicationDate = new DateTime(1925, 5, 14), PageCount = 194, Language = "English", AvailableCopies = 2, TotalCopies = 2, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Book { ISBN = "978-0486280615", Title = "Adventures of Huckleberry Finn", Description = "The adventures of a boy and a runaway slave", Publisher = "Chatto & Windus", PublicationDate = new DateTime(1884, 12, 10), PageCount = 366, Language = "English", AvailableCopies = 3, TotalCopies = 3, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Book { ISBN = "978-1501142970", Title = "The Shining", Description = "A horror novel about a family in an isolated hotel", Publisher = "Doubleday", PublicationDate = new DateTime(1977, 1, 28), PageCount = 447, Language = "English", AvailableCopies = 4, TotalCopies = 4, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Book { ISBN = "978-0385490818", Title = "The Handmaid's Tale", Description = "A dystopian novel set in a totalitarian society", Publisher = "McClelland and Stewart", PublicationDate = new DateTime(1985, 6, 1), PageCount = 311, Language = "English", AvailableCopies = 3, TotalCopies = 3, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Book { ISBN = "978-0062315007", Title = "The Alchemist", Description = "A philosophical book about a young Andalusian shepherd", Publisher = "HarperTorch", PublicationDate = new DateTime(1988, 4, 15), PageCount = 208, Language = "Portuguese", AvailableCopies = 5, TotalCopies = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Book { ISBN = "978-0439023481", Title = "The Hunger Games", Description = "A dystopian novel about a televised fight to the death", Publisher = "Scholastic Press", PublicationDate = new DateTime(2008, 9, 14), PageCount = 374, Language = "English", AvailableCopies = 4, TotalCopies = 4, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Book { ISBN = "978-0316769174", Title = "The Catcher in the Rye", Description = "A story about teenage rebellion and alienation", Publisher = "Little, Brown and Company", PublicationDate = new DateTime(1951, 7, 16), PageCount = 234, Language = "English", AvailableCopies = 3, TotalCopies = 3, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Book { ISBN = "978-0061120084", Title = "Brave New World", Description = "A dystopian novel set in a futuristic World State", Publisher = "Chatto & Windus", PublicationDate = new DateTime(1932, 1, 1), PageCount = 311, Language = "English", AvailableCopies = 2, TotalCopies = 2, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Book { ISBN = "978-0316346627", Title = "The Book Thief", Description = "A novel about a young girl living with foster parents in Nazi Germany", Publisher = "Picador", PublicationDate = new DateTime(2005, 9, 1), PageCount = 552, Language = "English", AvailableCopies = 4, TotalCopies = 4, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Book { ISBN = "978-0544003415", Title = "The Lord of the Rings", Description = "The epic fantasy trilogy about the War of the Ring", Publisher = "George Allen & Unwin", PublicationDate = new DateTime(1954, 7, 29), PageCount = 1178, Language = "English", AvailableCopies = 3, TotalCopies = 3, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        dbContext.Books.AddRange(books);
        await dbContext.SaveChangesAsync();

        // Create BookAuthor relationships
        var bookAuthors = new List<BookAuthor>
        {
            new BookAuthor { BookId = books[0].BookId, AuthorId = authors.First(a => a.LastName == "Rowling").AuthorId },
            new BookAuthor { BookId = books[1].BookId, AuthorId = authors.First(a => a.LastName == "Orwell").AuthorId },
            new BookAuthor { BookId = books[2].BookId, AuthorId = authors.First(a => a.LastName == "Austen").AuthorId },
            new BookAuthor { BookId = books[3].BookId, AuthorId = authors.First(a => a.LastName == "Fitzgerald").AuthorId },
            new BookAuthor { BookId = books[4].BookId, AuthorId = authors.First(a => a.LastName == "Lee").AuthorId },
            new BookAuthor { BookId = books[5].BookId, AuthorId = authors.First(a => a.LastName == "García Márquez").AuthorId },
            new BookAuthor { BookId = books[6].BookId, AuthorId = authors.First(a => a.LastName == "Tolstoy").AuthorId },
            new BookAuthor { BookId = books[7].BookId, AuthorId = authors.First(a => a.LastName == "Christie").AuthorId },
            new BookAuthor { BookId = books[8].BookId, AuthorId = authors.First(a => a.LastName == "Tolkien").AuthorId },
            new BookAuthor { BookId = books[9].BookId, AuthorId = authors.First(a => a.LastName == "Hemingway").AuthorId },
            new BookAuthor { BookId = books[10].BookId, AuthorId = authors.First(a => a.LastName == "Woolf").AuthorId },
            new BookAuthor { BookId = books[11].BookId, AuthorId = authors.First(a => a.LastName == "Twain").AuthorId },
            new BookAuthor { BookId = books[12].BookId, AuthorId = authors.First(a => a.LastName == "King").AuthorId },
            new BookAuthor { BookId = books[13].BookId, AuthorId = authors.First(a => a.LastName == "Atwood").AuthorId },
            new BookAuthor { BookId = books[14].BookId, AuthorId = authors.First(a => a.LastName == "Coelho").AuthorId },
            new BookAuthor { BookId = books[19].BookId, AuthorId = authors.First(a => a.LastName == "Tolkien").AuthorId }
        };

        dbContext.BookAuthors.AddRange(bookAuthors);

        // Create BookGenre relationships
        var bookGenres = new List<BookGenre>
        {
            new BookGenre { BookId = books[0].BookId, GenreId = genres.First(g => g.Name == "Fantasy").GenreId },
            new BookGenre { BookId = books[1].BookId, GenreId = genres.First(g => g.Name == "Dystopian").GenreId },
            new BookGenre { BookId = books[1].BookId, GenreId = genres.First(g => g.Name == "Science Fiction").GenreId },
            new BookGenre { BookId = books[2].BookId, GenreId = genres.First(g => g.Name == "Romance").GenreId },
            new BookGenre { BookId = books[2].BookId, GenreId = genres.First(g => g.Name == "Classic").GenreId },
            new BookGenre { BookId = books[3].BookId, GenreId = genres.First(g => g.Name == "Classic").GenreId },
            new BookGenre { BookId = books[3].BookId, GenreId = genres.First(g => g.Name == "Literary Fiction").GenreId },
            new BookGenre { BookId = books[4].BookId, GenreId = genres.First(g => g.Name == "Classic").GenreId },
            new BookGenre { BookId = books[4].BookId, GenreId = genres.First(g => g.Name == "Historical Fiction").GenreId },
            new BookGenre { BookId = books[5].BookId, GenreId = genres.First(g => g.Name == "Literary Fiction").GenreId },
            new BookGenre { BookId = books[6].BookId, GenreId = genres.First(g => g.Name == "Historical Fiction").GenreId },
            new BookGenre { BookId = books[6].BookId, GenreId = genres.First(g => g.Name == "Classic").GenreId },
            new BookGenre { BookId = books[7].BookId, GenreId = genres.First(g => g.Name == "Mystery").GenreId },
            new BookGenre { BookId = books[8].BookId, GenreId = genres.First(g => g.Name == "Fantasy").GenreId },
            new BookGenre { BookId = books[9].BookId, GenreId = genres.First(g => g.Name == "Classic").GenreId },
            new BookGenre { BookId = books[10].BookId, GenreId = genres.First(g => g.Name == "Literary Fiction").GenreId },
            new BookGenre { BookId = books[11].BookId, GenreId = genres.First(g => g.Name == "Classic").GenreId },
            new BookGenre { BookId = books[12].BookId, GenreId = genres.First(g => g.Name == "Horror").GenreId },
            new BookGenre { BookId = books[13].BookId, GenreId = genres.First(g => g.Name == "Dystopian").GenreId },
            new BookGenre { BookId = books[13].BookId, GenreId = genres.First(g => g.Name == "Science Fiction").GenreId },
            new BookGenre { BookId = books[14].BookId, GenreId = genres.First(g => g.Name == "Fiction").GenreId },
            new BookGenre { BookId = books[19].BookId, GenreId = genres.First(g => g.Name == "Fantasy").GenreId }
        };

        dbContext.BookGenres.AddRange(bookGenres);
        await dbContext.SaveChangesAsync();

        logger.LogInformation($"✅ {books.Count} books seeded successfully with author and genre relationships");
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

    /// <summary>
    /// Seeds 20 additional customers for GraphQL testing
    /// </summary>
    private static async Task SeedAdditionalCustomersAsync(
        UserManager<ApplicationUser> userManager,
        LibraryDbContext dbContext,
        ILogger logger)
    {
        var additionalCustomers = new List<(string email, string firstName, string lastName, string phone, string address, string city, string postalCode)>
        {
            ("michael.brown@email.com", "Michael", "Brown", "+1-555-0103", "789 Pine Road", "Chicago", "60601"),
            ("sarah.wilson@email.com", "Sarah", "Wilson", "+1-555-0104", "321 Elm Street", "Houston", "77001"),
            ("david.moore@email.com", "David", "Moore", "+1-555-0105", "654 Maple Avenue", "Phoenix", "85001"),
            ("laura.taylor@email.com", "Laura", "Taylor", "+1-555-0106", "987 Cedar Lane", "Philadelphia", "19019"),
            ("james.anderson@email.com", "James", "Anderson", "+1-555-0107", "147 Birch Drive", "San Antonio", "78201"),
            ("emma.thomas@email.com", "Emma", "Thomas", "+1-555-0108", "258 Spruce Court", "San Diego", "92101"),
            ("robert.jackson@email.com", "Robert", "Jackson", "+1-555-0109", "369 Willow Way", "Dallas", "75201"),
            ("olivia.white@email.com", "Olivia", "White", "+1-555-0110", "741 Ash Boulevard", "San Jose", "95101"),
            ("william.harris@email.com", "William", "Harris", "+1-555-0111", "852 Oak Terrace", "Austin", "78701"),
            ("sophia.martin@email.com", "Sophia", "Martin", "+1-555-0112", "963 Pine Circle", "Jacksonville", "32099"),
            ("daniel.garcia@email.com", "Daniel", "Garcia", "+1-555-0113", "159 Elm Place", "Fort Worth", "76101"),
            ("isabella.martinez@email.com", "Isabella", "Martinez", "+1-555-0114", "357 Maple Street", "Columbus", "43004"),
            ("matthew.robinson@email.com", "Matthew", "Robinson", "+1-555-0115", "486 Cedar Avenue", "Charlotte", "28201"),
            ("mia.clark@email.com", "Mia", "Clark", "+1-555-0116", "753 Birch Road", "San Francisco", "94101"),
            ("joseph.rodriguez@email.com", "Joseph", "Rodriguez", "+1-555-0117", "951 Spruce Lane", "Indianapolis", "46201"),
            ("charlotte.lewis@email.com", "Charlotte", "Lewis", "+1-555-0118", "357 Willow Drive", "Seattle", "98101"),
            ("christopher.lee@email.com", "Christopher", "Lee", "+1-555-0119", "159 Ash Court", "Denver", "80201"),
            ("amelia.walker@email.com", "Amelia", "Walker", "+1-555-0120", "753 Oak Way", "Boston", "02101"),
            ("alexander.hall@email.com", "Alexander", "Hall", "+1-555-0121", "951 Pine Boulevard", "Nashville", "37201"),
            ("evelyn.allen@email.com", "Evelyn", "Allen", "+1-555-0122", "357 Elm Terrace", "Detroit", "48201")
        };

        foreach (var (email, firstName, lastName, phone, address, city, postalCode) in additionalCustomers)
        {
            await CreateCustomerIfNotExistsAsync(
                userManager,
                dbContext,
                logger,
                email,
                "Customer$123!",
                firstName,
                lastName,
                phone,
                address,
                city,
                postalCode,
                maxBooksAllowed: 5
            );
        }

        logger.LogInformation("✅ Additional 20 customers seeded successfully");
    }

    /// <summary>
    /// Seeds loans for customers with various books
    /// </summary>
    private static async Task SeedLoansAsync(LibraryDbContext dbContext, ILogger logger)
    {
        if (dbContext.Loans.Any())
        {
            logger.LogInformation("ℹ️  Loans already exist, skipping...");
            return;
        }

        var customers = dbContext.Customers.ToList();
        var books = dbContext.Books.ToList();
        var random = new Random(42); // Fixed seed for reproducibility

        var loans = new List<Loan>();
        var loanDate = DateTime.UtcNow.AddMonths(-2); // Start loans 2 months ago

        // Create loans for each customer (1-3 loans per customer)
        foreach (var customer in customers)
        {
            int loanCount = random.Next(1, 4); // 1 to 3 loans
            var availableBooks = books.Where(b => b.AvailableCopies > 0).ToList();

            for (int i = 0; i < loanCount && availableBooks.Any(); i++)
            {
                var book = availableBooks[random.Next(availableBooks.Count)];
                var isReturned = random.Next(0, 100) < 60; // 60% chance the book is returned
                
                // Generate actual loan date first
                var actualLoanDate = loanDate.AddDays(random.Next(0, 60));
                
                var loan = new Loan
                {
                    CustomerId = customer.CustomerId,
                    BookId = book.BookId,
                    LoanDate = actualLoanDate,
                    DueDate = actualLoanDate.AddDays(random.Next(14, 28)),
                    Status = isReturned ? LoanStatus.Returned : LoanStatus.Active,
                    ReturnDate = isReturned ? (DateTime?)actualLoanDate.AddDays(random.Next(7, 21)) : null,
                    RenewalCount = random.Next(0, 3),
                    LateFee = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Calculate late fee if applicable
                if (!isReturned && loan.IsOverdue)
                {
                    loan.LateFee = loan.DaysOverdue * 0.5m; // $0.50 per day
                }

                loans.Add(loan);
                
                // Update book availability
                if (!isReturned)
                {
                    book.AvailableCopies--;
                }

                availableBooks.Remove(book);
            }
        }

        dbContext.Loans.AddRange(loans);
        await dbContext.SaveChangesAsync();

        logger.LogInformation($"✅ {loans.Count} loans seeded successfully");
    }
}
