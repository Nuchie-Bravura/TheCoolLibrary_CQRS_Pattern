using CoolLibrary.Domain.Contracts;
using Microsoft.Extensions.Logging;

public abstract class AuthorsBooksBaseService<T>
{
    protected readonly IArchiveStorage _archiveStorage;
    protected readonly ILogger<T> _logger;

    protected AuthorsBooksBaseService(IArchiveStorage archiveStorage, ILogger<T> logger)
    {
        _archiveStorage = archiveStorage;
        _logger = logger;
    }
}
