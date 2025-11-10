// ? CQRS PATTERN - EJEMPLO DE ESTRUCTURA

// ====================================================================
// 1??  COMMANDS (Escritura - Cambiar estado)
// ====================================================================

namespace CoolLibrary.Application.Commands;

/// <summary>
/// Command: Crear un nuevo cliente
/// Los Commands siempre causan cambios en el estado
/// </summary>
public class CreateCustomerCommand
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
}

/// <summary>
/// Command Handler: Ejecutar el comando CreateCustomer
/// </summary>
public class CreateCustomerCommandHandler
{
    private readonly LibraryDbContext _dbContext;
    private readonly ILogger<CreateCustomerCommandHandler> _logger;

    public CreateCustomerCommandHandler(
        LibraryDbContext dbContext,
        ILogger<CreateCustomerCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<int> Handle(CreateCustomerCommand command)
    {
        // Validaciones
        if (await _dbContext.Customers.AnyAsync(c => c.UserId == command.Email))
            throw new InvalidOperationException("Customer already exists");

        // Crear
        var customer = new Customer
        {
            Phone = command.Phone,
            Address = command.Address,
            City = command.City,
            PostalCode = command.PostalCode,
            MembershipDate = DateTime.UtcNow,
            MembershipStatus = MembershipStatus.Active,
            MaxBooksAllowed = 5,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("? Customer created: {CustomerId}", customer.CustomerId);
        return customer.CustomerId;
    }
}

// ====================================================================
// 2??  QUERIES (Lectura - NO cambiar estado)
// ====================================================================

namespace CoolLibrary.Application.Queries;

/// <summary>
/// Query: Obtener todos los clientes
/// Las Queries NUNCA cambian el estado
/// </summary>
public class GetAllCustomersQuery
{
}

/// <summary>
/// Query Handler: Ejecutar GetAllCustomersQuery
/// OPTIMIZADO para lectura - puede usar DTOs especializados
/// </summary>
public class GetAllCustomersQueryHandler
{
    private readonly LibraryDbContext _dbContext;
    private readonly ILogger<GetAllCustomersQueryHandler> _logger;

    public GetAllCustomersQueryHandler(
        LibraryDbContext dbContext,
        ILogger<GetAllCustomersQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<List<CustomerReadDto>> Handle(GetAllCustomersQuery query)
    {
        // ?? VENTAJA CQRS: Aquí podemos optimizar para lectura
        // - Usar índices específicos
        // - Hacer joins optimizados
        // - Usar proyecciones eficientes
        // - Incluso leer de una BASE DE DATOS SEPARADA si es necesario

        var customers = await _dbContext.Customers
            .AsNoTracking()  // ? No track para lectura (más rápido)
            .Select(c => new CustomerReadDto
            {
                CustomerId = c.CustomerId,
                Phone = c.Phone,
                Address = c.Address,
                City = c.City,
                MembershipStatus = c.MembershipStatus,
                MaxBooksAllowed = c.MaxBooksAllowed
            })
            .ToListAsync();

        _logger.LogInformation("?? Retrieved {Count} customers", customers.Count);
        return customers;
    }
}

/// <summary>
/// Query: Obtener un cliente por ID
/// </summary>
public class GetCustomerByIdQuery
{
    public int CustomerId { get; set; }
}

public class GetCustomerByIdQueryHandler
{
    private readonly LibraryDbContext _dbContext;

    public GetCustomerByIdQueryHandler(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CustomerReadDto> Handle(GetCustomerByIdQuery query)
    {
        var customer = await _dbContext.Customers
            .AsNoTracking()
            .Where(c => c.CustomerId == query.CustomerId)
            .Select(c => new CustomerReadDto
            {
                CustomerId = c.CustomerId,
                Phone = c.Phone,
                Address = c.Address,
                City = c.City
            })
            .FirstOrDefaultAsync();

        if (customer == null)
            throw new KeyNotFoundException($"Customer {query.CustomerId} not found");

        return customer;
    }
}

// ====================================================================
// 3??  DTOs ESPECIALIZADOS (CQRS permite DTOs diferentes)
// ====================================================================

namespace CoolLibrary.Application.DTOs;

/// <summary>
/// DTO para LECTURA (simplificado, solo lo necesario)
/// </summary>
public class CustomerReadDto
{
    public int CustomerId { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public MembershipStatus MembershipStatus { get; set; }
    public int MaxBooksAllowed { get; set; }
}

/// <summary>
/// DTO para ESCRITURA (incluye validaciones)
/// </summary>
public class CustomerWriteDto
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    [Phone]
    public string Phone { get; set; }
}

// ====================================================================
// 4??  CONTROLLER CON CQRS (vs Repository)
// ====================================================================

namespace CoolLibrary.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class CustomersControllerCQRS : ControllerBase
{
    // ? REPOSITORY PATTERN
    // private readonly ICustomers _repository;
    // public async Task<IActionResult> GetAll()
    // {
    //     var customers = await _repository.GetAllAsync();
    //     return Ok(customers);
    // }

    // ? CQRS PATTERN
    private readonly ICommandBus _commandBus;
    private readonly IQueryBus _queryBus;

    public CustomersControllerCQRS(
        ICommandBus commandBus,
        IQueryBus queryBus)
    {
        _commandBus = commandBus;
        _queryBus = queryBus;
    }

    // LECTURA
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _queryBus.Execute(new GetAllCustomersQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _queryBus.Execute(new GetCustomerByIdQuery { CustomerId = id });
        return Ok(result);
    }

    // ESCRITURA
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command)
    {
        var customerId = await _commandBus.Execute(command);
        return CreatedAtAction(nameof(GetById), new { id = customerId }, customerId);
    }
}

// ====================================================================
// 5??  BUSES (Necesarios para CQRS)
// ====================================================================

namespace CoolLibrary.Application.Bus;

/// <summary>
/// Command Bus: Ejecuta Commands
/// Cada command tiene exactamente un handler
/// </summary>
public interface ICommandBus
{
    Task<TResult> Execute<TResult>(ICommand<TResult> command);
}

public class CommandBus : ICommandBus
{
    private readonly IServiceProvider _serviceProvider;

    public CommandBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResult> Execute<TResult>(ICommand<TResult> command)
    {
        var handlerType = typeof(ICommandHandler<,>)
            .MakeGenericType(command.GetType(), typeof(TResult));

        var handler = _serviceProvider.GetService(handlerType);
        if (handler == null)
            throw new InvalidOperationException($"No handler for {command.GetType().Name}");

        var method = handlerType.GetMethod("Handle");
        return (TResult)await (dynamic)method.Invoke(handler, new object[] { command });
    }
}

/// <summary>
/// Query Bus: Ejecuta Queries
/// Cada query tiene exactamente un handler
/// </summary>
public interface IQueryBus
{
    Task<TResult> Execute<TResult>(IQuery<TResult> query);
}

public class QueryBus : IQueryBus
{
    private readonly IServiceProvider _serviceProvider;

    public QueryBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResult> Execute<TResult>(IQuery<TResult> query)
    {
        var handlerType = typeof(IQueryHandler<,>)
            .MakeGenericType(query.GetType(), typeof(TResult));

        var handler = _serviceProvider.GetService(handlerType);
        if (handler == null)
            throw new InvalidOperationException($"No handler for {query.GetType().Name}");

        var method = handlerType.GetMethod("Handle");
        return (TResult)await (dynamic)method.Invoke(handler, new object[] { query });
    }
}

// Interfaces
public interface ICommand<TResult> { }
public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<TResult> Handle(TCommand command);
}

public interface IQuery<TResult> { }
public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<TResult> Handle(TQuery query);
}

// ====================================================================
// 6??  REGISTRAR EN PROGRAM.CS (Dependency Injection)
// ====================================================================

// En Program.cs:
/*
// CQRS Setup
builder.Services.AddScoped<ICommandBus, CommandBus>();
builder.Services.AddScoped<IQueryBus, QueryBus>();

// Register Command Handlers
builder.Services.AddScoped<ICommandHandler<CreateCustomerCommand, int>, CreateCustomerCommandHandler>();

// Register Query Handlers
builder.Services.AddScoped<IQueryHandler<GetAllCustomersQuery, List<CustomerReadDto>>, GetAllCustomersQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetCustomerByIdQuery, CustomerReadDto>, GetCustomerByIdQueryHandler>();
*/
