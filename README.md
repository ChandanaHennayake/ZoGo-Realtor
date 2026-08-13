# zogo Realtor

Real estate platform backend built with **Clean Architecture** on **.NET 10** and **PostgreSQL**.

## Solution Structure

```
RealEstate/
├── src/
│   ├── zogo.API              # Web API (controllers, middleware, Swagger)
│   ├── zogo.Application    # Use cases, DTOs, validation, interfaces
│   ├── zogo.Domain         # Entities, domain interfaces, repository contracts
│   └── zogo.Infrastructure # EF Core, PostgreSQL, JWT, repositories
├── tests/
│   ├── zogo.UnitTests
│   └── zogo.IntegrationTests
└── README.md
```

## Tech Stack

| Area | Technology |
|------|------------|
| Runtime | .NET 10 |
| Database | PostgreSQL |
| ORM | Entity Framework Core |
| Validation | FluentValidation |
| Auth | JWT Bearer |
| Logging | Serilog |
| API Docs | Swagger / OpenAPI |
| Versioning | Asp.Versioning.Mvc |

## Foundation Features

- **Clean Architecture** with strict layer dependencies
- **Repository pattern** and **Unit of Work**
- **UUID primary keys** with PostgreSQL `gen_random_uuid()`
- **`timestamptz`** columns for all audit timestamps
- **Soft-delete** via global query filters and save interceptor
- **Audit fields** (`CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`) via interceptor
- **Standard API response** envelope (`ApiResponse<T>`)
- **Global exception handling** middleware
- **JWT infrastructure** (token generation + validation)
- **CORS**, **API versioning**, **request logging**

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 15+](https://www.postgresql.org/download/)

## Getting Started

### 1. Configure the database

Update the connection string in `src/zogo.API/appsettings.Development.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=zogo_realtor_dev;Username=postgres;Password=YOUR_PASSWORD"
}
```

Create the database:

```sql
CREATE DATABASE zogo_realtor_dev;
```

### 2. Run migrations (when entities are added)

```bash
dotnet ef migrations add InitialCreate --project src/zogo.Infrastructure --startup-project src/zogo.API
dotnet ef database update --project src/zogo.Infrastructure --startup-project src/zogo.API
```

### 3. Run the API

```bash
dotnet run --project src/zogo.API
```

Swagger UI: `https://localhost:<port>/swagger`

Health check: `GET /api/v1/health`

### 4. Run tests

```bash
dotnet test
```

## Configuration

| Section | Purpose |
|---------|---------|
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection |
| `Jwt` | Secret, issuer, audience, token expiry |
| `Cors` | Allowed origins, methods, headers |
| `Serilog` | Log levels and sinks |

> **Important:** Replace the JWT secret in production with a secure value (minimum 32 characters).

## Adding Business Features

When building property or other domain APIs:

1. Add entities in `zogo.Domain` (inherit `AuditableEntity`)
2. Add EF configurations in `zogo.Infrastructure/Persistence/Configurations`
3. Add DTOs, validators, and services in `zogo.Application`
4. Add controllers in `zogo.API/Controllers/v1`

## Architecture

```
┌─────────────────────────────────────────┐
│              zogo.API                   │
│  Controllers · Middleware · Swagger     │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│          zogo.Application               │
│  DTOs · Validators · Service Interfaces │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│            zogo.Domain                  │
│  Entities · Repository Interfaces       │
└─────────────────▲───────────────────────┘
                  │
┌─────────────────┴───────────────────────┐
│         zogo.Infrastructure             │
│  EF Core · Repos · JWT · Interceptors   │
└─────────────────────────────────────────┘
```

## License

Proprietary — zogo Realtor
