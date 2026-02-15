using CleanArchitecture.Template.Application.Abstractions.Persistence;
using CleanArchitecture.Template.Application.Abstractions.Specifications;
using CleanArchitecture.Template.Domain.Aggregates;
using CleanArchitecture.Template.Domain.ValueObjects;
using Microsoft.Data.Sqlite;

namespace CleanArchitecture.Template.Infrastructure.Ado.Persistence;

public sealed class AdoCustomerRepository(AdoUnitOfWork uow) :
    IReadRepository<Customer, Guid>,
    IWriteRepository<Customer>
{
    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = """
            SELECT Id, Name, Email, IsActive, IsDeleted, DeletedAt
            FROM Customers
            WHERE Id = $id AND IsDeleted = 0
            LIMIT 1;
            """;

        await using var cmd = new SqliteCommand(sql, uow.Connection, uow.Transaction);
        cmd.Parameters.AddWithValue("$id", id.ToString());

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            return null;

        return Map(reader);
    }

    public async Task<IReadOnlyList<Customer>> ListAsync(CancellationToken ct = default)
    {
        const string sql = """
            SELECT Id, Name, Email, IsActive, IsDeleted, DeletedAt
            FROM Customers
            WHERE IsDeleted = 0;
            """;

        await using var cmd = new SqliteCommand(sql, uow.Connection, uow.Transaction);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var list = new List<Customer>();
        while (await reader.ReadAsync(ct))
            list.Add(Map(reader));

        return list;
    }

    public async Task<Customer?> FirstOrDefaultAsync(ISpecification<Customer> specification, CancellationToken ct = default)
    {
        var list = await ListAsync(specification, ct);
        return list.FirstOrDefault();
    }

    public async Task<IReadOnlyList<Customer>> ListAsync(ISpecification<Customer> specification, CancellationToken ct = default)
    {
        var (where, parameters) = AdoSpecificationTranslator.Translate(specification);

        var sql = $"""
            SELECT Id, Name, Email, IsActive, IsDeleted, DeletedAt
            FROM Customers
            WHERE IsDeleted = 0 {(string.IsNullOrWhiteSpace(where) ? "" : "AND " + where)}
            ORDER BY Name
            {(specification.Take is null ? "" : "LIMIT $take")}
            {(specification.Skip is null ? "" : "OFFSET $skip")};
            """;

        await using var cmd = new SqliteCommand(sql, uow.Connection, uow.Transaction);

        foreach (var p in parameters)
            cmd.Parameters.AddWithValue(p.Key, p.Value);

        if (specification.Take is not null)
            cmd.Parameters.AddWithValue("$take", specification.Take.Value);
        if (specification.Skip is not null)
            cmd.Parameters.AddWithValue("$skip", specification.Skip.Value);

        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var list = new List<Customer>();
        while (await reader.ReadAsync(ct))
            list.Add(Map(reader));

        return list;
    }

    public async Task AddAsync(Customer aggregate, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO Customers (Id, Name, Email, IsActive, IsDeleted, DeletedAt)
            VALUES ($id, $name, $email, $isActive, $isDeleted, $deletedAt);
            """;

        await using var cmd = new SqliteCommand(sql, uow.Connection, uow.Transaction);
        cmd.Parameters.AddWithValue("$id", aggregate.Id.ToString());
        cmd.Parameters.AddWithValue("$name", aggregate.Name);
        cmd.Parameters.AddWithValue("$email", aggregate.Email.Value);
        cmd.Parameters.AddWithValue("$isActive", aggregate.IsActive ? 1 : 0);
        cmd.Parameters.AddWithValue("$isDeleted", aggregate.IsDeleted ? 1 : 0);
        cmd.Parameters.AddWithValue("$deletedAt", aggregate.DeletedAt?.ToUnixTimeSeconds());

        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task RemoveAsync(Customer aggregate, CancellationToken ct = default)
    {
        const string sql = """
            UPDATE Customers
            SET IsDeleted = 1, DeletedAt = $deletedAt
            WHERE Id = $id;
            """;

        await using var cmd = new SqliteCommand(sql, uow.Connection, uow.Transaction);
        cmd.Parameters.AddWithValue("$id", aggregate.Id.ToString());
        cmd.Parameters.AddWithValue("$deletedAt", DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        await cmd.ExecuteNonQueryAsync(ct);
    }

    private static Customer Map(SqliteDataReader reader)
    {
        var id = Guid.Parse(reader.GetString(0));
        var name = reader.GetString(1);
        var email = Email.Create(reader.GetString(2));
        var isActive = reader.GetInt32(3) == 1;
        var isDeleted = reader.GetInt32(4) == 1;
        var deletedAt = reader.IsDBNull(5) ? (DateTimeOffset?)null : DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(5));

        var c = new Customer(id, name, email);
        if (!isActive) c.Deactivate();
        if (isDeleted) c.SoftDelete(deletedAt ?? DateTimeOffset.UtcNow);

        return c;
    }
}
