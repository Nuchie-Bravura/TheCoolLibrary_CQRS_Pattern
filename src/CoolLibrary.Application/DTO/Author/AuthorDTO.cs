using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolLibrary.Application.DTO.Author
{
   
    public class AuthorDTO
    {
      
        public int AuthorId { get; set; }

     
        public string FullName { get; set; } = string.Empty;

   
        public string? Biography { get; set; }

   
        public string? Nationality { get; set; }


        public List<AuthorBookDTO> Books { get; set; } = new();

    }
}
