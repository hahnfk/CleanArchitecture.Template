using CleanArchitecture.Template.Application.Abstractions.Persistence;
using CleanArchitecture.Template.Application.Abstractions.Specifications;
using CleanArchitecture.Template.Domain.Aggregates;

namespace CleanArchitecture.Template.Infrastructure.InMemory;

public sealed class InMemoryCustomerRepository :
    IReadRepository<Customer, Guid>,
    IWriteRepository<Customer>
{
    private readonly InMemoryStore _store;

    public InMemoryCustomerRepository(InMemoryStore store) => _store = store;

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var customer = _store.TryGetCustomer(id);
        if (customer is null || customer.IsDeleted) return Task.FromResult<Customer?>(null);
        return Task.FromResult<Customer?>(customer);
    }

    public Task<IReadOnlyList<Customer>> ListAsync(CancellationToken ct = default)
    {
        var list = _store.Customers.Where(c => !c.IsDeleted).ToList();
        return Task.FromResult<IReadOnlyList<Customer>>(list);
    }

    public Task<Customer?> FirstOrDefaultAsync(ISpecification<Customer> specification, CancellationToken ct = default)
        => Task.FromResult(ApplySpecification(specification).FirstOrDefault());

    public Task<IReadOnlyList<Customer>> ListAsync(ISpecification<Customer> specification, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Customer>>(ApplySpecification(specification).ToList());

    public Task AddAsync(Customer aggregate, CancellationToken ct = default)
    {
        _store.Upsert(aggregate);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Customer aggregate, CancellationToken ct = default)
    {
        // Soft-delete to mimic EF global filter behavior.
        aggregate.SoftDelete(DateTimeOffset.UtcNow);
        _store.Upsert(aggregate);
        return Task.CompletedTask;
    }

    private IEnumerable<Customer> ApplySpecification(ISpecification<Customer> specification)
    {
        IEnumerable<Customer> query = _store.Customers.Where(c => !c.IsDeleted);

        if (specification.Criteria is not null)
            query = query.Where(specification.Criteria.Compile());

        if (specification.OrderBy is not null)
            query = query.OrderBy(specification.OrderBy.Compile());
        else if (specification.OrderByDescending is not null)
            query = query.OrderByDescending(specification.OrderByDescending.Compile());

        if (specification.Skip is not null)
            query = query.Skip(specification.Skip.Value);

        if (specification.Take is not null)
            query = query.Take(specification.Take.Value);

        return query;
    }
}
