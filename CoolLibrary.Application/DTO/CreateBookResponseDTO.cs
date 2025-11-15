using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolLibrary.Application.DTO
{
    public class CreateBookResponseDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public IEnumerable<AuthorDTO> Authors { get; set; } = new List<AuthorDTO>();
         
        public IEnumerable<LinkBookDTO> Links { get; set; } = new List<LinkBookDTO>();
    }

    public class LinkBookDTO
    {
            public string Rel { get; set; } = string.Empty;
            public string Href { get; set; } = string.Empty; // URL
            public string Method { get; set; } = string.Empty;
    }
}
