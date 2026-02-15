using CleanArchitecture.Template.Application.Abstractions.Queries;
using CleanArchitecture.Template.Application.Customers.Dtos;
using CleanArchitecture.Template.Application.Customers.UseCases.CreateCustomer;
using CleanArchitecture.Template.Application.Customers.UseCases.DeleteCustomer;
using CleanArchitecture.Template.Infrastructure.Ado;
using CleanArchitecture.Template.Infrastructure.EfCore;
using CleanArchitecture.Template.Infrastructure.EfCore.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

var provider = builder.Configuration.GetValue<string>("DataAccess:Provider") ?? "Ef";

if (string.Equals(provider, "Ado", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddAdoInfrastructure(builder.Configuration);
}
else
{
    builder.Services.AddEfCoreInfrastructure(builder.Configuration);
}

builder.Services.AddScoped<CreateCustomerHandler>();
builder.Services.AddScoped<DeleteCustomerHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CleanArchitecture.Template API",
        Version = "v1"
    });
});

var app = builder.Build();

if (string.Equals(provider, "Ado", StringComparison.OrdinalIgnoreCase))
{
    var cs = app.Configuration.GetConnectionString("AppDb") ?? "Data Source=app.db";
    await DatabaseBootstrapper.EnsureSchemaAsync(cs);
}
else
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
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

app.MapGet("/", () => Results.Ok(new { Status = "OK", Provider = provider }));

app.MapPost("/customers", async (CreateCustomerCommand cmd, CreateCustomerHandler handler, CancellationToken ct) =>
{
    var result = await handler.HandleAsync(cmd, ct);
    return result.IsSuccess
        ? Results.Created($"/customers/{result.Value}", new { Id = result.Value })
        : Results.BadRequest(result.Error);
});

app.MapGet("/customers/{id:guid}", async (Guid id, ICustomerQueries queries, CancellationToken ct) =>
{
    var dto = await queries.GetDetailsAsync(id, ct);
    return dto is null ? Results.NotFound() : Results.Ok(dto);
});

app.MapGet("/customers", async (string? term, int page, int pageSize, ICustomerQueries queries, CancellationToken ct) =>
{
    var result = await queries.SearchAsync(new CustomerSearchQuery(term, page, pageSize), ct);
    return Results.Ok(result);
});

app.MapDelete("/customers/{id:guid}", async (Guid id, DeleteCustomerHandler handler, CancellationToken ct) =>
{
    var result = await handler.HandleAsync(id, ct);
    return result.IsSuccess ? Results.NoContent() : Results.NotFound(result.Error);
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
