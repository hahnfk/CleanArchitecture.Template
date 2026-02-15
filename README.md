# CleanArchitecture.Template

Clean Architecture + DDD (Aggregates) + EF Core (SQLite) + CQRS-ready queries.

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
- `src/CleanArchitecture.Template.Infrastructure.EfCore` — EF Core DbContext, repository implementations, query implementations
- `src/CleanArchitecture.Template.Presentation.Api` — Minimal API endpoints
- `tests/CleanArchitecture.Template.Domain.UnitTests` — domain tests (xUnit v3)
- `tests/CleanArchitecture.Template.Infrastructure.IntegrationTests` — EF Core integration tests (SQLite)

## Notes

- Repositories do **not** call `SaveChanges`. Use `IUnitOfWork` in your use cases.
- Reads (search/paging/projections) are implemented as query services to keep repositories lean.

## EF Core migrations

From the repository root:

```bash
# Add a migration (writes to Infrastructure.EfCore/Persistence/Migrations)
dotnet ef migrations add InitialCreate \
  -p src/CleanArchitecture.Template.Infrastructure.EfCore \
  -s src/CleanArchitecture.Template.Presentation.Api \
  -o Persistence/Migrations

# Apply migrations to the configured database
dotnet ef database update \
  -p src/CleanArchitecture.Template.Infrastructure.EfCore \
  -s src/CleanArchitecture.Template.Presentation.Api
```

> Tip: you can override the connection string for tooling via `APPDB_CONNECTIONSTRING`
> (see `AppDbContextFactory`).

## Switching between EF Core and ADO.NET

By default, the template uses EF Core for repositories and queries.

To use the ADO.NET (Microsoft.Data.Sqlite) implementation, set:

```json
{
  "DataAccess": {
    "Provider": "Ado"
  }
}
```

Valid values:
- `"Ef"` (default)
- `"Ado"`
