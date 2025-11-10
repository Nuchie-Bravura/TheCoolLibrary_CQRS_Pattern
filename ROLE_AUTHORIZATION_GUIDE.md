# Role-Based Authorization Implementation Guide

## ?? Overview

This document explains the role-based authorization system implemented in the CoolLibrary API. The system uses **ASP.NET Core Identity** with **JWT tokens** to manage user authentication and role-based access control.

---

## ?? Implementation Summary

### **Roles Created**
- **Admin**: Full access to all endpoints (including CustomersController)
- **User**: Access to Books, Authors, and Loans. **NO access** to CustomersController

### **Seeded Credentials**
For testing purposes, the following admin account is automatically created:
- **Email**: `admin@fake.com`
- **Password**: `admin$123!`

?? **WARNING**: Change these credentials in production!

---

## ?? What Changed in the Solution

### **1. New File: `IdentitySeedExtensions.cs`**
**Location**: `CoolLibrary.API/Extensions/IdentitySeedExtensions.cs`

**Purpose**: Seeds the database with roles and admin user during application startup.

**What it does**:
- Creates two roles: `Admin` and `User`
- Creates a default admin account: `admin@fake.com` / `admin$123!`
- Assigns the `Admin` role to the seeded user
- Runs automatically when the application starts

**Key Methods**:
```csharp
// Extension method called from Program.cs
public static async Task SeedRolesAndAdminAsync(this WebApplication app)

// Creates Admin and User roles
private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)

// Creates default admin account
private static async Task SeedAdminUserAsync(UserManager<IdentityUser> userManager, ILogger logger)
```

---

### **2. Updated: `Program.cs`**

**Changes**:
- Added `using CoolLibrary.API.Extensions;`
- Added role seeding call after `var app = builder.Build();`:
  ```csharp
  // SEED ROLES AND ADMIN USER
  await app.SeedRolesAndAdminAsync();
  ```

**Why?**
This ensures that roles and admin user are created/verified every time the application starts. It's safe to call multiple times (idempotent).

---

### **3. Updated: `AuthController.cs`**

**Changes in `Register` endpoint**:
```csharp
// Step 6: Assign default "User" role to new registrations
var roleResult = await _userManager.AddToRoleAsync(newUser, "User");
```

**What this means**:
- All users who register through `/api/auth/register` automatically get the `User` role
- They can access Books, Authors, and Loans endpoints
- They **CANNOT** access Customers endpoints (Admin only)

---

### **4. Updated: `CustomersController.cs`**

**Authorization attribute**:
```csharp
[Authorize(Roles = "Admin")]  // Only Admin role can access
```

**What this means**:
- Only users with `Admin` role can access any endpoint in this controller
- Users with `User` role will receive a **403 Forbidden** response

---

### **5. Updated: `BooksController.cs`**

**Authorization attribute**:
```csharp
[Authorize(Roles = "User,Admin")]  // Both roles can access
```

**What this means**:
- Users with either `User` or `Admin` role can access this controller
- Unauthenticated users will receive a **401 Unauthorized** response

---

### **6. Updated: `AuthorsController.cs`**

**Authorization attribute**:
```csharp
[Authorize(Roles = "User,Admin")]  // Both roles can access
```

**What this means**:
- Users with either `User` or `Admin` role can access this controller

---

### **7. Updated: `LoansController.cs`**

**Authorization attribute**:
```csharp
[Authorize(Roles = "User,Admin")]  // Both roles can access
```

**What this means**:
- Users with either `User` or `Admin` role can access loan operations

---

## ??? Database Changes

The seeding process creates records in the following Identity tables:

### **AspNetRoles** (Roles table)
| Id | Name | NormalizedName |
|----|------|----------------|
| GUID | Admin | ADMIN |
| GUID | User | USER |

### **AspNetUsers** (Users table)
| Id | UserName | Email | EmailConfirmed |
|----|----------|-------|----------------|
| GUID | admin@fake.com | admin@fake.com | true |

### **AspNetUserRoles** (User-Role mapping)
| UserId | RoleId |
|--------|--------|
| admin-GUID | Admin-GUID |

---

## ?? How to Test

