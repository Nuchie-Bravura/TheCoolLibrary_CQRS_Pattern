using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolLibrary.Application.DTO.Author
{
    public class CreateAuthorRequestDTO
    {
        public string FullName { get; set; } = string.Empty;

        public string? Biography { get; set; }

        public string? Nationality { get; set; }
        public DateTime? BirthDate { get; set; }

        public IFormFile? PhotoFile { get; set; }
    }
}
