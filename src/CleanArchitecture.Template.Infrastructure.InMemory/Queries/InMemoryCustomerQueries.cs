using CleanArchitecture.Template.Application.Abstractions.Queries;
using CleanArchitecture.Template.Application.Customers.Dtos;

namespace CleanArchitecture.Template.Infrastructure.InMemory.Queries;

public sealed class InMemoryCustomerQueries : ICustomerQueries
{
    private readonly InMemoryStore _store;

    public InMemoryCustomerQueries(InMemoryStore store) => _store = store;

    public Task<CustomerDetailsDto?> GetDetailsAsync(Guid id, CancellationToken ct = default)
    {
        var c = _store.TryGetCustomer(id);
        if (c != null && !c.IsDeleted)
            return Task.FromResult<CustomerDetailsDto?>(new CustomerDetailsDto(c.Id, c.Name, c.Email.Value, c.IsActive));

        return Task.FromResult<CustomerDetailsDto?>(null);
    }

    public Task<PagedResult<CustomerListItemDto>> SearchAsync(CustomerSearchQuery query, CancellationToken ct = default)
    {
        var term = query.Term?.Trim();
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 200);

        var q = _store.Customers.Where(x => !x.IsDeleted && x.IsActive);

        if (!string.IsNullOrWhiteSpace(term))
            q = q.Where(x => x.Name.Contains(term) || x.Email.Value.Contains(term));

        var total = q.Count();

        var items = q.OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new CustomerListItemDto(x.Id, x.Name, x.Email.Value))
            .ToList();

        return Task.FromResult(new PagedResult<CustomerListItemDto>(items, total, page, pageSize));
    }
}
