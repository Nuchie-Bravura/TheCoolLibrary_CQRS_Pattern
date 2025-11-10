# ? ApplicationUser & Customer Refactoring - Implementation Complete

## ?? Problem Solved

**Before**: Duplicate data between `AspNetUsers` (Identity) and `Customers` table
**After**: Clean 1-to-1 relationship with proper separation of concerns

---

## ?? Architecture Changes

### **BEFORE (Incorrect)**
```
AspNetUsers (Identity)          Customers (Domain)
??? Email                        ??? Email ? DUPLICATED
??? UserName                     ??? FirstName ? DUPLICATED
??? Password                     ??? LastName ? DUPLICATED
??? Roles                        ??? MembershipDate
                                 ??? MaxBooksAllowed
                                 ??? Loans
```

**Problems**:
- ? Data duplication
- ? Sync issues between tables
- ? No relationship between User and Customer

---

### **AFTER (Correct - Option 1)**
```
ApplicationUser (extends IdentityUser)    Customer (Domain Entity)
??? Id (PK)                              ??? CustomerId (PK)
??? Email                                ??? UserId (FK) ???
??? UserName                             ?                  ?
??? Password (hashed)                    ?    1:1           ?
??? Roles                                ?  Relationship    ?
??? FirstName ? NEW                     ?                  ?
??? LastName ? NEW                      ????????????????????
??? CreatedAt ? NEW                     ??? Phone
??? UpdatedAt ? NEW                     ??? Address
??? Customer (navigation) ?            ??? City
                                         ??? PostalCode
                                         ??? MembershipDate
                                         ??? MembershipStatus
                                         ??? MaxBooksAllowed
                                         ??? Loans
                                         ??? Reservations
                                         ??? Fines
```

**Benefits**:
- ? No data duplication
- ? Single source of truth for user info
- ? Clear separation: Identity = Auth, Customer = Business
- ? Not all users need to be customers
- ? Admin users don't need customer profiles

---

## ?? Files Created/Modified

### **NEW FILES**

1. ? **`CoolLibrary.Domain/Entities/ApplicationUser.cs`**
   - Extends `IdentityUser`
   - Adds custom properties: `FirstName`, `LastName`, `CreatedAt`, `UpdatedAt`
   - Navigation property to `Customer` (optional 1-to-1)

2. ? **`CoolLibrary.Infrastructure/Data/Configurations/ApplicationUserConfiguration.cs`**
   - EF Core configuration for `ApplicationUser`
   - Configures 1-to-1 relationship with `Customer`
   - Indexes on `LastName` and `FirstName`

---

### **MODIFIED FILES**

#### **Domain Layer**

1. ? **`CoolLibrary.Domain/Entities/Customer.cs`**
   - **Removed**: `FirstName`, `LastName`, `Email` (now in `ApplicationUser`)
   - **Added**: `UserId` (FK), `User` (navigation property)
   - **Added**: Computed properties `FullName` and `Email` (delegate to `User`)

2. ? **`CoolLibrary.Domain/CoolLibrary.Domain.csproj`**
   - **Added**: `Microsoft.Extensions.Identity.Stores` package reference

---

#### **Infrastructure Layer**

3. ? **`CoolLibrary.Infrastructure/Data/LibraryDbContext.cs`**
   - Changed from `IdentityDbContext` to `IdentityDbContext<ApplicationUser>`
   - Added `ApplicationUserConfiguration` to `OnModelCreating`
   - **Removed**: Customer seeding (moved to `DatabaseSeeder`)

4. ? **`CoolLibrary.Infrastructure/Data/Configurations/CustomerConfiguration.cs`**
   - **Removed**: `FirstName`, `LastName`, `Email` configurations
   - **Added**: `UserId` configuration (FK to `ApplicationUser`)
   - **Added**: Unique index on `UserId`
   - **Updated**: Ignore computed properties (`FullName`, `Email`)

5. ? **`CoolLibrary.Infrastructure/Data/DatabaseSeeder.cs`**
   - Changed from `UserManager<IdentityUser>` to `UserManager<ApplicationUser>`
   - **Added**: `SeedSampleCustomersAsync` method
   - Creates `ApplicationUser` first, then `Customer` with `UserId` link
   - Seeds 2 sample customers with credentials:
     - `john.smith@email.com` / `Customer$123!`
     - `emily.johnson@email.com` / `Customer$123!`

6. ? **`CoolLibrary.Infrastructure/Repositories/CustomersRepository.cs`**
   - **Added**: `.Include(c => c.User)` in all queries
   - **Updated**: Search methods to use `c.User.FirstName`, `c.User.LastName`, `c.User.Email`
   - **Updated**: `EmailExistsAsync` to check `c.User.Email`

