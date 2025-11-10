# API Testing Examples - Postman/HTTP Requests

## ?? Base URL
```
https://localhost:5001/api/v1
```

---

## ?? Authentication Endpoints (Public)

### 1. Register New User (Gets "User" Role)

**Request:**
```http
POST https://localhost:5001/api/v1/auth/register
Content-Type: application/json

{
  "email": "john.doe@test.com",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!"
}
```

**Response (200 OK):**
```json
{
  "message": "User registered successfully",
  "email": "john.doe@test.com",
  "role": "User"
}
```

---

### 2. Login as Admin (Seeded User)

**Request:**
```http
POST https://localhost:5001/api/v1/auth/login
Content-Type: application/json

{
  "email": "admin@fake.com",
  "password": "admin$123!"
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhYmMtMTIzLWd1aWQiLCJlbWFpbCI6ImFkbWluQGZha2UuY29tIiwianRpIjoiOTg3LWd1aWQiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsIm5hbWVpZCI6ImFiYy0xMjMtZ3VpZCIsIm5hbWUiOiJhZG1pbkBmYWtlLmNvbSIsImV4cCI6MTcwNTQyMTQwMCwiaXNzIjoiaHR0cHM6Ly90dS1hcGkuY29tIiwiYXVkIjoiaHR0cHM6Ly90dS1hcGkuY29tIn0.signature",
  "expiresAt": "2024-01-16T15:30:00Z",
  "email": "admin@fake.com",
  "roles": [
    "Admin"
  ]
}
```

**?? Save the token!** You'll use it in subsequent requests.

---

### 3. Login as Regular User

**Request:**
```http
POST https://localhost:5001/api/v1/auth/login
Content-Type: application/json

{
  "email": "john.doe@test.com",
  "password": "SecurePass123!"
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ4eXotNDU2LWd1aWQiLCJlbWFpbCI6ImpvaG4uZG9lQHRlc3QuY29tIiwianRpIjoiMTIzLWd1aWQiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJVc2VyIiwibmFtZWlkIjoieHl6LTQ1Ni1ndWlkIiwibmFtZSI6ImpvaG4uZG9lQHRlc3QuY29tIiwiZXhwIjoxNzA1NDIxNDAwLCJpc3MiOiJodHRwczovL3R1LWFwaS5jb20iLCJhdWQiOiJodHRwczovL3R1LWFwaS5jb20ifQ.signature",
  "expiresAt": "2024-01-16T15:30:00Z",
  "email": "john.doe@test.com",
  "roles": [
    "User"
  ]
}
```

---

### 4. Renew Token (Requires Authentication)

**Request:**
```http
POST https://localhost:5001/api/v1/auth/renewToken
Content-Type: application/json
Authorization: Bearer {your-current-token}
```

**Response (200 OK):**
```json
{
  "token": "new-jwt-token-here",
  "expiresAt": "2024-01-16T16:30:00Z",
  "email": "john.doe@test.com",
  "roles": [
    "User"
  ]
}
```

---

## ?? Books Endpoints (User & Admin)

### 5. Get All Books (? User & Admin)

**Request:**
```http
GET https://localhost:5001/api/v1/books/ListBooks
Authorization: Bearer {user-or-admin-token}
```

**Response (200 OK):**
```json
[
  {
    "bookId": 1,
    "title": "1984",
    "isbn": "978-0-452-28423-4",
    "description": "A dystopian social science fiction novel",
    "publicationDate": "1949-06-08T00:00:00",
    "publisher": "Secker & Warburg",
    "pageCount": 328,
    "language": "English",
    "availableCopies": 3,
    "totalCopies": 5,
    "authors": ["George Orwell"],
    "genres": ["Fiction", "Classic Literature"]
  },
  {
    "bookId": 2,
    "title": "Pride and Prejudice",
    "isbn": "978-0-14-143951-8",
    "description": "A romantic novel of manners",
    "publicationDate": "1813-01-28T00:00:00",
    "publisher": "T. Egerton",
    "pageCount": 432,
    "language": "English",
    "availableCopies": 2,
    "totalCopies": 3,
    "authors": ["Jane Austen"],
    "genres": ["Fiction", "Classic Literature"]
  }
]
```

---

## ?? Authors Endpoints (User & Admin)

### 6. Get All Authors with Books (? User & Admin)

**Request:**
```http
GET https://localhost:5001/api/v1/authors/with-books
Authorization: Bearer {user-or-admin-token}
```

