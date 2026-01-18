using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CoolLibrary.API.DTOs
{
    /// <summary>
    /// Request DTO for creating an author with photo
    /// This belongs to the API layer because it contains IFormFile
    /// </summary>
    public class CreateAuthorRequest
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? Biography { get; set; }

        public string? Nationality { get; set; }
        
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Photo file to upload (optional)
        /// </summary>
        public IFormFile? PhotoFile { get; set; }
    }
}
