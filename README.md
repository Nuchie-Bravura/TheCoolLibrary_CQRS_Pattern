

```markdown
# 📚 CoolLibrary — Library Management System  
*A modern, extensible Library Management System built with ASP.NET Core 9.0 using Clean Architecture.*

---

## 🧩 Project Purpose  
**CoolLibrary** is an extension of a larger **Context Engineering** project, which can be found here:  
👉 **`https://github.com/JCDiazGomez/TheCoolLibrary`**

The purpose of this project is:

1. To apply **Clean Architecture** deeply, using repository pattern, extension methods, DTO layers, and strict separation of concerns.  
2. To serve as a **baseline** for a future, more advanced version using **CQRS**, so its complexity and benefits can be evaluated.

This makes CoolLibrary both a **learning project** and a **production-ready template**.

---

## 🏗️ Architecture Overview

CoolLibrary follows **Clean Architecture** with strict dependency rules:

```

API → Infrastructure → Application → Domain
Domain has no dependencies.

````

### 📦 Layers

#### **1. Domain (Core Business Logic)**
- Contains entities, value objects, enums, domain events.
- No dependencies.
- Represents pure business rules.

#### **2. Application (Use Cases Layer)**
- Depends only on Domain.
- Contains use cases, DTOs, validations, business logic orchestration.
- Defines repository interfaces (abstractions).
- Uses the Repository Pattern for clean separation from data access.

#### **3. Infrastructure (External Implementations)**
- Depends on Domain + Application.
- Contains:
  - EF Core repositories
  - Database context and migrations
  - External providers (Kafka, Redis, Logging, Email, KeyVault)
- All implementation details live here.

#### **4. API (Presentation Layer)**
- ASP.NET Core 9 minimal hosting model.
- Clean `Program.cs` using **extension methods** such as:
  - `AddApplication()`
  - `AddInfrastructure()`
  - `AddJwtAuthentication()`
- Includes controllers/endpoints, Swagger, JWT, and middleware.

---

## ✨ Why This Architecture?
- Maximum decoupling  
- Business logic fully testable  
- Clean folder structure  
- Clear separation for:
  - DTOs  
  - Validators  
  - Services  
  - Extensions  
  - Repository interfaces and implementations  
- Ready for CQRS, Mediatr, Vertical Slices, and Event Sourcing.

---

## 🛣️ Roadmap

### **Phase 1 — Clean Architecture (this project)**
Solid foundation using:
- Repository Pattern  
- DTO and Validation layers  
- Extension methods to keep Program.cs clean  
- Separation of responsibilities across all layers  

### **Phase 2 — CQRS Version (coming next)**
A full rewrite using:
- CQRS  
- Commands and Queries  
- Mediatr  
- Validation and logging pipeline behaviors  
- Vertical slice structure (optional)

Goal → Evaluate if CQRS is worth adopting for larger systems like SmartOrders.

---

## 🚀 Getting Started

### **Prerequisites**
- .NET 9 SDK  
- SQL Server  
- Visual Studio 2022 / Rider / VS Code  
- Azure CLI (optional for Key Vault)

---

### **Installation Steps**

#### 1. Clone the repository  
```bash
git clone <your-repo-url>
cd CoolLibrary
````

#### 2. Restore dependencies

```bash
dotnet restore
```

#### 3. Configure appsettings

Copy the example settings:

```
appsettings.example.json → appsettings.json
```

Update SQL Server connection string.

#### 4. Configure Azure Key Vault (local development)

```bash
dotnet user-secrets init --project CoolLibrary.API
dotnet user-secrets set "Jwt:Key" "your-secret-key-value" --project CoolLibrary.API
```

#### 5. Apply database migrations

```bash
dotnet ef database update --project CoolLibrary.Infrastructure --startup-project CoolLibrary.API
```

#### 6. Run the application

```bash
dotnet run --project CoolLibrary.API
```

#### 7. Open Swagger UI

* [https://localhost:5001](https://localhost:5001)
* [http://localhost:5000](http://localhost:5000)

---

## 🧰 Tech Stack

### **Frameworks & Libraries**

* ASP.NET Core 9
* Entity Framework Core 9
* AutoMapper
* FluentValidation
* Swashbuckle (Swagger/OpenAPI)
* Confluent.Kafka
* OpenTelemetry
* Prometheus

### **Database**

* SQL Server

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


