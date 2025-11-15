using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolLibrary.Application.DTO.Author
{
    public class CreateAuthorResponseDTO
    {
        public int AuthorId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public IEnumerable<LinkDTO> Links { get; set; } = new List<LinkDTO>();
    }

    public class LinkDTO
    {
        public string Rel { get; set; } = string.Empty;  
        public string Href { get; set; } = string.Empty; // URL
        public string Method { get; set; } = string.Empty; 
    }
}
