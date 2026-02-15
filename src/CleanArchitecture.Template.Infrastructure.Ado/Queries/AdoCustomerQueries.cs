using CleanArchitecture.Template.Application.Abstractions.Queries;
using CleanArchitecture.Template.Application.Customers.Dtos;
using Microsoft.Data.Sqlite;

namespace CleanArchitecture.Template.Infrastructure.Ado.Queries;

public sealed class AdoCustomerQueries : ICustomerQueries
{
    private readonly string _connectionString;

    public AdoCustomerQueries(string connectionString) => _connectionString = connectionString;

    public async Task<CustomerDetailsDto?> GetDetailsAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = @"
SELECT Id, Name, Email, IsActive
FROM Customers
WHERE Id = $id AND IsDeleted = 0
LIMIT 1;";

        await using var con = new SqliteConnection(_connectionString);
        await con.OpenAsync(ct);

        await using var cmd = new SqliteCommand(sql, con);
        cmd.Parameters.AddWithValue("$id", id.ToString());

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            return null;

        return new CustomerDetailsDto(
            Guid.Parse(reader.GetString(0)),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetInt32(3) == 1);
    }

    public async Task<PagedResult<CustomerListItemDto>> SearchAsync(CustomerSearchQuery query, CancellationToken ct = default)
    {
        var term = query.Term?.Trim();
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 200);
        var skip = (page - 1) * pageSize;

        var where = "IsDeleted = 0 AND IsActive = 1";
        var hasTerm = !string.IsNullOrWhiteSpace(term);

        if (hasTerm)
            where += " AND (Name LIKE $term OR Email LIKE $term)";

        var sqlCount = $"SELECT COUNT(1) FROM Customers WHERE {where};";
        var sqlItems = $@"
SELECT Id, Name, Email
FROM Customers
WHERE {where}
ORDER BY Name
LIMIT $take OFFSET $skip;";

        await using var con = new SqliteConnection(_connectionString);
        await con.OpenAsync(ct);

        await using var countCmd = new SqliteCommand(sqlCount, con);
        if (hasTerm) countCmd.Parameters.AddWithValue("$term", $"%{term}%");
        var total = Convert.ToInt32(await countCmd.ExecuteScalarAsync(ct));

        await using var itemsCmd = new SqliteCommand(sqlItems, con);
        if (hasTerm) itemsCmd.Parameters.AddWithValue("$term", $"%{term}%");
        itemsCmd.Parameters.AddWithValue("$take", pageSize);
        itemsCmd.Parameters.AddWithValue("$skip", skip);

        await using var reader = await itemsCmd.ExecuteReaderAsync(ct);

        var items = new List<CustomerListItemDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new CustomerListItemDto(
                Guid.Parse(reader.GetString(0)),
                reader.GetString(1),
                reader.GetString(2)));
        }

        return new PagedResult<CustomerListItemDto>(items, total, page, pageSize);
    }
}
