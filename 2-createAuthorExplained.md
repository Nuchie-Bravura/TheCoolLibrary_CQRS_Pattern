````md
# Add New Author

## En el patrón repositorio

En el patrón repositorio tengo un controlador donde inyecto todos los servicios que necesito.

Por ejemplo, como un caso de uso es crear un nuevo autor, lo que hago es crear en la capa de aplicación un servicio llamado `CreateAuthorService`.

Es en este servicio donde voy a meter toda la lógica y donde voy a usar los repositorios de la capa de infraestructura.

---

## Inyección de repositorios desde infraestructura

En la capa API, en `Program.cs`, inyecto desde la capa de infraestructura.  
Esto sirve también para CQRS:

```csharp
services.AddScoped<IAuthors, AuthorsRepository>();
services.AddScoped<IBooks, BooksRepository>();
services.AddScoped<ICustomers, CustomersRepository>();
services.AddScoped<ILoans, LoansRepository>();
services.AddScoped<IArchiveStorage, AzureArchiveStorageRepository>();
````

---

## Inyección de servicios desde la capa de aplicación

```csharp
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
}
```

---

## Controlador

```csharp
public class AuthorsController : ControllerBase
{
    private readonly CreateAuthorService _createAuthorService;
    private readonly GetAllAuthorsService _getAllAuthorsService;
    private readonly DeleteAuthorService _deleteAuthorService;

    public AuthorsController(
        CreateAuthorService createAuthorService,
        GetAllAuthorsService getAllAuthorsService,
        DeleteAuthorService deleteAuthorService)
    {
        _createAuthorService = createAuthorService;
        _getAllAuthorsService = getAllAuthorsService;
        _deleteAuthorService = deleteAuthorService;
    }
}
```

---

Entonces, desde el servicio en la capa de aplicación, cuando hago:

```csharp
public class CreateAuthorService
{
    private readonly IAuthors _authorsRepository;
    private readonly IArchiveStorage _archiveStorage;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateAuthorService> _logger;

    public CreateAuthorService(
        IAuthors authorsRepository,
        IArchiveStorage archiveStorage,
        IMapper mapper,
        ILogger<CreateAuthorService> logger)
    {
        _authorsRepository = authorsRepository;
        _archiveStorage = archiveStorage;
        _mapper = mapper;
        _logger = logger;
    }
}
```

ya sabemos que ese `_authorsRepository` es del tipo `AuthorsRepository`.

---

Entonces, en el controlador esto debe ser un recurso o acción, y debe estar dentro del `AuthorsController`.

El recurso admite un DTO, un DTO de entrada definido en la capa de aplicación:

```csharp
[HttpPost("AddNewAuthor")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> AddNewAuthor([FromForm] CreateAuthorRequestDTO createAuthorRequestDTO)
{
    var createdAuthor = await _createAuthorService.ExecuteAsync(createAuthorRequestDTO);
    return CreatedAtAction(
        nameof(GetAllAuthorsWithBooks),
        new { id = createdAuthor.AuthorId, version = "1.0" },
        createdAuthor);
}
```

Entiendo que lo necesito porque es como un contrato que defino yo y que el usuario debe seguir.
Este es el formato de entrada, y yo daré otro formato de salida, también con un DTO.

---

La creación del autor está en el servicio `CreateAuthorService` y en el método `ExecuteAsync`, y hace cinco cosas:

1. Primero mapea entre la entidad `Author` y el DTO de entrada que recibe.
2. Si viene con foto, la tiene que guardar en Azure Blob.
3. Inserta con LINQ el autor en la tabla `Author` de la base de datos.
4. Crea la respuesta, que es el DTO, con `IMapper`.
5. Logea todo con `ILogger`.

---

## En CQRS esto es distinto

Como ya sabemos, en el `Program.cs` no vamos a inyectar los servicios, solo MediatR, y en este caso el `TokenService` porque era laborioso hacerlo y se salía del objetivo del ejercicio.

```csharp
public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
    // Application Services
    services.AddScoped<TokenService>();

    // MediatR
    services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(ApplicationServiceExtensions).Assembly));

    // AutoMapper
    services.AddAutoMapper(typeof(MappingProfile).Assembly);

    return services;
}
```

---

En el controlador no inyectamos los servicios, solo el mediador:

```csharp
private readonly IMediator _mediator;

public AuthorsController(IMediator mediator)
{
    _mediator = mediator;
}
```

---

En el recurso, solo usamos el `Send` del mediador, con su DTO de entrada por supuesto, y su `CreatedAtAction` y su `nameof` para el HATEOAS del DTO de respuesta:

```csharp
[HttpPost("AddNewAuthor")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> AddNewAuthor([FromForm] CreateAuthorRequestDTO createAuthorRequestDTO)
{
    var createdAuthor = await _mediator.Send(new CreateAuthorCommand(createAuthorRequestDTO));
    return CreatedAtAction(
        nameof(GetAllAuthorsWithBooks),
        new { id = createdAuthor.AuthorId, version = "1.0" },
        createdAuthor);
}
```

---

El mediator sabe que cuando ocurre el `Send` debe lanzar el handler, que funciona exactamente igual que el servicio anterior, pero ahora el método que hace los cinco pasos anteriores no se llama `ExecuteAsync`, se llama `Handle`.

Y dentro del `Handle` se hace exactamente lo mismo:

1. Primero mapea entre la entidad `Author` y el DTO de entrada que recibe.
2. Si viene con foto, la tiene que guardar en Azure Blob.
3. Inserta con LINQ el autor en la tabla `Author` de la base de datos.
4. Crea la respuesta, que es el DTO, con `IMapper`.
5. Logea todo con `ILogger`.

La lógica es la misma.
Lo único que cambia es dónde vive:

* Antes: en un **servicio** (`CreateAuthorService`).
* Ahora: en un **handler** (`CreateAuthorCommandHandler`).

```
```
