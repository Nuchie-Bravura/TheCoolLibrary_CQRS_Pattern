# 📚 CoolLibrary — Library Management System

*A modern, extensible Library Management System built with **ASP.NET Core 9.0** using **Clean Architecture**.*

---

## 🧩 Project Purpose

**CoolLibrary** is part of a larger project, **Context Engineering**:  
👉 [`https://github.com/JCDiazGomez/TheCoolLibrary`](https://github.com/JCDiazGomez/TheCoolLibrary)

The main goals of this project are:

1. To rigorously apply **Clean Architecture**, including the repository pattern, extension methods, DTO layers, and strict separation of concerns.
2. To serve as a **baseline** for a more advanced future version using **CQRS**, so its benefits and complexity can be compared.

This makes CoolLibrary both a **learning project** and a **production-ready template**.

---

## 🏗️ Architecture Overview

CoolLibrary follows **Clean Architecture** with strict dependency rules:

```
API → Infrastructure → Application → Domain
Domain has no dependencies.
```

### 📦 Project Layers

#### 1. **Domain** (Core Business Logic)
- Contains entities, value objects, enums, and domain events.
- Has no external dependencies.
- Encapsulates pure business rules.

#### 2. **Application** (Use Cases Layer)
- Depends only on Domain.
- Contains use cases, DTOs, validations, and business logic orchestration.
- Defines repository interfaces (abstractions).
- Uses the Repository Pattern to separate business logic from data access.

#### 3. **Infrastructure** (External Implementations)
- Depends on Domain and Application.
- Contains:
  - EF Core repositories
  - DbContext and migrations
  - External providers (Kafka, Redis, Logging, Email, KeyVault)
- All implementation details live here.

#### 4. **API** (Presentation Layer)
- ASP.NET Core 9, minimal hosting model.
- Clean `Program.cs` using extension methods like:
  - `AddApplication()`
  - `AddInfrastructure()`
  - `AddJwtAuthentication()`
- Contains controllers, endpoints, Swagger, JWT, and middleware.

---

## ✨ Why This Architecture?

- Maximum decoupling
- Fully testable business logic
- Clean folder structure
- Clear separation of:
    - DTOs
    - Validators
    - Services
    - Extensions
    - Repository interfaces and implementations
- Ready for CQRS, Mediatr, Vertical Slices, and Event Sourcing.

---

## 🛣️ Roadmap

### Phase 1 — **Clean Architecture** (this project)
- Repository Pattern
- DTO and Validation layers
- Extension methods to keep Program.cs clean
- Responsibility separation across all layers

### Phase 2 — **CQRS Version** 
- Full CQRS rewrite
- Commands and Queries
- Mediatr
- Validation and logging pipeline behaviors
- Optional: Vertical slice structure

**Goal:** Evaluate if CQRS is worthwhile for large systems like SmartOrders.

---

## 🚀 Getting Started

### **Prerequisites**
- .NET 9 SDK
- SQL Server
- Visual Studio 2022 / Rider / VS Code
- Azure CLI (optional, for Key Vault)

---

### **Installation**

1. Clone the repository:
    ```bash
    git clone <your-repo-url>
    cd CoolLibrary
    ```

2. Restore dependencies:
    ```bash
    dotnet restore
    ```

3. Configure the settings (copy the example file):
    ```
    appsettings.example.json → appsettings.json
    ```
    Update the SQL Server connection string.

4. Configure Azure Key Vault (for local development):
    ```bash
    dotnet user-secrets init --project CoolLibrary.API
    dotnet user-secrets set "Jwt:Key" "your-secret" --project CoolLibrary.API
    ```

5. Apply the database migrations:
    ```bash
    dotnet ef database update --project CoolLibrary.Infrastructure --startup-project CoolLibrary.API
    ```

6. Run the application:
    ```bash
    dotnet run --project CoolLibrary.API
    ```

7. Open the Swagger UI:
    - [https://localhost:5001](https://localhost:5001)
    - [http://localhost:5000](http://localhost:5000)

---

## 🧰 Tech Stack

### **Frameworks & Libraries**
- ASP.NET Core 9
- Entity Framework Core 9
- AutoMapper
- FluentValidation
- Azzure Secrets & Blob Storage
- Extenstion Methods on Lawyers to simplify program.cs readability
- Swashbuckle (Swagger/OpenAPI)
- Redis RateLimit & Cache
- Moq & FluentAssertions &  AAA (Arrange-Act-Assert) pattern
- GraphQL {just query} Hot chocolate & banana cake  {only on Repository Pattern}

### **Database**
- SQL Server

---

## 📚 Domain Entities

The system manages:

* Books
* Authors
* Genres
* Customers
* Loans
* Reservations
* Fines

---

## 🧑‍💻 Development

Build the solution:
```bash
dotnet build
```

Run the API:
```bash
dotnet run --project CoolLibrary.API
```