### **1. Run the Application**
```bash
dotnet run --project CoolLibrary.API
```

**Console Output** (you should see):
```
? Role 'Admin' created successfully
? Role 'User' created successfully
? Admin user created successfully: admin@fake.com
?? Default admin credentials - Email: admin@fake.com, Password: admin$123!
```

---

### **2. Login as Admin**

**Endpoint**: `POST /api/v1/auth/login`

**Request**:
```json
{
  "email": "admin@fake.com",
  "password": "admin$123!"
}
```

**Response**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-01-16T15:30:00Z",
  "email": "admin@fake.com",
  "roles": ["Admin"]
}
```

**Copy the token** - you'll need it for authenticated requests.

---

### **3. Register a Regular User**

**Endpoint**: `POST /api/v1/auth/register`

**Request**:
```json
{
  "email": "user@test.com",
  "password": "User$123!",
  "confirmPassword": "User$123!"
}
```

**Response**:
```json
{
  "message": "User registered successfully",
  "email": "user@test.com",
  "role": "User"
}
```

---

### **4. Login as Regular User**

**Endpoint**: `POST /api/v1/auth/login`

**Request**:
```json
{
  "email": "user@test.com",
  "password": "User$123!"
}
```

**Response**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-01-16T15:30:00Z",
  "email": "user@test.com",
  "roles": ["User"]
}
```

---

### **5. Test Access Control**

#### ? **Admin Can Access Everything**

**Headers**: `Authorization: Bearer {admin-token}`

| Endpoint | Method | Expected Result |
|----------|--------|----------------|
| `/api/v1/customers` | GET | ? 200 OK |
| `/api/v1/books/ListBooks` | GET | ? 200 OK |
| `/api/v1/authors/with-books` | GET | ? 200 OK |
| `/api/v1/loans/availability/1` | GET | ? 200 OK |

---

#### ? **User Can Access Books/Authors/Loans**

**Headers**: `Authorization: Bearer {user-token}`

| Endpoint | Method | Expected Result |
|----------|--------|----------------|
| `/api/v1/books/ListBooks` | GET | ? 200 OK |
| `/api/v1/authors/with-books` | GET | ? 200 OK |
| `/api/v1/loans/availability/1` | GET | ? 200 OK |

---

#### ? **User CANNOT Access Customers**

**Headers**: `Authorization: Bearer {user-token}`

| Endpoint | Method | Expected Result |
|----------|--------|----------------|
| `/api/v1/customers` | GET | ? **403 Forbidden** |
| `/api/v1/customers` | POST | ? **403 Forbidden** |
| `/api/v1/customers/1` | DELETE | ? **403 Forbidden** |

---

## ?? Error Handling Explained

### **Scenario 1: User with "User" Role Tries to Access CustomersController**

**Request**:
```http
GET /api/v1/customers
Authorization: Bearer {user-token}
```

**Response**:
```http
HTTP/1.1 403 Forbidden
Content-Type: application/json

{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "traceId": "00-abc123..."
}
```

**What happened**:
1. User's JWT token was **validated successfully** (authentication ?)
2. ASP.NET Core checked the `[Authorize(Roles = "Admin")]` attribute
3. User's token contains `role: "User"`, not `"Admin"`
4. Authorization **failed** ? 403 Forbidden response

**This is handled automatically by ASP.NET Core** - no custom code needed!

---

### **Scenario 2: No Token Provided**

**Request**:
```http
GET /api/v1/customers
(No Authorization header)
```

**Response**:
```http
HTTP/1.1 401 Unauthorized
WWW-Authenticate: Bearer
```

**What happened**:
1. No JWT token provided
2. `[Authorize]` attribute requires authentication
3. Authentication **failed** ? 401 Unauthorized response

---

### **Scenario 3: Invalid or Expired Token**

**Request**:
```http
GET /api/v1/customers
Authorization: Bearer invalid-or-expired-token
```

**Response**:
```http
HTTP/1.1 401 Unauthorized
WWW-Authenticate: Bearer error="invalid_token"
```

**What happened**:
1. JWT validation failed (signature invalid or token expired)
2. Authentication **failed** ? 401 Unauthorized response

---

## ?? How JWT Tokens Contain Roles

