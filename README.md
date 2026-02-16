
# CleanArchitecture.Template

Clean Architecture + DDD (Aggregates) + pluggable persistence (InMemory / EF Core / ADO.NET) + CQRS-ready queries.

## Prerequisites
- .NET SDK 10.0.x (see `global.json`)
- Visual Studio 2026 / VS Code

## Quick start

```bash
dotnet restore
dotnet build
dotnet test

dotnet run --project src/CleanArchitecture.Template.Presentation.Api
```

Then open:
- OpenAPI JSON: `https://localhost:5001/openapi/v1.json` (development)
- Swagger UI: `https://localhost:5001/swagger`

## Project structure

- `src/CleanArchitecture.Template.Domain` — aggregates, value objects, domain errors
- `src/CleanArchitecture.Template.Application` — use cases, repository contracts, DTOs, result pattern
- `src/CleanArchitecture.Template.Contracts` — shared options & enums (e.g. `PersistenceOptions`)
- `src/CleanArchitecture.Template.Infrastructure.EfCore` — EF Core DbContext, repository & query implementations (SQLite)
- `src/CleanArchitecture.Template.Infrastructure.Ado` — ADO.NET (Microsoft.Data.Sqlite) repository & query implementations
- `src/CleanArchitecture.Template.Infrastructure.InMemory` — in-memory repository & query implementations (no database required)
- `src/CleanArchitecture.Template.Presentation.Api` — Minimal API endpoints
- `tests/CleanArchitecture.Template.Domain.UnitTests` — domain tests (xUnit v3)
- `tests/CleanArchitecture.Template.Infrastructure.IntegrationTests` — EF Core integration tests (SQLite)

## Notes

- Repositories do **not** call `SaveChanges`. Use `IUnitOfWork` in your use cases.
- Reads (search/paging/projections) are implemented as query services to keep repositories lean.

## Persistence providers

The provider is selected via `appsettings.json` → `Persistence:Provider`.
By default, the template uses **InMemory** — no database setup required to get started.

```json
{ 
    "Persistence": { 
        "Provider": "InMemory" 
    }
}
```

Valid values:
- `"InMemory"` (default) — all data lives in memory; ideal for prototyping & tests
- `"Ef"` — EF Core with SQLite (requires a connection string & migrations)
- `"Ado"` — ADO.NET with Microsoft.Data.Sqlite

When using `Ef` or `Ado`, provide a connection string:

```json
{ 
    "Persistence": { 
        "Provider": "Ef", 
        "ConnectionString": "Data Source=app.db" 
    } 
}
```

## EF Core migrations

Only relevant when `Provider` is set to `"Ef"`. From the repository root:

```bash
Add a migration (writes to Infrastructure.EfCore/Persistence/Migrations)

dotnet ef migrations add InitialCreate 
-p src/CleanArchitecture.Template.Infrastructure.EfCore 
-s src/CleanArchitecture.Template.Presentation.Api 
-o Persistence/Migrations

Apply migrations to the configured database

dotnet ef database update 
-p src/CleanArchitecture.Template.Infrastructure.EfCore 
-s src/CleanArchitecture.Template.Presentation.Api
```

> Tip: you can override the connection string for tooling via `APPDB_CONNECTIONSTRING`
> (see `AppDbContextFactory`).
