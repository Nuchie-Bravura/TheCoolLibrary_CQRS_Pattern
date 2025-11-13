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
         
        public IEnumerable<LinkDTO> Links { get; set; } = new List<LinkDTO>();
    }
}   
