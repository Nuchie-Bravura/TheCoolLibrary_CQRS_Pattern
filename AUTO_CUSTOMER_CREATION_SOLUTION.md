# ? SOLUCIÓN: Crear Customer Automáticamente al Registrarse

## ?? Problema Identificado

**ANTES**:
```
Usuario se registra ? Solo se crea en AspNetUsers
                      ? NO se crea en Customers
```

**Resultado**: Usuarios sin perfil de Customer = No pueden hacer préstamos

---

## ? Solución Implementada

**AHORA**:
```
Usuario se registra ? 1. Se crea ApplicationUser (AspNetUsers)
                      2. Se asigna rol "User"
                      3. ? Se crea Customer automáticamente (vinculado a User)
```

**Resultado**: Cada usuario registrado tiene su perfil de Customer listo para usar

---

## ?? Archivos Modificados

### 1. **`RegisterDTO.cs`** - Agregados campos requeridos

**ANTES**:
```csharp
public class RegisterDTO
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}
```

**DESPUÉS**:
```csharp
public class RegisterDTO
{
    // ? NEW: Required fields
    [Required]
    public string FirstName { get; set; }
    
    [Required]
    public string LastName { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; }
    
    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
    
    // ? NEW: Optional fields for customer profile
    [Phone]
    public string? Phone { get; set; }
    
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
}
```

**Beneficios**:
- ? FirstName y LastName obligatorios
- ? Dirección, ciudad, código postal opcionales
- ? Validaciones automáticas con DataAnnotations

---

### 2. **`AuthController.cs`** - Crea Customer automáticamente

**ANTES** (Registro sin Customer):
```csharp
public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
{
    var newUser = new ApplicationUser
    {
        UserName = registerDto.Email,
        Email = registerDto.Email,
        EmailConfirmed = true
    };

    var result = await _userManager.CreateAsync(newUser, registerDto.Password);
    await _userManager.AddToRoleAsync(newUser, "User");

    return Ok(new { message = "User registered successfully" });
}
```

**DESPUÉS** (Registro CON Customer):
```csharp
public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
{
    // Step 1: Create ApplicationUser
    var newUser = new ApplicationUser
    {
        UserName = registerDto.Email,
        Email = registerDto.Email,
        EmailConfirmed = true,
        FirstName = registerDto.FirstName,   // ? NEW
        LastName = registerDto.LastName,     // ? NEW
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    var result = await _userManager.CreateAsync(newUser, registerDto.Password);
    await _userManager.AddToRoleAsync(newUser, "User");

    // Step 2: Create Customer profile automatically! ?
    var customer = new Customer
    {
        UserId = newUser.Id,  // Link to ApplicationUser
        Phone = registerDto.Phone,
        Address = registerDto.Address,
        City = registerDto.City,
        PostalCode = registerDto.PostalCode,
        MembershipDate = DateTime.UtcNow,
        MembershipStatus = MembershipStatus.Active,
        MaxBooksAllowed = 5,  // Default: 5 books
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    _dbContext.Customers.Add(customer);
    await _dbContext.SaveChangesAsync();

    return Ok(new 
    { 
        message = "User and customer profile created successfully",
        email = newUser.Email,
        customerId = customer.CustomerId,  // ? Return customer ID
        role = "User"
    });
}
```

**Cambios clave**:
1. ? Inyección de `LibraryDbContext` en el constructor
2. ? Crear `Customer` después de crear `ApplicationUser`
3. ? Vincular `Customer.UserId` con `ApplicationUser.Id`
4. ? Valores por defecto: `MembershipStatus.Active`, `MaxBooksAllowed = 5`
5. ? Retornar `customerId` en la respuesta

---

## ?? Cómo Probarlo

### **Registro de Nuevo Usuario**

**Request**:
```http
POST /api/v1/auth/register
Content-Type: application/json

{
  "firstName": "María",
  "lastName": "García",
  "email": "maria.garcia@email.com",
  "password": "Maria$123!",
  "confirmPassword": "Maria$123!",
  "phone": "+34-600-123456",
  "address": "Calle Mayor 10",
  "city": "Madrid",
  "postalCode": "28013"
}
```

**Response**:
```json
{
  "message": "User and customer profile created successfully",
  "email": "maria.garcia@email.com",
  "customerId": 3,
  "role": "User"
}
```

---

### **Verificar que se crearon ambos**

#### **1. Ver el ApplicationUser**
```sql
SELECT Id, UserName, Email, FirstName, LastName, EmailConfirmed
FROM AspNetUsers
WHERE Email = 'maria.garcia@email.com';
```

**Resultado esperado**:
```
Id: guid-xxx
UserName: maria.garcia@email.com
Email: maria.garcia@email.com
FirstName: María
LastName: García
EmailConfirmed: 1
```

---

#### **2. Ver el Customer vinculado**
```sql
SELECT c.CustomerId, c.UserId, u.FirstName, u.LastName, u.Email,
       c.Phone, c.Address, c.City, c.MembershipStatus, c.MaxBooksAllowed
FROM Customers c
INNER JOIN AspNetUsers u ON c.UserId = u.Id
WHERE u.Email = 'maria.garcia@email.com';
```

**Resultado esperado**:
```
CustomerId: 3
UserId: guid-xxx (mismo que AspNetUsers.Id)
FirstName: María (viene de ApplicationUser)
LastName: García (viene de ApplicationUser)
Email: maria.garcia@email.com (viene de ApplicationUser)
Phone: +34-600-123456
Address: Calle Mayor 10
City: Madrid
MembershipStatus: Active (1)
MaxBooksAllowed: 5
```

---