**Response (200 OK):**
```json
[
  {
    "authorId": 1,
    "firstName": "George",
    "lastName": "Orwell",
    "fullName": "George Orwell",
    "biography": "English novelist and essayist, journalist and critic",
    "birthDate": "1903-06-25T00:00:00",
    "nationality": "British",
    "books": [
      {
        "bookId": 1,
        "title": "1984",
        "isbn": "978-0-452-28423-4"
      },
      {
        "bookId": 4,
        "title": "Animal Farm",
        "isbn": "978-0-452-28424-1"
      }
    ]
  }
]
```

---

## ?? Loans Endpoints (User & Admin)

### 7. Check Book Availability (? User & Admin)

**Request:**
```http
GET https://localhost:5001/api/v1/loans/availability/1
Authorization: Bearer {user-or-admin-token}
```

**Response (200 OK):**
```json
{
  "bookId": 1,
  "bookTitle": "1984",
  "totalCopies": 5,
  "availableCopies": 3,
  "loanedCopies": 2,
  "isAvailable": true
}
```

---

### 8. Request a Loan (? User & Admin)

**Request:**
```http
POST https://localhost:5001/api/v1/loans/RequestLoan
Content-Type: application/json
Authorization: Bearer {user-or-admin-token}

{
  "customerId": 1,
  "bookId": 1,
  "loanDate": "2024-01-15T00:00:00",
  "expectedReturnDate": "2024-02-15T00:00:00"
}
```

**Response (200 OK):**
```json
{
  "loanId": 123,
  "customerId": 1,
  "bookId": 1,
  "loanDate": "2024-01-15T00:00:00",
  "expectedReturnDate": "2024-02-15T00:00:00",
  "status": "Active",
  "remainingCopies": 2
}
```

**Response (400 Bad Request) - Book Unavailable:**
```json
{
  "error": "Book with ID 1 is not available for loan"
}
```

---

## ?? Customers Endpoints (Admin Only)

### 9. Get All Customers (? Admin Only)

**Request with Admin Token:**
```http
GET https://localhost:5001/api/v1/customers
Authorization: Bearer {admin-token}
```

**Response (200 OK):**
```json
[
  {
    "customerId": 1,
    "fullName": "John Smith",
    "email": "john.smith@email.com",
    "membershipStatus": "Active",
    "membershipDate": "2023-07-01T00:00:00Z"
  },
  {
    "customerId": 2,
    "fullName": "Emily Johnson",
    "email": "emily.johnson@email.com",
    "membershipStatus": "Active",
    "membershipDate": "2023-10-01T00:00:00Z"
  }
]
```

---

**Request with User Token (? Forbidden):**
```http
GET https://localhost:5001/api/v1/customers
Authorization: Bearer {user-token}
```

**Response (403 Forbidden):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "traceId": "00-abc123..."
}
```

---

### 10. Create Customer (? Admin Only)

**Request with Admin Token:**
```http
POST https://localhost:5001/api/v1/customers
Content-Type: application/json
Authorization: Bearer {admin-token}

{
  "firstName": "Alice",
  "lastName": "Williams",
  "email": "alice.williams@email.com",
  "phone": "+1-555-0199",
  "address": "789 Pine Street",
  "city": "Chicago",
  "postalCode": "60601",
  "maxBooksAllowed": 5
}
```

**Response (201 Created):**
```json
{
  "customerId": 3,
  "fullName": "Alice Williams",
  "email": "alice.williams@email.com",
  "membershipStatus": "Active",
  "membershipDate": "2024-01-15T10:30:00Z"
}
```

---

**Request with User Token (? Forbidden):**
```http
POST https://localhost:5001/api/v1/customers
Content-Type: application/json
Authorization: Bearer {user-token}

{
  "firstName": "Alice",
  "lastName": "Williams",
  "email": "alice.williams@email.com"
}
```

**Response (403 Forbidden):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "traceId": "00-xyz789..."
}
```

---

### 11. Delete Customer (? Admin Only)

**Request with Admin Token:**
```http
DELETE https://localhost:5001/api/v1/customers/3
Authorization: Bearer {admin-token}
```

**Response (204 No Content):**
```
(Empty body)
```

---

**Request with User Token (? Forbidden):**
```http
DELETE https://localhost:5001/api/v1/customers/3
Authorization: Bearer {user-token}
```

**Response (403 Forbidden):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "traceId": "00-def456..."
}
```

---

### 12. Patch Customer (? Admin Only)

**Request with Admin Token:**
```http
PATCH https://localhost:5001/api/v1/customers/1
Content-Type: application/json-patch+json
Authorization: Bearer {admin-token}

