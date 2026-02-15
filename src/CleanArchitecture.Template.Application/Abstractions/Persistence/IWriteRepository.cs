using CleanArchitecture.Template.Domain.Abstractions;

namespace CleanArchitecture.Template.Application.Abstractions.Persistence;

public interface IWriteRepository<TAggregate>
    where TAggregate : class, IAggregateRoot
{
    Task AddAsync(TAggregate aggregate, CancellationToken ct = default);
    Task RemoveAsync(TAggregate aggregate, CancellationToken ct = default);
}