When you decode the JWT token (use [jwt.io](https://jwt.io)), you'll see:

**Admin Token Payload**:
```json
{
  "sub": "user-guid-here",
  "email": "admin@fake.com",
  "jti": "unique-token-id",
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "Admin",
  "exp": 1705421400,
  "iss": "https://tu-api.com",
  "aud": "https://tu-api.com"
}
```

**User Token Payload**:
```json
{
  "sub": "user-guid-here",
  "email": "user@test.com",
  "jti": "unique-token-id",
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "User",
  "exp": 1705421400,
  "iss": "https://tu-api.com",
  "aud": "https://tu-api.com"
}
```

The `role` claim is what ASP.NET Core uses to check authorization!

---

## ?? Repository Changes

**Question**: Did repositories change?

**Answer**: **NO**, repositories remain unchanged.

**Why?**
- Authorization happens at the **API/Controller layer** (HTTP requests)
- Repositories are in the **Infrastructure layer** (data access)
- Repositories don't know about authentication or roles
- This follows **Clean Architecture** principles

**Separation of Concerns**:
```
HTTP Request
    ?
[Authorize(Roles = "Admin")] ? Authorization check here
    ?
Controller
    ?
Repository ? No authorization logic here
    ?
Database
```

---

## ?? Best Practices Implemented

### ? **1. Least Privilege Principle**
- New users get `User` role by default
- Only manually created/seeded users get `Admin` role

### ? **2. Separation of Concerns**
- Authentication: JWT token validation
- Authorization: Role-based access control
- Business Logic: Repositories (no auth logic)

### ? **3. Idempotent Seeding**
- Seeding can run multiple times safely
- Checks if roles/users exist before creating

### ? **4. Logging**
- All role/user creation events are logged
- Failed login attempts are logged

### ? **5. Clear Error Messages**
- 401: You need to authenticate (no token)
- 403: You're authenticated but don't have permission (wrong role)

---

## ?? Security Notes

### **Production Checklist**:
- [ ] Change default admin password
- [ ] Use Azure Key Vault for JWT secret key
- [ ] Enable email confirmation for new users
- [ ] Implement password reset functionality
- [ ] Add account lockout after failed login attempts
- [ ] Use HTTPS only
- [ ] Implement token refresh mechanism
- [ ] Add audit logging for admin actions

---

## ?? Testing with Swagger

1. **Start the application**
2. **Navigate to Swagger UI**: `https://localhost:5001/`
3. **Login as admin**:
   - Expand `/api/v1/auth/login`
   - Click "Try it out"
   - Enter admin credentials
   - Copy the token from response
4. **Authorize in Swagger**:
   - Click ?? "Authorize" button (top right)
   - Enter: `Bearer {your-token-here}`
   - Click "Authorize"
5. **Test endpoints**:
   - All endpoints should now work with admin token
   - Try with user token - Customers endpoints should fail

---

## ?? Quick Reference

### **Role Summary**
| Role | CustomersController | BooksController | AuthorsController | LoansController |
|------|---------------------|-----------------|-------------------|-----------------|
| Admin | ? Full Access | ? Full Access | ? Full Access | ? Full Access |
| User | ? Forbidden | ? Full Access | ? Full Access | ? Full Access |
| No Auth | ? Unauthorized | ? Unauthorized | ? Unauthorized | ? Unauthorized |

### **HTTP Status Codes**
- **200 OK**: Request successful
- **401 Unauthorized**: No token or invalid token
- **403 Forbidden**: Valid token but insufficient permissions (wrong role)
- **404 Not Found**: Resource doesn't exist

---

## ?? Summary

You now have a complete role-based authorization system:
- ? Two roles: Admin and User
- ? Automatic role seeding on startup
- ? Default admin account for testing
- ? New users get "User" role automatically
- ? CustomersController restricted to Admin only
- ? Other controllers accessible to both roles
- ? Proper error handling (401 vs 403)
- ? JWT tokens include role claims
- ? No changes needed in repositories

**Next Steps**:
1. Run `dotnet ef database update` to apply migrations (if needed)
2. Run the application
3. Test with the seeded admin account
4. Register a test user
5. Verify access control is working correctly