[
  {
    "op": "replace",
    "path": "/firstName",
    "value": "Jonathan"
  },
  {
    "op": "replace",
    "path": "/maxBooksAllowed",
    "value": 10
  }
]
```

**Response (200 OK):**
```json
{
  "customerId": 1,
  "fullName": "Jonathan Smith",
  "email": "john.smith@email.com",
  "membershipStatus": "Active",
  "membershipDate": "2023-07-01T00:00:00Z"
}
```

---

## ? Error Scenarios

### Scenario 1: No Token Provided

**Request:**
```http
GET https://localhost:5001/api/v1/books/ListBooks
(No Authorization header)
```

**Response (401 Unauthorized):**
```http
HTTP/1.1 401 Unauthorized
WWW-Authenticate: Bearer
```

---

### Scenario 2: Invalid Token

**Request:**
```http
GET https://localhost:5001/api/v1/books/ListBooks
Authorization: Bearer invalid-token-12345
```

**Response (401 Unauthorized):**
```http
HTTP/1.1 401 Unauthorized
WWW-Authenticate: Bearer error="invalid_token"
```

---

### Scenario 3: Expired Token

**Request:**
```http
GET https://localhost:5001/api/v1/books/ListBooks
Authorization: Bearer {expired-token}
```

**Response (401 Unauthorized):**
```http
HTTP/1.1 401 Unauthorized
WWW-Authenticate: Bearer error="invalid_token", error_description="The token expired at '01/15/2024 10:00:00'"
```

---

### Scenario 4: Insufficient Permissions (User trying to access Admin endpoint)

**Request:**
```http
GET https://localhost:5001/api/v1/customers
Authorization: Bearer {user-token-with-User-role}
```

**Response (403 Forbidden):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "traceId": "00-1234567890abcdef-1234567890abcdef-00"
}
```

---

## ?? Quick Reference Table

| Endpoint | Method | No Token | User Token | Admin Token |
|----------|--------|----------|------------|-------------|
| `/auth/register` | POST | ? 200 | ? 200 | ? 200 |
| `/auth/login` | POST | ? 200 | ? 200 | ? 200 |
| `/auth/renewToken` | POST | ? 401 | ? 200 | ? 200 |
| `/books/ListBooks` | GET | ? 401 | ? 200 | ? 200 |
| `/authors/with-books` | GET | ? 401 | ? 200 | ? 200 |
| `/loans/availability/{id}` | GET | ? 401 | ? 200 | ? 200 |
| `/loans/RequestLoan` | POST | ? 401 | ? 200 | ? 200 |
| `/customers` | GET | ? 401 | ? 403 | ? 200 |
| `/customers` | POST | ? 401 | ? 403 | ? 201 |
| `/customers/{id}` | DELETE | ? 401 | ? 403 | ? 204 |
| `/customers/{id}` | PATCH | ? 401 | ? 403 | ? 200 |

**Legend:**
- ? Success (status code shown)
- ? Error (status code shown)

---

## ?? Postman Collection Setup

### Environment Variables
Create these in your Postman environment:

```json
{
  "baseUrl": "https://localhost:5001/api/v1",
  "adminToken": "",
  "userToken": ""
}
```

### Pre-request Script for Login
After logging in, save the token automatically:

```javascript
// In the "Tests" tab of login requests
pm.test("Save token", function () {
    var jsonData = pm.response.json();
    
    // Determine if this is admin or user based on roles
    if (jsonData.roles.includes("Admin")) {
        pm.environment.set("adminToken", jsonData.token);
    } else {
        pm.environment.set("userToken", jsonData.token);
    }
});
```

### Using Tokens in Requests
In the Authorization tab:
- Type: Bearer Token
- Token: `{{adminToken}}` or `{{userToken}}`

---

## ?? Testing Checklist

- [ ] Register new user ? receives "User" role
- [ ] Login as admin ? receives token with "Admin" role
- [ ] Login as user ? receives token with "User" role
- [ ] Access `/books` with user token ? Success ?
- [ ] Access `/books` with admin token ? Success ?
- [ ] Access `/customers` with user token ? Forbidden ?
- [ ] Access `/customers` with admin token ? Success ?
- [ ] Access any endpoint without token ? Unauthorized ?
- [ ] Token renewal works correctly
- [ ] Expired token returns 401

---

## ?? Tips

1. **Use Postman Environment Variables** to store tokens
2. **Check token expiration** in jwt.io before testing
3. **Use Swagger UI** for quick testing (built-in authorization)
4. **Monitor logs** to see authentication/authorization events
5. **Test edge cases**: expired tokens, invalid formats, missing roles

