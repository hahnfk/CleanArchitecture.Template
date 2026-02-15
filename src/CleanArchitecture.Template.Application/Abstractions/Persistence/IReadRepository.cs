using CleanArchitecture.Template.Domain.Abstractions;

using CleanArchitecture.Template.Application.Abstractions.Specifications;

namespace CleanArchitecture.Template.Application.Abstractions.Persistence;

public interface IReadRepository<TAggregate, TId>
    where TAggregate : class, IAggregateRoot
{
    Task<TAggregate?> GetByIdAsync(TId id, CancellationToken ct = default);
    Task<IReadOnlyList<TAggregate>> ListAsync(CancellationToken ct = default);

    Task<TAggregate?> FirstOrDefaultAsync(ISpecification<TAggregate> specification, CancellationToken ct = default);
    Task<IReadOnlyList<TAggregate>> ListAsync(ISpecification<TAggregate> specification, CancellationToken ct = default);
}
