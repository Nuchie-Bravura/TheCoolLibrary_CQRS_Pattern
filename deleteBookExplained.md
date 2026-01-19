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

En Program.cs se inyectan todos los servicios de la capa de aplicación, que están organizados por entidades dentro de una carpeta Services:

public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
    // Application Services
    services.AddScoped<LoanRequestService>();
    services.AddScoped<TokenService>();
    services.AddScoped<GetAllAuthorsService>();
    services.AddScoped<CreateAuthorService>();
    services.AddScoped<DeleteAuthorService>();  
    services.AddScoped<CreateBookService>();
    services.AddScoped<GetAllBooksService>();
    services.AddScoped<DeleteBookService>();
    //services.AddScoped<LoanApprovalService>();
    //services.AddScoped<ReservationService>();
    //services.AddScoped<ReturnLoanService>();
    //services.AddScoped<GetUserLoansService>();

    // AutoMapper
    services.AddAutoMapper(typeof(MappingProfile).Assembly);

    return services;
}


El controlador depende de los servicios de la capa de aplicación, y estos servicios a su vez dependen de los repositorios, que implementan las interfaces definidas en la capa de dominio.

Por ejemplo, el servicio DeleteBookService recibe los repositorios e infraestructuras necesarias:

public DeleteBookService(
    IBooks booksRepository,
    IArchiveStorage archiveStorage,
    ILogger<DeleteBookService> logger)
{
    _booksRepository = booksRepository;
    _archiveStorage = archiveStorage;
    _logger = logger;
}


Cuando quiero un recurso para borrar un libro, creo el servicio DeleteBookService, lo registro en Program.cs y luego lo inyecto en el controlador.

En este servicio se aplica toda la lógica de negocio:

Encontrar el libro por su Id.

Ver si tiene una imagen guardada en Azure Blob Storage y borrarla.

Logear todo el proceso.

Por tanto, el servicio es donde vive la lógica de negocio, y donde se usan los repositorios necesarios.

🔀 CQRS

En CQRS, en la capa API no se inyectan servicios de aplicación, se inyecta únicamente el Mediator.

En lugar de registrar muchos servicios, solo registro MediatR:

public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
    // Application Services
    services.AddScoped<TokenService>(); // Este lo dejo por comodidad

    // MediatR
    services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(ApplicationServiceExtensions).Assembly));

    // AutoMapper
    services.AddAutoMapper(typeof(MappingProfile).Assembly);

    return services;
}


En el controlador desaparecen los servicios de aplicación y solo queda el IMediator:

private readonly IMediator _mediator;

public BooksController(IMediator mediator)
{
    _mediator = mediator;
}


Y el endpoint queda así:

[HttpDelete("{bookId}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> DeleteBook(int bookId)
{
    await _mediator.Send(new DeleteBookCommand(bookId));
    return NoContent();
}

🧠 Diferencia clave

Antes, con Repository Pattern:

El controlador llamaba a un servicio:
DeleteBookService

El servicio:

Inyectaba los repositorios.

Ejecutaba la lógica en métodos como SafeBookDeleteAsync.

Usaba interfaces de dominio (IBooks, IArchiveStorage, etc.).

Ahora, con CQRS:

El controlador no conoce servicios concretos.

Solo llama a:

_mediator.Send(new DeleteBookCommand(bookId));


MediatR:

Recibe el DeleteBookCommand.

Busca su Handler correspondiente.

Ejecuta el método Handle.

Y ese Handler tiene exactamente la misma responsabilidad que antes tenía DeleteBookService:

Encontrar el libro.

Borrar la imagen de Azure Blob Storage.

Logear el proceso.

Usar los repositorios que implementan las interfaces de la capa de dominio.

Pero ahora:

El borrado no se hace en un método tipo
DeleteBookService.SafeDeleteAsync(),
sino directamente en el método Handle() del DeleteBookCommandHandler.

📌 Resumen rápido
Repository Pattern	CQRS
Controlador → Servicio	Controlador → Mediator
Servicio contiene la lógica	Handler contiene la lógica
Muchos servicios registrados	Solo MediatR registrado
Llamada directa a métodos	Envío de comandos/queries
Estructura más simple	Más desacoplada y escalable

Ambos modelos usan:

Repositorios.

Interfaces de dominio.

Infraestructura.

AutoMapper.

Clean Architecture.

La diferencia es cómo orquestas la ejecución de los casos de uso:

Con servicios directos (Repository Pattern).

Con comandos, queries y handlers (CQRS + MediatR).
