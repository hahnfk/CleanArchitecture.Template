using CleanArchitecture.Template.Domain.Aggregates;
using CleanArchitecture.Template.Domain.Primitives;
using CleanArchitecture.Template.Domain.ValueObjects;
using Xunit;

namespace CleanArchitecture.Template.Domain.UnitTests;

public sealed class CustomerTests
{
    [Fact]
    public void Creating_customer_with_invalid_email_throws()
    {
        Assert.Throws<DomainException>(() =>
        {
            _ = new Customer(Guid.NewGuid(), "Frank", Email.Create("not-an-email"));
        });
    }

    [Fact]
    public void Rename_trims_and_sets_name()
    {
        var c = new Customer(Guid.NewGuid(), "  Frank  ", Email.Create("frank@example.com"));
        Assert.Equal("Frank", c.Name);

        c.Rename("  Alice  ");
        Assert.Equal("Alice", c.Name);
    }

    [Fact]
    public void SoftDelete_sets_flags()
    {
        var c = new Customer(Guid.NewGuid(), "Alice", Email.Create("alice@example.com"));
        var now = DateTimeOffset.UtcNow;

        c.SoftDelete(now);

        Assert.True(c.IsDeleted);
        Assert.Equal(now, c.DeletedAt);
    }
}