### **3. Login y obtener token**
```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "maria.garcia@email.com",
  "password": "Maria$123!"
}
```

**Response**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-01-16T15:30:00Z",
  "email": "maria.garcia@email.com",
  "roles": ["User"]
}
```

**Decodificar el token en jwt.io**:
```json
{
  "sub": "guid-xxx",
  "email": "maria.garcia@email.com",
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname": "María",
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname": "García",
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "User"
}
```

---

### **4. Verificar que el Customer puede hacer préstamos**
```http
GET /api/v1/customers
Authorization: Bearer {admin-token}
```

**Response** (fragmento):
```json
[
  {
    "customerId": 3,
    "fullName": "María García",
    "email": "maria.garcia@email.com",
    "phone": "+34-600-123456",
    "address": "Calle Mayor 10",
    "city": "Madrid",
    "postalCode": "28013",
    "membershipStatus": "Active",
    "membershipDate": "2024-01-16T10:30:00Z",
    "maxBooksAllowed": 5,
    "currentLoanCount": 0,
    "canBorrowMoreBooks": true
  }
]
```

---

## ?? Flujo Completo

```
???????????????????????????????????????????????????????????????????
?  1. Usuario llena formulario de registro                        ?
?     - FirstName, LastName, Email, Password, etc.                ?
???????????????????????????????????????????????????????????????????
                         ?
???????????????????????????????????????????????????????????????????
?  2. POST /api/v1/auth/register                                  ?
?     - Valida datos (DataAnnotations)                            ?
?     - Verifica que email no exista                              ?
???????????????????????????????????????????????????????????????????
                         ?
???????????????????????????????????????????????????????????????????
?  3. Crea ApplicationUser (AspNetUsers)                          ?
?     - UserName = Email                                          ?
?     - Email, FirstName, LastName                                ?
?     - Password (hashed automáticamente)                         ?
???????????????????????????????????????????????????????????????????
                         ?
???????????????????????????????????????????????????????????????????
?  4. Asigna rol "User"                                           ?
?     - Agrega a tabla AspNetUserRoles                            ?
???????????????????????????????????????????????????????????????????
                         ?
???????????????????????????????????????????????????????????????????
?  5. Crea Customer (Customers) ?                                ?
?     - UserId (FK a ApplicationUser)                             ?
?     - Phone, Address, City, PostalCode                          ?
?     - MembershipStatus = Active                                 ?
?     - MaxBooksAllowed = 5                                       ?
???????????????????????????????????????????????????????????????????
                         ?
???????????????????????????????????????????????????????????????????
?  6. Retorna respuesta exitosa                                   ?
?     - Email, CustomerId, Role                                   ?
???????????????????????????????????????????????????????????????????
                         ?
???????????????????????????????????????????????????????????????????
?  ? Usuario listo para:                                         ?
?     - Hacer login                                               ?
?     - Pedir libros prestados                                    ?
?     - Hacer reservaciones                                       ?
???????????????????????????????????????????????????????????????????
```

---

## ? Ventajas de esta Solución

| Aspecto | ANTES | AHORA |
|---------|-------|-------|
| Registro | Solo crea User | ? Crea User + Customer |
| Estado inicial | Usuario sin perfil | ? Usuario con perfil completo |
| Puede pedir libros | ? No (no tiene Customer) | ? Sí (tiene Customer) |
| Paso extra | Manual (Admin crea Customer) | ? Automático |
| Experiencia UX | Mala (usuario no puede usar el sistema) | ? Excelente (listo para usar) |
| Sincronización | Manual, propenso a errores | ? Automática y garantizada |

---

## ?? Campos Opcionales vs Requeridos

### **Requeridos**:
- ? `FirstName`
- ? `LastName`
- ? `Email`
- ? `Password`
- ? `ConfirmPassword`

### **Opcionales**:
- ? `Phone` (puede agregarse después)
- ? `Address` (puede agregarse después)
- ? `City` (puede agregarse después)
- ? `PostalCode` (puede agregarse después)

**¿Por qué opcionales?**
- No todos tienen dirección inmediatamente
- Pueden actualizarla más tarde vía `PATCH /api/v1/customers/{id}`
- Reduce fricción en el registro

---

## ?? Siguiente Paso: Actualizar el Frontend

Si tienes un frontend (Angular, React, etc.), actualiza el formulario de registro:

**ANTES**:
```html
<form>
  <input name="email" type="email" required>
  <input name="password" type="password" required>
  <input name="confirmPassword" type="password" required>
</form>
```

**AHORA**:
```html
<form>
  <input name="firstName" type="text" required>
  <input name="lastName" type="text" required>
  <input name="email" type="email" required>
  <input name="password" type="password" required>
  <input name="confirmPassword" type="password" required>
  
  <!-- Optional fields -->
  <input name="phone" type="tel">
  <input name="address" type="text">
  <input name="city" type="text">
  <input name="postalCode" type="text">
</form>
```

---

## ? Resumen

**Problema**: 
- ? Usuarios registrados no tenían perfil de Customer
- ? No podían pedir libros prestados

**Solución**:
- ? `RegisterDTO` actualizado con FirstName, LastName y campos de dirección
- ? `AuthController` crea Customer automáticamente al registrarse
- ? Customer vinculado a ApplicationUser vía `UserId`
- ? Valores por defecto: `MembershipStatus.Active`, `MaxBooksAllowed = 5`

**Resultado**:
- ? Usuarios listos para usar el sistema inmediatamente
- ? Experiencia de usuario mejorada
- ? Sincronización automática entre User y Customer
- ? Sin pasos manuales adicionales

**Estado**: ? **Implementado y compilando correctamente**

