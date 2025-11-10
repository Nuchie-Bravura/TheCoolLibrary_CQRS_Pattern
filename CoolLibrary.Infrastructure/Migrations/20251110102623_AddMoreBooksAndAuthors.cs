using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CoolLibrary.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreBooksAndAuthors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "AuthorId", "Biography", "BirthDate", "CreatedAt", "FirstName", "LastName", "Nationality", "UpdatedAt" },
                values: new object[,]
                {
                    { 4, "Colombian novelist and Nobel Prize winner", new DateTime(1927, 3, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gabriel", "García Márquez", "Colombian", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, "American author of horror, supernatural fiction, suspense, crime, science-fiction, and fantasy novels", new DateTime(1947, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Stephen", "King", "American", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, "English writer known for her detective novels", new DateTime(1890, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Agatha", "Christie", "British", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, "English writer, poet, philologist, and academic, best known as the author of The Lord of the Rings", new DateTime(1892, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "J.R.R.", "Tolkien", "British", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, "American author best known for his thriller novels", new DateTime(1964, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dan", "Brown", "American", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, "American writer and professor of biochemistry, best known for his works of science fiction", new DateTime(1920, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Isaac", "Asimov", "American", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, "Russian writer who is regarded as one of the greatest authors of all time", new DateTime(1828, 9, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Leo", "Tolstoy", "Russian", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, "American novelist, essayist, and short story writer", new DateTime(1896, 9, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "F. Scott", "Fitzgerald", "American", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, "American novelist best known for To Kill a Mockingbird", new DateTime(1926, 4, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Harper", "Lee", "American", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, "American novelist, short-story writer, and journalist", new DateTime(1899, 7, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ernest", "Hemingway", "American", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, "Brazilian lyricist and novelist, best known for The Alchemist", new DateTime(1947, 8, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Paulo", "Coelho", "Brazilian", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, "Canadian poet, novelist, literary critic, essayist, and environmental activist", new DateTime(1939, 11, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Margaret", "Atwood", "Canadian", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, "British writer and physician, best known for creating Sherlock Holmes", new DateTime(1859, 5, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Arthur", "Conan Doyle", "British", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, "French poet, novelist, and dramatist of the Romantic movement", new DateTime(1802, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Victor", "Hugo", "French", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, "American author and screenwriter, one of the most celebrated 20th-century American writers", new DateTime(1920, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ray", "Bradbury", "American", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 19, "British writer and lay theologian, best known for The Chronicles of Narnia", new DateTime(1898, 11, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "C.S.", "Lewis", "British", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 20, "English writer and philosopher, best known for Brave New World", new DateTime(1894, 7, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Aldous", "Huxley", "British", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookId", "AvailableCopies", "CreatedAt", "Description", "ISBN", "Language", "PageCount", "PublicationDate", "Publisher", "Title", "TotalCopies", "UpdatedAt" },
                values: new object[,]
                {
                    { 6, 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "The multi-generational story of the Buendía family", "978-0-06-088328-7", "English", 417, new DateTime(1967, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Harper & Row", "One Hundred Years of Solitude", 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A family heads to an isolated hotel for the winter", "978-0-385-33312-0", "English", 447, new DateTime(1977, 1, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Doubleday", "The Shining", 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ten strangers are lured to an isolated island mansion", "978-0-06-207348-4", "English", 272, new DateTime(1939, 11, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Collins Crime Club", "And Then There Were None", 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "An epic high-fantasy novel", "978-0-618-00222-1", "English", 1178, new DateTime(1954, 7, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Allen & Unwin", "The Lord of the Rings", 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A mystery thriller novel", "978-0-307-47463-1", "English", 454, new DateTime(2003, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Doubleday", "The Da Vinci Code", 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A science fiction novel about the fall and rise of civilizations", "978-0-553-29335-0", "English", 255, new DateTime(1951, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gnome Press", "Foundation", 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A novel that chronicles the French invasion of Russia", "978-1-4000-7954-0", "English", 1225, new DateTime(1869, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Russian Messenger", "War and Peace", 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A novel about the American Dream in the Jazz Age", "978-0-7432-7356-5", "English", 180, new DateTime(1925, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Charles Scribner's Sons", "The Great Gatsby", 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A novel about racial injustice and childhood innocence", "978-0-06-112008-4", "English", 324, new DateTime(1960, 7, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "J.B. Lippincott & Co.", "To Kill a Mockingbird", 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A short novel about an aging Cuban fisherman", "978-0-684-80122-3", "English", 127, new DateTime(1952, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Charles Scribner's Sons", "The Old Man and the Sea", 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A novel about a young Andalusian shepherd's journey to Egypt", "978-0-06-112241-5", "English", 197, new DateTime(1988, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "HarperTorch", "The Alchemist", 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A dystopian novel set in a totalitarian society", "978-0-385-49081-8", "English", 311, new DateTime(1985, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "McClelland and Stewart", "The Handmaid's Tale", 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Collection of twelve short stories featuring Sherlock Holmes", "978-0-14-017737-7", "English", 307, new DateTime(1892, 10, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "George Newnes", "The Adventures of Sherlock Holmes", 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 19, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A French historical novel", "978-0-451-41943-5", "English", 1463, new DateTime(1862, 4, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "A. Lacroix, Verboeckhoven & Cie", "Les Misérables", 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 20, 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A dystopian novel about a future American society where books are outlawed", "978-1-4516-7331-9", "English", 249, new DateTime(1953, 10, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ballantine Books", "Fahrenheit 451", 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 21, 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "First published book in The Chronicles of Narnia series", "978-0-06-112979-7", "English", 206, new DateTime(1950, 10, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Geoffrey Bles", "The Lion, the Witch and the Wardrobe", 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 22, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A dystopian novel set in a futuristic World State", "978-0-06-085052-4", "English", 311, new DateTime(1932, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chatto & Windus", "Brave New World", 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 23, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A mystery thriller novel", "978-0-307-27778-7", "English", 616, new DateTime(2000, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pocket Books", "Angels & Demons", 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 24, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A horror novel about seven children terrorized by an evil entity", "978-0-345-34296-8", "English", 1138, new DateTime(1986, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Viking", "It", 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 25, 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A detective novel featuring Hercule Poirot", "978-0-06-093546-7", "English", 256, new DateTime(1934, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Collins Crime Club", "Murder on the Orient Express", 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 26, 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A fantasy novel and children's book", "978-0-618-57422-7", "English", 310, new DateTime(1937, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "George Allen & Unwin", "The Hobbit", 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 27, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A novel about a tragic love affair", "978-0-14-028329-5", "English", 864, new DateTime(1878, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Russian Messenger", "Anna Karenina", 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 28, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A novel about the Spanish Civil War", "978-0-7432-7357-2", "English", 471, new DateTime(1940, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Charles Scribner's Sons", "For Whom the Bell Tolls", 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 29, 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A collection of science fiction short stories", "978-0-553-38034-9", "English", 224, new DateTime(1950, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gnome Press", "I, Robot", 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 30, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A novel about the lives and loves of the Dashwood sisters", "978-0-14-303943-3", "English", 409, new DateTime(1811, 10, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Thomas Egerton", "Sense and Sensibility", 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 31, 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "The second novel in the Harry Potter series", "978-0-439-13959-0", "English", 251, new DateTime(1998, 7, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bloomsbury", "Harry Potter and the Chamber of Secrets", 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 32, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A post-apocalyptic dark fantasy novel", "978-0-385-33313-7", "English", 823, new DateTime(1978, 10, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Doubleday", "The Stand", 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 33, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A detective novel featuring Hercule Poirot", "978-0-06-093547-4", "English", 288, new DateTime(1937, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Collins Crime Club", "Death on the Nile", 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 34, 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A novel about love, aging, illness, and death", "978-0-06-088329-4", "English", 348, new DateTime(1985, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Editorial Oveja Negra", "Love in the Time of Cholera", 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 35, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "The second published book in The Chronicles of Narnia series", "978-0-06-112980-3", "English", 223, new DateTime(1951, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Geoffrey Bles", "Prince Caspian", 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "GenreId", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fiction dealing with the solution of a crime or the unraveling of secrets", "Mystery", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fiction based on imagined future scientific or technological advances", "Science Fiction", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fiction intended to scare, unsettle, or horrify the reader", "Horror", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fiction characterized by fast pacing, frequent action, and resourceful heroes", "Thriller", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fiction focusing on romantic love and relationships", "Romance", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "BookAuthors",
                columns: new[] { "AuthorId", "BookId", "AuthorOrder" },
                values: new object[,]
                {
                    { 4, 6, 1 },
                    { 5, 7, 1 },
                    { 6, 8, 1 },
                    { 7, 9, 1 },
                    { 8, 10, 1 },
                    { 9, 11, 1 },
                    { 10, 12, 1 },
                    { 11, 13, 1 },
                    { 12, 14, 1 },
                    { 13, 15, 1 },
                    { 14, 16, 1 },
                    { 15, 17, 1 },
                    { 16, 18, 1 },
                    { 17, 19, 1 },
                    { 18, 20, 1 },
                    { 19, 21, 1 },
                    { 20, 22, 1 },
                    { 8, 23, 1 },
                    { 5, 24, 1 },
                    { 6, 25, 1 },
                    { 7, 26, 1 },
                    { 10, 27, 1 },
                    { 13, 28, 1 },
                    { 9, 29, 1 },
                    { 2, 30, 1 },
                    { 3, 31, 1 },
                    { 5, 32, 1 },
                    { 6, 33, 1 },
                    { 4, 34, 1 },
                    { 19, 35, 1 }
                });

            migrationBuilder.InsertData(
                table: "BookGenres",
                columns: new[] { "BookId", "GenreId" },
                values: new object[,]
                {
                    { 6, 1 },
                    { 6, 3 },
                    { 7, 6 },
                    { 8, 3 },
                    { 8, 4 },
                    { 9, 2 },
                    { 9, 3 },
                    { 10, 4 },
                    { 10, 7 },
                    { 11, 3 },
                    { 11, 5 },
                    { 12, 1 },
                    { 12, 3 },
                    { 13, 1 },
                    { 13, 3 },
                    { 14, 1 },
                    { 14, 3 },
                    { 15, 1 },
                    { 15, 3 },
                    { 16, 1 },
                    { 17, 1 },
                    { 17, 5 },
                    { 18, 3 },
                    { 18, 4 },
                    { 19, 1 },
                    { 19, 3 },
                    { 20, 3 },
                    { 20, 5 },
                    { 21, 2 },
                    { 21, 3 },
                    { 22, 3 },
                    { 22, 5 },
                    { 23, 4 },
                    { 23, 7 },
                    { 24, 6 },
                    { 25, 3 },
                    { 25, 4 },
                    { 26, 2 },
                    { 26, 3 },
                    { 27, 1 },
                    { 27, 3 },
                    { 27, 8 },
                    { 28, 1 },
                    { 28, 3 },
                    { 29, 3 },
                    { 29, 5 },
                    { 30, 1 },
                    { 30, 3 },
                    { 30, 8 },
                    { 31, 1 },
                    { 31, 2 },
                    { 32, 5 },
                    { 32, 6 },
                    { 33, 3 },
                    { 33, 4 },
                    { 34, 1 },
                    { 34, 8 },
                    { 35, 2 },
                    { 35, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 4, 6 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 5, 7 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 6, 8 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 7, 9 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 8, 10 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 9, 11 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 10, 12 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 11, 13 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 12, 14 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 13, 15 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 14, 16 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 15, 17 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 16, 18 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 17, 19 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 18, 20 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 19, 21 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 20, 22 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 8, 23 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 5, 24 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 6, 25 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 7, 26 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 10, 27 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 13, 28 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 9, 29 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 2, 30 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 3, 31 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 5, 32 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 6, 33 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 4, 34 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 19, 35 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 6, 1 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 6, 3 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 7, 6 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 8, 3 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 8, 4 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 9, 2 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 9, 3 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 10, 4 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 10, 7 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 11, 3 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 11, 5 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 12, 1 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 12, 3 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 13, 1 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 13, 3 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 14, 1 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 14, 3 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 15, 1 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 15, 3 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 16, 1 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 17, 1 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 17, 5 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 18, 3 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 18, 4 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 19, 1 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 19, 3 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 20, 3 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 20, 5 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 21, 2 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 21, 3 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 22, 3 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 22, 5 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 23, 4 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 23, 7 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 24, 6 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 25, 3 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 25, 4 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 26, 2 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 26, 3 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 27, 1 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 27, 3 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 27, 8 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 28, 1 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 28, 3 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 29, 3 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 29, 5 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 30, 1 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 30, 3 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 30, 8 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 31, 1 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 31, 2 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 32, 5 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 32, 6 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 33, 3 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 33, 4 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 34, 1 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 34, 8 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 35, 2 });

            migrationBuilder.DeleteData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 35, 3 });

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "GenreId",
                keyValue: 8);
        }
    }
}
