using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolLibrary.Domain.Contracts
{
    public interface IArchiveStorage
    {
        Task<string> StoreAsync(Stream fileStream, string fileName, string contentType);
        Task DeleteAsync(string fileUrl);

    }
}