---

#### **Application Layer**

7. ? **`CoolLibrary.Application/Mappings/MappingProfile.cs`**
   - **Updated**: `CustomerDTO` mapping to use computed properties
   - Maps `customer.FullName` (delegates to `customer.User.FullName`)
   - Maps `customer.Email` (delegates to `customer.User.Email`)

8. ? **`CoolLibrary.Application/Services/TokenService.cs`**
   - Changed from `IdentityUser` to `ApplicationUser`
   - **Added**: `FirstName` and `LastName` claims in JWT token

---

#### **API Layer**

9. ? **`CoolLibrary.API/Program.cs`**
   - Changed `AddIdentityCore<IdentityUser>` to `AddIdentityCore<ApplicationUser>`
   - **Added**: `using CoolLibrary.Domain.Entities;`
   - Fixed typo: `ILoANS` ? `ILoans`

10. ? **`CoolLibrary.API/Controllers/AuthController.cs`**
    - Changed from `UserManager<IdentityUser>` to `UserManager<ApplicationUser>`
    - **Added**: `using CoolLibrary.Domain.Entities;`

---

## ??? Database Changes Required

### **Migration Needed**

You need to create a migration to apply these changes:

```bash
# From solution root directory
dotnet ef migrations add ApplicationUserAndCustomerRefactoring --project CoolLibrary.Infrastructure --startup-project CoolLibrary.API

# Apply migration
dotnet ef database update --project CoolLibrary.Infrastructure --startup-project CoolLibrary.API
```

### **What the Migration Will Do**

1. **Add columns to `AspNetUsers` table**:
   - `FirstName` (nvarchar(100), required)
   - `LastName` (nvarchar(100), required)
   - `CreatedAt` (datetime2, required, default: GETUTCDATE())
   - `UpdatedAt` (datetime2, required, default: GETUTCDATE())

2. **Modify `Customers` table**:
   - **Add**: `UserId` (nvarchar(450), required, unique)
   - **Add**: Foreign key constraint to `AspNetUsers.Id`
   - **Remove**: `FirstName` column
   - **Remove**: `LastName` column
   - **Remove**: `Email` column
   - **Drop**: Unique index on `Email`
   - **Add**: Unique index on `UserId`

3. **Add index**:
   - `IX_ApplicationUser_Name` on `AspNetUsers (LastName, FirstName)`

---

## ?? Data Migration Strategy

**IMPORTANT**: Existing data needs to be migrated!

### **Option 1: Fresh Database (Development)**
```bash
# Drop and recreate database
dotnet ef database drop --project CoolLibrary.Infrastructure --startup-project CoolLibrary.API --force
dotnet ef database update --project CoolLibrary.Infrastructure --startup-project CoolLibrary.API
```

### **Option 2: Migrate Existing Data (Production)**

Create a custom migration with data migration logic:

```csharp
// In the migration Up() method, BEFORE dropping columns:

// Step 1: Add new columns to AspNetUsers
migrationBuilder.AddColumn<string>("FirstName", "AspNetUsers", ...);
migrationBuilder.AddColumn<string>("LastName", "AspNetUsers", ...);
migrationBuilder.AddColumn<string>("UserId", "Customers", ...);

// Step 2: Migrate existing customer data to AspNetUsers
migrationBuilder.Sql(@"
    -- For each customer, create an ApplicationUser
    INSERT INTO AspNetUsers (Id, UserName, Email, EmailConfirmed, FirstName, LastName, CreatedAt, UpdatedAt)
    SELECT 
        NEWID(),  -- New GUID for UserId
        c.Email,  -- Username = Email
        c.Email,
        1,  -- EmailConfirmed = true
        c.FirstName,
        c.LastName,
        c.CreatedAt,
        c.UpdatedAt
    FROM Customers c
    WHERE NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Email = c.Email)
");

// Step 3: Link customers to their ApplicationUsers
migrationBuilder.Sql(@"
    UPDATE c
    SET c.UserId = u.Id
    FROM Customers c
    INNER JOIN AspNetUsers u ON u.Email = c.Email
");

// Step 4: Drop old columns from Customers
migrationBuilder.DropColumn("FirstName", "Customers");
migrationBuilder.DropColumn("LastName", "Customers");
migrationBuilder.DropColumn("Email", "Customers");
```

---

## ?? Testing the Implementation

### **1. Run the Application**
```bash
dotnet run --project CoolLibrary.API
```

