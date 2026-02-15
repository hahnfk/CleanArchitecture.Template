using CleanArchitecture.Template.Application.Customers.Dtos;
using CleanArchitecture.Template.Domain.Aggregates;
using CleanArchitecture.Template.Domain.ValueObjects;
using CleanArchitecture.Template.Infrastructure.EfCore.Persistence;
using CleanArchitecture.Template.Infrastructure.EfCore.Queries;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CleanArchitecture.Template.Infrastructure.IntegrationTests;

public sealed class CustomerQueriesTests
{
    [Fact]
    public async Task Search_excludes_soft_deleted_customers_via_global_filter()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

        var alice = new Customer(Guid.NewGuid(), "Alice", Email.Create("alice@example.com"));
        var bob = new Customer(Guid.NewGuid(), "Bob", Email.Create("bob@example.com"));
        bob.SoftDelete(DateTimeOffset.UtcNow);

        db.Customers.Add(alice);
        db.Customers.Add(bob);

        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var queries = new CustomerQueries(db);
        var result = await queries.SearchAsync(new CustomerSearchQuery(null, 1, 50), TestContext.Current.CancellationToken);

        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal("Alice", result.Items[0].Name);
    }
}
