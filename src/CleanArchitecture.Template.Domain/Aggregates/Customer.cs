using CleanArchitecture.Template.Domain.Abstractions;
using CleanArchitecture.Template.Domain.Primitives;
using CleanArchitecture.Template.Domain.ValueObjects;

namespace CleanArchitecture.Template.Domain.Aggregates;

public sealed class Customer : IAggregateRoot, ISoftDeletable
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Email Email { get; private set; } = Email.Create("nobody@example.com");
    public bool IsActive { get; private set; } = true;

    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    // EF Core
    private Customer() { }

    public Customer(Guid id, string name, Email email)
    {
        Id = id;
        Rename(name);
        ChangeEmail(email);
        IsActive = true;
        IsDeleted = false;
        DeletedAt = null;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Customer name must not be empty.");

        Name = name.Trim();
    }

    public void ChangeEmail(Email email)
        => Email = email ?? throw new DomainException("Email must not be null.");

    public void Deactivate()
        => IsActive = false;

    public void SoftDelete(DateTimeOffset utcNow)
    {
        IsDeleted = true;
        DeletedAt = utcNow;
    }
}