**Expected Console Output**:
```
? Role 'Admin' created successfully
? Role 'User' created successfully
? Admin user created successfully: admin@fake.com
? Sample customer created: john.smith@email.com
? Sample customer created: emily.johnson@email.com
```

---

### **2. Test Login as Admin**
```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "admin@fake.com",
  "password": "admin$123!"
}
```

**Response** (note the new claims):
```json
{
  "token": "eyJhbGci...",
  "expiresAt": "2024-01-16T15:30:00Z",
  "email": "admin@fake.com",
  "roles": ["Admin"]
}
```

**Decode the token at jwt.io** - you'll see:
```json
{
  "sub": "user-guid",
  "email": "admin@fake.com",
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname": "Admin",  ? NEW
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname": "User",     ? NEW
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "Admin"
}
```

---

### **3. Test Login as Customer**
```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "john.smith@email.com",
  "password": "Customer$123!"
}
```

**Response**:
```json
{
  "token": "eyJhbGci...",
  "expiresAt": "2024-01-16T15:30:00Z",
  "email": "john.smith@email.com",
  "roles": ["User"]
}
```

---

### **4. Test Get All Customers (Admin Only)**
```http
GET /api/v1/customers
Authorization: Bearer {admin-token}
```

**Response**:
```json
[
  {
    "customerId": 1,
    "fullName": "John Smith",  ? From customer.User.FirstName + LastName
    "email": "john.smith@email.com",  ? From customer.User.Email
    "membershipStatus": "Active",
    "membershipDate": "2024-07-15T00:00:00Z"
  },
  {
    "customerId": 2,
    "fullName": "Emily Johnson",
    "email": "emily.johnson@email.com",
    "membershipStatus": "Active",
    "membershipDate": "2024-10-15T00:00:00Z"
  }
]
```

---

## ? Benefits of This Refactoring

### **1. Single Source of Truth**
- User information (FirstName, LastName, Email) only in `ApplicationUser`
- No more sync issues between tables

### **2. Flexibility**
- Not all users need to be customers
- Admin users don't need customer profiles
- Easy to add other user types (Librarians, Employees, etc.)

### **3. Clean Architecture**
- Identity handles authentication/authorization
- Customer handles library business logic
- Clear separation of concerns

### **4. Better Queries**
```csharp
// Before (Error-prone):
var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);

// After (Type-safe):
var customer = await _context.Customers
    .Include(c => c.User)  // Eager load
    .FirstOrDefaultAsync(c => c.User.Email == email);

// Access customer info:
Console.WriteLine(customer.FullName);  // Uses customer.User.FullName internally
Console.WriteLine(customer.Email);     // Uses customer.User.Email internally
```

### **5. JWT Tokens Include More Info**
- FirstName and LastName now in token claims
- Can display user's full name in UI without extra API calls

---

## ?? Comparison Table

| Feature | Before | After |
|---------|--------|-------|
| Email storage | 2 places (AspNetUsers + Customers) | 1 place (AspNetUsers) |
| Name storage | 1 place (Customers) | 1 place (ApplicationUser) |
| User-Customer link | ? None | ? UserId FK |
| Admin without customer | ? Not possible | ? Possible |
| Data duplication | ? Yes | ? No |
| Sync issues | ? Potential | ? None |
| Query complexity | Simple but fragile | Slightly more complex but type-safe |
| JWT claims | Basic | ? Includes FirstName/LastName |

---

## ?? Next Steps

1. **Create and Apply Migration**:
   ```bash
   dotnet ef migrations add ApplicationUserRefactoring --project CoolLibrary.Infrastructure --startup-project CoolLibrary.API
   dotnet ef database update --project CoolLibrary.Infrastructure --startup-project CoolLibrary.API
   ```

2. **Test All Endpoints**:
   - Register new user
   - Login as admin
   - Login as customer
   - Get customers (admin only)
   - Search customers by name/email

3. **Update Documentation**:
   - API documentation
   - Database schema diagrams
   - User guides

4. **Consider Additional Features**:
   - Email confirmation workflow
   - Password reset functionality
   - Account lockout after failed attempts
   - Two-factor authentication

---

## ?? Summary

**What changed**:
- ? Created `ApplicationUser` extending `IdentityUser`
- ? Refactored `Customer` to reference `ApplicationUser`
- ? Eliminated data duplication
- ? Clean 1-to-1 relationship
- ? Updated all repositories, services, and mappings
- ? DatabaseSeeder creates customers with linked users

**Result**:
A clean, maintainable architecture following best practices for ASP.NET Core Identity integration with domain entities.

**Status**: ? **Code complete - Migration pending**

