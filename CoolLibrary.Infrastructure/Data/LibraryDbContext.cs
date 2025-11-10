using CoolLibrary.Domain.Entities;
using CoolLibrary.Domain.Enums;
using CoolLibrary.Infrastructure.Data.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoolLibrary.Infrastructure.Data;

/// <summary>
/// Entity Framework DbContext for the Library Management System
/// Uses ApplicationUser instead of IdentityUser for custom user properties
/// </summary>
public class LibraryDbContext : IdentityDbContext<ApplicationUser>
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
    }

    // DbSets for all entities
    public DbSet<Author> Authors { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Fine> Fines { get; set; }
    public DbSet<BookAuthor> BookAuthors { get; set; }
    public DbSet<BookGenre> BookGenres { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations
        modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());  // ? NEW
        modelBuilder.ApplyConfiguration(new AuthorConfiguration());
        modelBuilder.ApplyConfiguration(new GenreConfiguration());
        modelBuilder.ApplyConfiguration(new BookConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new LoanConfiguration());
        modelBuilder.ApplyConfiguration(new ReservationConfiguration());
        modelBuilder.ApplyConfiguration(new FineConfiguration());
        modelBuilder.ApplyConfiguration(new BookAuthorConfiguration());
        modelBuilder.ApplyConfiguration(new BookGenreConfiguration());

        // Seed initial data
        SeedData(modelBuilder);
    }

    /// <summary>
    /// Seeds initial data for testing and development
    /// </summary>
    private void SeedData(ModelBuilder modelBuilder)
    {
        // Use static dates to avoid model changes on each build
        var baseDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        // Seed Authors
        modelBuilder.Entity<Author>().HasData(
            new Author
            {
                AuthorId = 1,
                FirstName = "George",
                LastName = "Orwell",
                Biography = "English novelist and essayist, journalist and critic",
                BirthDate = new DateTime(1903, 6, 25),
                Nationality = "British",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Author
            {
                AuthorId = 2,
                FirstName = "Jane",
                LastName = "Austen",
                Biography = "English novelist known primarily for her six major novels",
                BirthDate = new DateTime(1775, 12, 16),
                Nationality = "British",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Author
            {
                AuthorId = 3,
                FirstName = "J.K.",
                LastName = "Rowling",
                Biography = "British author, best known for the Harry Potter series",
                BirthDate = new DateTime(1965, 7, 31),
                Nationality = "British",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            // Nuevos autores
            new Author
            {
                AuthorId = 4,
                FirstName = "Gabriel",
                LastName = "García Márquez",
                Biography = "Colombian novelist and Nobel Prize winner",
                BirthDate = new DateTime(1927, 3, 6),
                Nationality = "Colombian",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Author
            {
                AuthorId = 5,
                FirstName = "Stephen",
                LastName = "King",
                Biography = "American author of horror, supernatural fiction, suspense, crime, science-fiction, and fantasy novels",
                BirthDate = new DateTime(1947, 9, 21),
                Nationality = "American",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Author
            {
                AuthorId = 6,
                FirstName = "Agatha",
                LastName = "Christie",
                Biography = "English writer known for her detective novels",
                BirthDate = new DateTime(1890, 9, 15),
                Nationality = "British",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Author
            {
                AuthorId = 7,
                FirstName = "J.R.R.",
                LastName = "Tolkien",
                Biography = "English writer, poet, philologist, and academic, best known as the author of The Lord of the Rings",
                BirthDate = new DateTime(1892, 1, 3),
                Nationality = "British",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Author
            {
                AuthorId = 8,
                FirstName = "Dan",
                LastName = "Brown",
                Biography = "American author best known for his thriller novels",
                BirthDate = new DateTime(1964, 6, 22),
                Nationality = "American",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Author
            {
                AuthorId = 9,
                FirstName = "Isaac",
                LastName = "Asimov",
                Biography = "American writer and professor of biochemistry, best known for his works of science fiction",
                BirthDate = new DateTime(1920, 1, 2),
                Nationality = "American",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Author
            {
                AuthorId = 10,
                FirstName = "Leo",
                LastName = "Tolstoy",
                Biography = "Russian writer who is regarded as one of the greatest authors of all time",
                BirthDate = new DateTime(1828, 9, 9),
                Nationality = "Russian",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Author
            {
                AuthorId = 11,
                FirstName = "F. Scott",
                LastName = "Fitzgerald",
                Biography = "American novelist, essayist, and short story writer",
                BirthDate = new DateTime(1896, 9, 24),
                Nationality = "American",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Author
            {
                AuthorId = 12,
                FirstName = "Harper",
                LastName = "Lee",
                Biography = "American novelist best known for To Kill a Mockingbird",
                BirthDate = new DateTime(1926, 4, 28),
                Nationality = "American",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Author
            {
                AuthorId = 13,
                FirstName = "Ernest",
                LastName = "Hemingway",
                Biography = "American novelist, short-story writer, and journalist",
                BirthDate = new DateTime(1899, 7, 21),
                Nationality = "American",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Author
            {
                AuthorId = 14,
                FirstName = "Paulo",
                LastName = "Coelho",
                Biography = "Brazilian lyricist and novelist, best known for The Alchemist",
                BirthDate = new DateTime(1947, 8, 24),
                Nationality = "Brazilian",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Author
            {
                AuthorId = 15,
                FirstName = "Margaret",
                LastName = "Atwood",
                Biography = "Canadian poet, novelist, literary critic, essayist, and environmental activist",
                BirthDate = new DateTime(1939, 11, 18),
                Nationality = "Canadian",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Author
            {
                AuthorId = 16,
                FirstName = "Arthur",
                LastName = "Conan Doyle",
                Biography = "British writer and physician, best known for creating Sherlock Holmes",
                BirthDate = new DateTime(1859, 5, 22),
                Nationality = "British",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Author
            {
                AuthorId = 17,
                FirstName = "Victor",
                LastName = "Hugo",
                Biography = "French poet, novelist, and dramatist of the Romantic movement",
                BirthDate = new DateTime(1802, 2, 26),
                Nationality = "French",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Author
            {
                AuthorId = 18,
                FirstName = "Ray",
                LastName = "Bradbury",
                Biography = "American author and screenwriter, one of the most celebrated 20th-century American writers",
                BirthDate = new DateTime(1920, 8, 22),
                Nationality = "American",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Author
            {
                AuthorId = 19,
                FirstName = "C.S.",
                LastName = "Lewis",
                Biography = "British writer and lay theologian, best known for The Chronicles of Narnia",
                BirthDate = new DateTime(1898, 11, 29),
                Nationality = "British",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Author
            {
                AuthorId = 20,
                FirstName = "Aldous",
                LastName = "Huxley",
                Biography = "English writer and philosopher, best known for Brave New World",
                BirthDate = new DateTime(1894, 7, 26),
                Nationality = "British",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            }
        );

        // Seed Genres
        modelBuilder.Entity<Genre>().HasData(
            new Genre
            {
                GenreId = 1,
                Name = "Fiction",
                Description = "Literary works of imaginative narration",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Genre
            {
                GenreId = 2,
                Name = "Fantasy",
                Description = "Fiction involving magical or supernatural elements",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Genre
            {
                GenreId = 3,
                Name = "Classic Literature",
                Description = "Literature that has stood the test of time",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            // Nuevos géneros
            new Genre
            {
                GenreId = 4,
                Name = "Mystery",
                Description = "Fiction dealing with the solution of a crime or the unraveling of secrets",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Genre
            {
                GenreId = 5,
                Name = "Science Fiction",
                Description = "Fiction based on imagined future scientific or technological advances",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Genre
            {
                GenreId = 6,
                Name = "Horror",
                Description = "Fiction intended to scare, unsettle, or horrify the reader",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Genre
            {
                GenreId = 7,
                Name = "Thriller",
                Description = "Fiction characterized by fast pacing, frequent action, and resourceful heroes",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Genre
            {
                GenreId = 8,
                Name = "Romance",
                Description = "Fiction focusing on romantic love and relationships",
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            }
        );

        // Seed Books
        modelBuilder.Entity<Book>().HasData(
            new Book
            {
                BookId = 1,
                ISBN = "978-0-452-28423-4",
                Title = "1984",
                Description = "A dystopian social science fiction novel and cautionary tale",
                PublicationDate = new DateTime(1949, 6, 8),
                Publisher = "Secker & Warburg",
                PageCount = 328,
                Language = "English",
                AvailableCopies = 3,
                TotalCopies = 5,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 2,
                ISBN = "978-0-14-143951-8",
                Title = "Pride and Prejudice",
                Description = "A romantic novel of manners",
                PublicationDate = new DateTime(1813, 1, 28),
                Publisher = "T. Egerton",
                PageCount = 432,
                Language = "English",
                AvailableCopies = 2,
                TotalCopies = 3,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 3,
                ISBN = "978-0-439-70818-8",
                Title = "Harry Potter and the Philosopher's Stone",
                Description = "The first novel in the Harry Potter series",
                PublicationDate = new DateTime(1997, 6, 26),
                Publisher = "Bloomsbury",
                PageCount = 223,
                Language = "English",
                AvailableCopies = 4,
                TotalCopies = 4,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 4,
                ISBN = "978-0-452-28424-1",
                Title = "Animal Farm",
                Description = "An allegorical novella about farm animals",
                PublicationDate = new DateTime(1945, 8, 17),
                Publisher = "Secker & Warburg",
                PageCount = 95,
                Language = "English",
                AvailableCopies = 1,
                TotalCopies = 2,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 5,
                ISBN = "978-0-14-143952-5",
                Title = "Emma",
                Description = "A novel about youthful hubris and romantic misunderstandings",
                PublicationDate = new DateTime(1815, 12, 23),
                Publisher = "John Murray",
                PageCount = 474,
                Language = "English",
                AvailableCopies = 3,
                TotalCopies = 3,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            // Nuevos 30 libros
            new Book
            {
                BookId = 6,
                ISBN = "978-0-06-088328-7",
                Title = "One Hundred Years of Solitude",
                Description = "The multi-generational story of the Buendía family",
                PublicationDate = new DateTime(1967, 5, 30),
                Publisher = "Harper & Row",
                PageCount = 417,
                Language = "English",
                AvailableCopies = 5,
                TotalCopies = 6,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 7,
                ISBN = "978-0-385-33312-0",
                Title = "The Shining",
                Description = "A family heads to an isolated hotel for the winter",
                PublicationDate = new DateTime(1977, 1, 28),
                Publisher = "Doubleday",
                PageCount = 447,
                Language = "English",
                AvailableCopies = 2,
                TotalCopies = 4,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 8,
                ISBN = "978-0-06-207348-4",
                Title = "And Then There Were None",
                Description = "Ten strangers are lured to an isolated island mansion",
                PublicationDate = new DateTime(1939, 11, 6),
                Publisher = "Collins Crime Club",
                PageCount = 272,
                Language = "English",
                AvailableCopies = 3,
                TotalCopies = 5,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 9,
                ISBN = "978-0-618-00222-1",
                Title = "The Lord of the Rings",
                Description = "An epic high-fantasy novel",
                PublicationDate = new DateTime(1954, 7, 29),
                Publisher = "Allen & Unwin",
                PageCount = 1178,
                Language = "English",
                AvailableCopies = 6,
                TotalCopies = 8,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 10,
                ISBN = "978-0-307-47463-1",
                Title = "The Da Vinci Code",
                Description = "A mystery thriller novel",
                PublicationDate = new DateTime(2003, 3, 18),
                Publisher = "Doubleday",
                PageCount = 454,
                Language = "English",
                AvailableCopies = 4,
                TotalCopies = 6,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 11,
                ISBN = "978-0-553-29335-0",
                Title = "Foundation",
                Description = "A science fiction novel about the fall and rise of civilizations",
                PublicationDate = new DateTime(1951, 6, 1),
                Publisher = "Gnome Press",
                PageCount = 255,
                Language = "English",
                AvailableCopies = 3,
                TotalCopies = 4,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 12,
                ISBN = "978-1-4000-7954-0",
                Title = "War and Peace",
                Description = "A novel that chronicles the French invasion of Russia",
                PublicationDate = new DateTime(1869, 1, 1),
                Publisher = "The Russian Messenger",
                PageCount = 1225,
                Language = "English",
                AvailableCopies = 2,
                TotalCopies = 3,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 13,
                ISBN = "978-0-7432-7356-5",
                Title = "The Great Gatsby",
                Description = "A novel about the American Dream in the Jazz Age",
                PublicationDate = new DateTime(1925, 4, 10),
                Publisher = "Charles Scribner's Sons",
                PageCount = 180,
                Language = "English",
                AvailableCopies = 5,
                TotalCopies = 7,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 14,
                ISBN = "978-0-06-112008-4",
                Title = "To Kill a Mockingbird",
                Description = "A novel about racial injustice and childhood innocence",
                PublicationDate = new DateTime(1960, 7, 11),
                Publisher = "J.B. Lippincott & Co.",
                PageCount = 324,
                Language = "English",
                AvailableCopies = 4,
                TotalCopies = 6,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 15,
                ISBN = "978-0-684-80122-3",
                Title = "The Old Man and the Sea",
                Description = "A short novel about an aging Cuban fisherman",
                PublicationDate = new DateTime(1952, 9, 1),
                Publisher = "Charles Scribner's Sons",
                PageCount = 127,
                Language = "English",
                AvailableCopies = 3,
                TotalCopies = 4,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 16,
                ISBN = "978-0-06-112241-5",
                Title = "The Alchemist",
                Description = "A novel about a young Andalusian shepherd's journey to Egypt",
                PublicationDate = new DateTime(1988, 1, 1),
                Publisher = "HarperTorch",
                PageCount = 197,
                Language = "English",
                AvailableCopies = 6,
                TotalCopies = 8,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 17,
                ISBN = "978-0-385-49081-8",
                Title = "The Handmaid's Tale",
                Description = "A dystopian novel set in a totalitarian society",
                PublicationDate = new DateTime(1985, 9, 1),
                Publisher = "McClelland and Stewart",
                PageCount = 311,
                Language = "English",
                AvailableCopies = 4,
                TotalCopies = 5,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 18,
                ISBN = "978-0-14-017737-7",
                Title = "The Adventures of Sherlock Holmes",
                Description = "Collection of twelve short stories featuring Sherlock Holmes",
                PublicationDate = new DateTime(1892, 10, 14),
                Publisher = "George Newnes",
                PageCount = 307,
                Language = "English",
                AvailableCopies = 3,
                TotalCopies = 5,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 19,
                ISBN = "978-0-451-41943-5",
                Title = "Les Misérables",
                Description = "A French historical novel",
                PublicationDate = new DateTime(1862, 4, 3),
                Publisher = "A. Lacroix, Verboeckhoven & Cie",
                PageCount = 1463,
                Language = "English",
                AvailableCopies = 2,
                TotalCopies = 3,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 20,
                ISBN = "978-1-4516-7331-9",
                Title = "Fahrenheit 451",
                Description = "A dystopian novel about a future American society where books are outlawed",
                PublicationDate = new DateTime(1953, 10, 19),
                Publisher = "Ballantine Books",
                PageCount = 249,
                Language = "English",
                AvailableCopies = 5,
                TotalCopies = 6,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 21,
                ISBN = "978-0-06-112979-7",
                Title = "The Lion, the Witch and the Wardrobe",
                Description = "First published book in The Chronicles of Narnia series",
                PublicationDate = new DateTime(1950, 10, 16),
                Publisher = "Geoffrey Bles",
                PageCount = 206,
                Language = "English",
                AvailableCopies = 4,
                TotalCopies = 6,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 22,
                ISBN = "978-0-06-085052-4",
                Title = "Brave New World",
                Description = "A dystopian novel set in a futuristic World State",
                PublicationDate = new DateTime(1932, 1, 1),
                Publisher = "Chatto & Windus",
                PageCount = 311,
                Language = "English",
                AvailableCopies = 3,
                TotalCopies = 5,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 23,
                ISBN = "978-0-307-27778-7",
                Title = "Angels & Demons",
                Description = "A mystery thriller novel",
                PublicationDate = new DateTime(2000, 5, 1),
                Publisher = "Pocket Books",
                PageCount = 616,
                Language = "English",
                AvailableCopies = 3,
                TotalCopies = 4,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 24,
                ISBN = "978-0-345-34296-8",
                Title = "It",
                Description = "A horror novel about seven children terrorized by an evil entity",
                PublicationDate = new DateTime(1986, 9, 15),
                Publisher = "Viking",
                PageCount = 1138,
                Language = "English",
                AvailableCopies = 2,
                TotalCopies = 4,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 25,
                ISBN = "978-0-06-093546-7",
                Title = "Murder on the Orient Express",
                Description = "A detective novel featuring Hercule Poirot",
                PublicationDate = new DateTime(1934, 1, 1),
                Publisher = "Collins Crime Club",
                PageCount = 256,
                Language = "English",
                AvailableCopies = 4,
                TotalCopies = 5,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 26,
                ISBN = "978-0-618-57422-7",
                Title = "The Hobbit",
                Description = "A fantasy novel and children's book",
                PublicationDate = new DateTime(1937, 9, 21),
                Publisher = "George Allen & Unwin",
                PageCount = 310,
                Language = "English",
                AvailableCopies = 5,
                TotalCopies = 7,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 27,
                ISBN = "978-0-14-028329-5",
                Title = "Anna Karenina",
                Description = "A novel about a tragic love affair",
                PublicationDate = new DateTime(1878, 1, 1),
                Publisher = "The Russian Messenger",
                PageCount = 864,
                Language = "English",
                AvailableCopies = 2,
                TotalCopies = 3,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 28,
                ISBN = "978-0-7432-7357-2",
                Title = "For Whom the Bell Tolls",
                Description = "A novel about the Spanish Civil War",
                PublicationDate = new DateTime(1940, 10, 21),
                Publisher = "Charles Scribner's Sons",
                PageCount = 471,
                Language = "English",
                AvailableCopies = 3,
                TotalCopies = 4,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 29,
                ISBN = "978-0-553-38034-9",
                Title = "I, Robot",
                Description = "A collection of science fiction short stories",
                PublicationDate = new DateTime(1950, 12, 2),
                Publisher = "Gnome Press",
                PageCount = 224,
                Language = "English",
                AvailableCopies = 4,
                TotalCopies = 5,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 30,
                ISBN = "978-0-14-303943-3",
                Title = "Sense and Sensibility",
                Description = "A novel about the lives and loves of the Dashwood sisters",
                PublicationDate = new DateTime(1811, 10, 30),
                Publisher = "Thomas Egerton",
                PageCount = 409,
                Language = "English",
                AvailableCopies = 3,
                TotalCopies = 4,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 31,
                ISBN = "978-0-439-13959-0",
                Title = "Harry Potter and the Chamber of Secrets",
                Description = "The second novel in the Harry Potter series",
                PublicationDate = new DateTime(1998, 7, 2),
                Publisher = "Bloomsbury",
                PageCount = 251,
                Language = "English",
                AvailableCopies = 5,
                TotalCopies = 6,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 32,
                ISBN = "978-0-385-33313-7",
                Title = "The Stand",
                Description = "A post-apocalyptic dark fantasy novel",
                PublicationDate = new DateTime(1978, 10, 3),
                Publisher = "Doubleday",
                PageCount = 823,
                Language = "English",
                AvailableCopies = 2,
                TotalCopies = 3,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 33,
                ISBN = "978-0-06-093547-4",
                Title = "Death on the Nile",
                Description = "A detective novel featuring Hercule Poirot",
                PublicationDate = new DateTime(1937, 11, 1),
                Publisher = "Collins Crime Club",
                PageCount = 288,
                Language = "English",
                AvailableCopies = 3,
                TotalCopies = 4,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 34,
                ISBN = "978-0-06-088329-4",
                Title = "Love in the Time of Cholera",
                Description = "A novel about love, aging, illness, and death",
                PublicationDate = new DateTime(1985, 9, 5),
                Publisher = "Editorial Oveja Negra",
                PageCount = 348,
                Language = "English",
                AvailableCopies = 4,
                TotalCopies = 5,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            },
            new Book
            {
                BookId = 35,
                ISBN = "978-0-06-112980-3",
                Title = "Prince Caspian",
                Description = "The second published book in The Chronicles of Narnia series",
                PublicationDate = new DateTime(1951, 10, 15),
                Publisher = "Geoffrey Bles",
                PageCount = 223,
                Language = "English",
                AvailableCopies = 3,
                TotalCopies = 5,
                CreatedAt = baseDate,
                UpdatedAt = baseDate
            }
        );

        // Seed Book-Author relationships
        modelBuilder.Entity<BookAuthor>().HasData(
            new BookAuthor { BookId = 1, AuthorId = 1, AuthorOrder = 1 }, // 1984 by George Orwell
            new BookAuthor { BookId = 2, AuthorId = 2, AuthorOrder = 1 }, // Pride and Prejudice by Jane Austen
            new BookAuthor { BookId = 3, AuthorId = 3, AuthorOrder = 1 }, // Harry Potter by J.K. Rowling
            new BookAuthor { BookId = 4, AuthorId = 1, AuthorOrder = 1 }, // Animal Farm by George Orwell
            new BookAuthor { BookId = 5, AuthorId = 2, AuthorOrder = 1 },  // Emma by Jane Austen
            // Nuevas relaciones
            new BookAuthor { BookId = 6, AuthorId = 4, AuthorOrder = 1 }, // One Hundred Years of Solitude
            new BookAuthor { BookId = 7, AuthorId = 5, AuthorOrder = 1 }, // The Shining
            new BookAuthor { BookId = 8, AuthorId = 6, AuthorOrder = 1 }, // And Then There Were None
            new BookAuthor { BookId = 9, AuthorId = 7, AuthorOrder = 1 }, // The Lord of the Rings
            new BookAuthor { BookId = 10, AuthorId = 8, AuthorOrder = 1 }, // The Da Vinci Code
            new BookAuthor { BookId = 11, AuthorId = 9, AuthorOrder = 1 }, // Foundation
            new BookAuthor { BookId = 12, AuthorId = 10, AuthorOrder = 1 }, // War and Peace
            new BookAuthor { BookId = 13, AuthorId = 11, AuthorOrder = 1 }, // The Great Gatsby
            new BookAuthor { BookId = 14, AuthorId = 12, AuthorOrder = 1 }, // To Kill a Mockingbird
            new BookAuthor { BookId = 15, AuthorId = 13, AuthorOrder = 1 }, // The Old Man and the Sea
            new BookAuthor { BookId = 16, AuthorId = 14, AuthorOrder = 1 }, // The Alchemist
            new BookAuthor { BookId = 17, AuthorId = 15, AuthorOrder = 1 }, // The Handmaid's Tale
            new BookAuthor { BookId = 18, AuthorId = 16, AuthorOrder = 1 }, // The Adventures of Sherlock Holmes
            new BookAuthor { BookId = 19, AuthorId = 17, AuthorOrder = 1 }, // Les Misérables
            new BookAuthor { BookId = 20, AuthorId = 18, AuthorOrder = 1 }, // Fahrenheit 451
            new BookAuthor { BookId = 21, AuthorId = 19, AuthorOrder = 1 }, // The Lion, the Witch and the Wardrobe
            new BookAuthor { BookId = 22, AuthorId = 20, AuthorOrder = 1 }, // Brave New World
            new BookAuthor { BookId = 23, AuthorId = 8, AuthorOrder = 1 }, // Angels & Demons
            new BookAuthor { BookId = 24, AuthorId = 5, AuthorOrder = 1 }, // It
            new BookAuthor { BookId = 25, AuthorId = 6, AuthorOrder = 1 }, // Murder on the Orient Express
            new BookAuthor { BookId = 26, AuthorId = 7, AuthorOrder = 1 }, // The Hobbit
            new BookAuthor { BookId = 27, AuthorId = 10, AuthorOrder = 1 }, // Anna Karenina
            new BookAuthor { BookId = 28, AuthorId = 13, AuthorOrder = 1 }, // For Whom the Bell Tolls
            new BookAuthor { BookId = 29, AuthorId = 9, AuthorOrder = 1 }, // I, Robot
            new BookAuthor { BookId = 30, AuthorId = 2, AuthorOrder = 1 }, // Sense and Sensibility
            new BookAuthor { BookId = 31, AuthorId = 3, AuthorOrder = 1 }, // Harry Potter Chamber of Secrets
            new BookAuthor { BookId = 32, AuthorId = 5, AuthorOrder = 1 }, // The Stand
            new BookAuthor { BookId = 33, AuthorId = 6, AuthorOrder = 1 }, // Death on the Nile
            new BookAuthor { BookId = 34, AuthorId = 4, AuthorOrder = 1 }, // Love in the Time of Cholera
            new BookAuthor { BookId = 35, AuthorId = 19, AuthorOrder = 1 }  // Prince Caspian
        );

        // Seed Book-Genre relationships
        modelBuilder.Entity<BookGenre>().HasData(
            new BookGenre { BookId = 1, GenreId = 1 }, // 1984 - Fiction
            new BookGenre { BookId = 1, GenreId = 3 }, // 1984 - Classic Literature
            new BookGenre { BookId = 2, GenreId = 1 }, // Pride and Prejudice - Fiction
            new BookGenre { BookId = 2, GenreId = 3 }, // Pride and Prejudice - Classic Literature
            new BookGenre { BookId = 3, GenreId = 1 }, // Harry Potter - Fiction
            new BookGenre { BookId = 3, GenreId = 2 }, // Harry Potter - Fantasy
            new BookGenre { BookId = 4, GenreId = 1 }, // Animal Farm - Fiction
            new BookGenre { BookId = 4, GenreId = 3 }, // Animal Farm - Classic Literature
            new BookGenre { BookId = 5, GenreId = 1 }, // Emma - Fiction
            new BookGenre { BookId = 5, GenreId = 3 },  // Emma - Classic Literature
            // Nuevas relaciones
            new BookGenre { BookId = 6, GenreId = 1 }, // One Hundred Years of Solitude - Fiction
            new BookGenre { BookId = 6, GenreId = 3 }, // One Hundred Years of Solitude - Classic
            new BookGenre { BookId = 7, GenreId = 6 }, // The Shining - Horror
            new BookGenre { BookId = 8, GenreId = 4 }, // And Then There Were None - Mystery
            new BookGenre { BookId = 8, GenreId = 3 }, // And Then There Were None - Classic
            new BookGenre { BookId = 9, GenreId = 2 }, // Lord of the Rings - Fantasy
            new BookGenre { BookId = 9, GenreId = 3 }, // Lord of the Rings - Classic
            new BookGenre { BookId = 10, GenreId = 7 }, // The Da Vinci Code - Thriller
            new BookGenre { BookId = 10, GenreId = 4 }, // The Da Vinci Code - Mystery
            new BookGenre { BookId = 11, GenreId = 5 }, // Foundation - Science Fiction
            new BookGenre { BookId = 11, GenreId = 3 }, // Foundation - Classic
            new BookGenre { BookId = 12, GenreId = 1 }, // War and Peace - Fiction
            new BookGenre { BookId = 12, GenreId = 3 }, // War and Peace - Classic
            new BookGenre { BookId = 13, GenreId = 1 }, // The Great Gatsby - Fiction
            new BookGenre { BookId = 13, GenreId = 3 }, // The Great Gatsby - Classic
            new BookGenre { BookId = 14, GenreId = 1 }, // To Kill a Mockingbird - Fiction
            new BookGenre { BookId = 14, GenreId = 3 }, // To Kill a Mockingbird - Classic
            new BookGenre { BookId = 15, GenreId = 1 }, // The Old Man and the Sea - Fiction
            new BookGenre { BookId = 15, GenreId = 3 }, // The Old Man and the Sea - Classic
            new BookGenre { BookId = 16, GenreId = 1 }, // The Alchemist - Fiction
            new BookGenre { BookId = 17, GenreId = 5 }, // The Handmaid's Tale - Science Fiction
            new BookGenre { BookId = 17, GenreId = 1 }, // The Handmaid's Tale - Fiction
            new BookGenre { BookId = 18, GenreId = 4 }, // Sherlock Holmes - Mystery
            new BookGenre { BookId = 18, GenreId = 3 }, // Sherlock Holmes - Classic
            new BookGenre { BookId = 19, GenreId = 1 }, // Les Misérables - Fiction
            new BookGenre { BookId = 19, GenreId = 3 }, // Les Misérables - Classic
            new BookGenre { BookId = 20, GenreId = 5 }, // Fahrenheit 451 - Science Fiction
            new BookGenre { BookId = 20, GenreId = 3 }, // Fahrenheit 451 - Classic
            new BookGenre { BookId = 21, GenreId = 2 }, // Narnia - Fantasy
            new BookGenre { BookId = 21, GenreId = 3 }, // Narnia - Classic
            new BookGenre { BookId = 22, GenreId = 5 }, // Brave New World - Science Fiction
            new BookGenre { BookId = 22, GenreId = 3 }, // Brave New World - Classic
            new BookGenre { BookId = 23, GenreId = 7 }, // Angels & Demons - Thriller
            new BookGenre { BookId = 23, GenreId = 4 }, // Angels & Demons - Mystery
            new BookGenre { BookId = 24, GenreId = 6 }, // It - Horror
            new BookGenre { BookId = 25, GenreId = 4 }, // Murder on the Orient Express - Mystery
            new BookGenre { BookId = 25, GenreId = 3 }, // Murder on the Orient Express - Classic
            new BookGenre { BookId = 26, GenreId = 2 }, // The Hobbit - Fantasy
            new BookGenre { BookId = 26, GenreId = 3 }, // The Hobbit - Classic
            new BookGenre { BookId = 27, GenreId = 1 }, // Anna Karenina - Fiction
            new BookGenre { BookId = 27, GenreId = 3 }, // Anna Karenina - Classic
            new BookGenre { BookId = 27, GenreId = 8 }, // Anna Karenina - Romance
            new BookGenre { BookId = 28, GenreId = 1 }, // For Whom the Bell Tolls - Fiction
            new BookGenre { BookId = 28, GenreId = 3 }, // For Whom the Bell Tolls - Classic
            new BookGenre { BookId = 29, GenreId = 5 }, // I, Robot - Science Fiction
            new BookGenre { BookId = 29, GenreId = 3 }, // I, Robot - Classic
            new BookGenre { BookId = 30, GenreId = 1 }, // Sense and Sensibility - Fiction
            new BookGenre { BookId = 30, GenreId = 3 }, // Sense and Sensibility - Classic
            new BookGenre { BookId = 30, GenreId = 8 }, // Sense and Sensibility - Romance
            new BookGenre { BookId = 31, GenreId = 1 }, // Harry Potter 2 - Fiction
            new BookGenre { BookId = 31, GenreId = 2 }, // Harry Potter 2 - Fantasy
            new BookGenre { BookId = 32, GenreId = 6 }, // The Stand - Horror
            new BookGenre { BookId = 32, GenreId = 5 }, // The Stand - Science Fiction
            new BookGenre { BookId = 33, GenreId = 4 }, // Death on the Nile - Mystery
            new BookGenre { BookId = 33, GenreId = 3 }, // Death on the Nile - Classic
            new BookGenre { BookId = 34, GenreId = 1 }, // Love in the Time of Cholera - Fiction
            new BookGenre { BookId = 34, GenreId = 8 }, // Love in the Time of Cholera - Romance
            new BookGenre { BookId = 35, GenreId = 2 }, // Prince Caspian - Fantasy
            new BookGenre { BookId = 35, GenreId = 3 }  // Prince Caspian - Classic
        );

        // NOTE: Customer seeding removed - now done in DatabaseSeeder
        // Customers are created with their associated ApplicationUser accounts
    }

    /// <summary>
    /// Override SaveChanges to automatically update UpdatedAt timestamps
    /// </summary>
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override SaveChangesAsync to automatically update UpdatedAt timestamps
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates the UpdatedAt property for modified entities
    /// </summary>
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified)
            .Where(e => e.Entity.GetType().GetProperty("UpdatedAt") != null);

        foreach (var entry in entries)
        {
            entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
        }
    }
}