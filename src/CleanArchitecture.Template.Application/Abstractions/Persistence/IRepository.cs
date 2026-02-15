using CleanArchitecture.Template.Domain.Abstractions;

namespace CleanArchitecture.Template.Application.Abstractions.Persistence;

public interface IRepository<TAggregate, TId> :
    IReadRepository<TAggregate, TId>,
    IWriteRepository<TAggregate>
    where TAggregate : class, IAggregateRoot
{
}
