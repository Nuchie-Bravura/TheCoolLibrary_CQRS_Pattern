using CoolLibrary.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolLibrary.Domain.Contracts
{
    public interface IAuthors
    {

        Task<IEnumerable<Author>> GetAllAsync();

        Task<IEnumerable<Author>> GetByNameAsync(string name);

        Task<int> SaveChangesAsync();
      
        Task<Author> InsertAsync(Author author);
        Task<Author> UpdateAsync(Author author);
        Task<Author?> PatchAsync(int authorId, Dictionary<string, object> updates);
        Task<Author?> GetByIdAsync(int authorId);



    }
}
