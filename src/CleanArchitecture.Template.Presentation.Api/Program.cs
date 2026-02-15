using CleanArchitecture.Template.Application;
using CleanArchitecture.Template.Application.Abstractions.Queries;
using CleanArchitecture.Template.Application.Customers.Dtos;
using CleanArchitecture.Template.Application.Customers.UseCases.CreateCustomer;
using CleanArchitecture.Template.Application.Customers.UseCases.DeleteCustomer;
using CleanArchitecture.Template.Application.Customers.UseCases.DeleteCustomers;
using CleanArchitecture.Template.Application.Customers.UseCases.GetCustomer;
using CleanArchitecture.Template.Application.Customers.UseCases.UpdateCustomer;
using CleanArchitecture.Template.Contracts.Persistence;
using CleanArchitecture.Template.Infrastructure.Ado;
using CleanArchitecture.Template.Infrastructure.EfCore;
using CleanArchitecture.Template.Infrastructure.EfCore.Persistence;
using CleanArchitecture.Template.Infrastructure.InMemory;
using CleanArchitecture.Template.Presentation.Api;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddPresentation();

var persistence = builder.Configuration.GetPersistenceOptions();

switch (persistence.Provider)
{
    case PersistenceProvider.Ado:
        builder.Services.AddAdoInfrastructure(builder.Configuration);
        break;

    case PersistenceProvider.InMemory:
        builder.Services.AddInMemoryInfrastructure(builder.Configuration);
        break;

    case PersistenceProvider.Ef:
    default:
        builder.Services.AddEfCoreInfrastructure(builder.Configuration);
        break;
}

var app = builder.Build();

// bootstrap persistence (template convenience)
switch (persistence.Provider)
{
    case PersistenceProvider.Ado:
        {
            var cs = persistence.ConnectionString
                     ?? app.Configuration.GetConnectionString("AppDb")
                     ?? "Data Source=app.db";
            await DatabaseBootstrapper.EnsureSchemaAsync(cs);
            break;
        }

    case PersistenceProvider.InMemory:
        break;

    case PersistenceProvider.Ef:
    default:
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // If no EF migrations exist yet, fall back to EnsureCreated (dev-friendly).
            // If migrations exist, use Migrate to keep schema in sync.
            var appliedMigrations = await db.Database.GetAppliedMigrationsAsync();
            var pendingMigrations = await db.Database.GetPendingMigrationsAsync();
            var hasMigrations = appliedMigrations.Any() || pendingMigrations.Any();
            if (!hasMigrations)
                await db.Database.EnsureCreatedAsync();
            else
                await db.Database.MigrateAsync();

            break;
        }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        c.RoutePrefix = "swagger";
    });
}

app.MapGet("/", () => Results.Ok(new { Status = "OK", Provider = persistence.Provider.ToString() }));

app.MapPost("/customers", async (CreateCustomerCommand cmd, CreateCustomerHandler handler, CancellationToken ct) =>
{
    var result = await handler.HandleAsync(cmd, ct);
    return result.IsSuccess
        ? Results.Created($"/customers/{result.Value}", new { Id = result.Value })
        : Results.BadRequest(result.Error);
});

app.MapGet("/customers/{id:guid}", async (Guid id, GetCustomerHandler handler, CancellationToken ct) =>
{
    var result = await handler.HandleAsync(new GetCustomerQuery(id), ct);
    return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Error);
});

app.MapGet("/customers", async (string? term, int page, int pageSize, ICustomerQueries queries, CancellationToken ct) =>
{
    var result = await queries.SearchAsync(new CustomerSearchQuery(term, page, pageSize), ct);
    return Results.Ok(result);
});

app.MapPut("/customers/{id:guid}", async (Guid id, UpdateCustomerCommand body, UpdateCustomerHandler handler, CancellationToken ct) =>
{
    var cmd = body with { Id = id };
    var result = await handler.HandleAsync(cmd, ct);
    return result.IsSuccess ? Results.NoContent() : Results.NotFound(result.Error);
});

app.MapDelete("/customers/{id:guid}", async (Guid id, DeleteCustomerHandler handler, CancellationToken ct) =>
{
    var result = await handler.HandleAsync(id, ct);
    return result.IsSuccess ? Results.NoContent() : Results.NotFound(result.Error);
});

app.MapDelete("/customers", async (Guid[] ids, DeleteCustomersHandler handler, CancellationToken ct) =>
{
    var result = await handler.HandleAsync(new DeleteCustomersCommand(ids), ct);
    return result.IsSuccess ? Results.Ok(new { Deleted = result.Value }) : Results.BadRequest(result.Error);
});

app.Run();

internal static class DatabaseBootstrapper
{
    public static async Task EnsureSchemaAsync(string connectionString)
    {
        const string sql = @"
CREATE TABLE IF NOT EXISTS Customers (
    Id TEXT NOT NULL PRIMARY KEY,
    Name TEXT NOT NULL,
    Email TEXT NOT NULL,
    IsActive INTEGER NOT NULL,
    IsDeleted INTEGER NOT NULL,
    DeletedAt INTEGER NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS IX_Customers_Email_NotDeleted
ON Customers(Email)
WHERE IsDeleted = 0;";

        await using var con = new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
        await con.OpenAsync();
        await using var cmd = new Microsoft.Data.Sqlite.SqliteCommand(sql, con);
        await cmd.ExecuteNonQueryAsync();
    }
}
