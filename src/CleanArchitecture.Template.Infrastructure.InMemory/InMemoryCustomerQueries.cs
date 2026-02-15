using CleanArchitecture.Template.Application.Abstractions.Queries;
using CleanArchitecture.Template.Application.Customers.Dtos;
using CleanArchitecture.Template.Domain.Aggregates;

namespace CleanArchitecture.Template.Infrastructure.InMemory;

public sealed class InMemoryCustomerQueries : ICustomerQueries
{
    private readonly InMemoryStore _store;

    public InMemoryCustomerQueries(InMemoryStore store) => _store = store;

    public Task<CustomerDetailsDto?> GetDetailsAsync(Guid id, CancellationToken ct = default)
    {
        var c = _store.TryGetCustomer(id);
        if (c is null || c.IsDeleted) return Task.FromResult<CustomerDetailsDto?>(null);

        return Task.FromResult<CustomerDetailsDto?>(new CustomerDetailsDto(c.Id, c.Name, c.Email.Value, c.IsActive));
    }

    public Task<PagedResult<CustomerListItemDto>> SearchAsync(CustomerSearchQuery query, CancellationToken ct = default)
    {
        var term = query.Term?.Trim();

        IEnumerable<Customer> customers = _store.Customers.Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(term))
        {
            customers = customers.Where(c =>
                c.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                c.Email.Value.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        var total = customers.Count();
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 200);

        var items = customers
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CustomerListItemDto(c.Id, c.Name, c.Email.Value))
            .ToList();

        return Task.FromResult(new PagedResult<CustomerListItemDto>(items, total, page, pageSize));
    }
}
