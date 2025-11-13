using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolLibrary.Application.DTO
{    
    public class CreateBookRequestDTO
    {

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public IFormFile? CoverPhotoURL { get; set; }

        public string ISBN { get; set; } = string.Empty;

        public string? Publisher {get; set; }
        
        public DateTime? PublicationDate { get; set; }

        public int PageCount { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }

        /// <summary>
        /// Language of the book (default: "English")
        /// </summary>
        public string? Language { get; set; }

        /// <summary>
        /// List of Author IDs for this book
        /// </summary>
        public IEnumerable<int> Authors { get; set; } = new List<int>();
    }
}



