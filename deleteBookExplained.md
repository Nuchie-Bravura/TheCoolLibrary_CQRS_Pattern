# Arquitectura del proyecto: Repository Pattern vs CQRS  caso DeleteBook

Ahora tengo este proyecto implementado de dos formas:

1. En formato **Repository Pattern**
2. En formato **CQRS**

---

## 🧱 Repository Pattern

En el formato Repository Pattern, los controladores inyectan directamente los **servicios de la capa de aplicación**.

Ejemplo en `BooksController`:

```csharp
public class BooksController : ControllerBase
{
    private readonly CreateBookService _createBookService;
    private readonly GetAllBooksService _getAllBooksService;    
    private readonly DeleteBookService _deleteBookService;

    public BooksController(
        CreateBookService createBookService,
        GetAllBooksService getAllBooksService,
        DeleteBookService deleteBookService)
    {
        _getAllBooksService = getAllBooksService;
        _deleteBookService = deleteBookService;
        _createBookService = createBookService;
    }

    [HttpDelete("{bookId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBook(int bookId)
    {
        await _deleteBookService.SafeBookDeleteAsync(bookId);
        return NoContent();
    }
}


// src\CoolLibrary.Infrastructure\Extensions\InfrastructureServiceExtensions.cs

public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services, 
    string connectionString, 
    IConfiguration configuration)
{
    // DbContext
    services.AddDbContext<LibraryDbContext>(options =>
        options.UseSqlServer(connectionString));

    // ✅ AQUÍ SE REGISTRAN LOS REPOSITORIOS
    services.AddScoped<IAuthors, AuthorsRepository>();
    services.AddScoped<IBooks, BooksRepository>();           // ← ¡ESTE!
    services.AddScoped<ICustomers, CustomersRepository>();
    services.AddScoped<ILoans, LoansRepository>();
    services.AddScoped<IArchiveStorage, AzureArchiveStorageRepository>(); // ← ¡ESTE!

    return services;
}
