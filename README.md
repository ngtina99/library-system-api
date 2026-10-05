# Library System API

ASP.NET Core Web API for managing books, users and book loans in a library system.

## Table of Contents

- [Tech Stack](#tech-stack)
- [Running the Application](#running-the-application)
  - [Running Tests](#running-tests)
- [Architecture](#architecture)
- [API Endpoints](#api-endpoints)
  - [Books](#books)
  - [Users](#users)
  - [Loans](#loans)
- [Business Rules](#business-rules)
- [Key Decisions](#key-decisions)

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- EF Core InMemory
- OpenAPI / Swagger
- xUnit

## Running the Application

Requirements:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) - **8.0.425** (verify with `dotnet --version`)

Run the API:

```bash
dotnet run --project src/LibrarySystem.Api
```

Swagger UI is available in development mode at:

```text
/swagger
```

### Running Tests

Run all tests from the repository root:

```bash
dotnet test
```

The test suite covers the following:

- borrowing an available book
- preventing borrowing of an unavailable book
- returning an active loan
- preventing a loan from being returned twice
- preventing deletion of a book that is currently on loan
- deleting a book that has no active loan
- rejecting a publication year in the future

## Architecture

The application uses a simple layered architecture:

```text
HTTP Request
     ↓
Controller
     ↓
Service
     ↓
EF Core DbContext
     ↓
InMemory Database
```

Project structure:
```text
Project/
├── Controllers/       # HTTP requests, responses, status codes
├── Services/          # Application and business logic
├── DTOs/              # API request/response contracts
├── Models/            # Persistence/domain entities
├── Data/              # EF Core DbContext and configurations
├── Validation/        # Custom validation attributes
└── tests/             # Automated tests
```

## API Endpoints

### Books

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/books` | List all books |
| `GET` | `/api/books/{id}` | Get a book by ID |
| `GET` | `/api/books?available=true` | Filter books by availability |
| `GET` | `/api/books?author=Martin` | Filter books by author |
| `POST` | `/api/books` | Create a new book |
| `PUT` | `/api/books/{id}` | Update a book |
| `DELETE` | `/api/books/{id}` | Delete a book if it is not on loan |

### Users

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/users` | List all users |
| `GET` | `/api/users/{id}` | Get a user by ID |
| `POST` | `/api/users` | Register a new user |

### Loans

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/loans/active` | List active loans |
| `GET` | `/api/loans/{id}` | Get a loan by ID |
| `POST` | `/api/loans` | Borrow a book |
| `POST` | `/api/loans/{id}/return` | Return a borrowed book |

## Business Rules

The API implements the following:

- A book can only be borrowed if it is available.
- Borrowing a book marks it as unavailable.
- Returning a book marks it as available again.
- A loan cannot be returned more than once.
- A book cannot be deleted while it has an active loan.
- A loan can only be created for an existing user and book.
- A book's publication year cannot be in the future.

## Key Decisions

- EF Core InMemory is used to keep the project simple and avoid requiring an external database or OS-specific dependencies.
- Concurrent borrowing is not protected in this InMemory implementation.
- DTOs are used to keep API contracts separate from internal models.
- Data Annotations and custom validation attributes are used to validate request DTOs.
- Services are used to keep business logic separate from HTTP handling in controllers.
- `409 Conflict` is returned for business-rule violations, such as borrowing an unavailable book.
- Expected errors are handled explicitly and mapped to appropriate HTTP status codes.
- Unexpected exceptions are handled globally using ASP.NET Core's built-in exception-handling middleware with `ProblemDetails`, returning `500 Internal Server Error`.
- `DateTimeOffset.UtcNow` is used for timestamps to avoid local time ambiguity.
- Asynchronous EF Core APIs are used for database operations.
- `AsNoTracking()` is applied to read-only queries to avoid unnecessary change tracking.
- A repository layer is omitted because EF Core's `DbContext` already provides data access abstraction, avoiding unnecessary complexity.
