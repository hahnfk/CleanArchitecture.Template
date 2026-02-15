namespace CleanArchitecture.Template.Domain.Abstractions;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTimeOffset? DeletedAt { get; }
}
