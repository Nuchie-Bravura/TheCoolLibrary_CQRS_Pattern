using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolLibrary.Application.DTO.Book
{

    /// <summary>
    /// Data Transfer Object for a Book.
    /// </summary>
    public class BookDTO
    {
        /// <summary>
        /// Unique identifier for the book.
        /// </summary>
        public int BookId { get; set; }

        /// <summary>
        /// The title of the book.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? CoverPhotoURL { get; set; }

        /// <summary>
        /// International Standard Book Number.
        /// </summary>
        public string ISBN { get; set; } = string.Empty;

        /// <summary>
        /// The publisher of the book.
        /// </summary>
        public string? Publisher { get; set; }

        /// <summary>
        /// The date the book was published.
        /// </summary>
        public DateTime? PublicationDate { get; set; }

        /// <summary>
        /// Indicates if the book is currently available for loan.
        /// </summary>
        public bool IsAvailable { get; set; }
    }
}
