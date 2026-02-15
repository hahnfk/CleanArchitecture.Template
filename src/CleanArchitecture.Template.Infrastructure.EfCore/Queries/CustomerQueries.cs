using CleanArchitecture.Template.Application.Abstractions.Queries;
using CleanArchitecture.Template.Application.Customers.Dtos;
using CleanArchitecture.Template.Infrastructure.EfCore.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Template.Infrastructure.EfCore.Queries;

public sealed class CustomerQueries : ICustomerQueries
{
    private readonly AppDbContext _db;

    public CustomerQueries(AppDbContext db) => _db = db;

    public Task<CustomerDetailsDto?> GetDetailsAsync(Guid id, CancellationToken ct = default)
        => _db.Customers.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new CustomerDetailsDto(x.Id, x.Name, x.Email.Value, x.IsActive))
            .SingleOrDefaultAsync(ct);

    public async Task<PagedResult<CustomerListItemDto>> SearchAsync(CustomerSearchQuery query, CancellationToken ct = default)
    {
        var term = query.Term?.Trim();
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 200);

        var q = _db.Customers.AsNoTracking().Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(term))
        {
            q = q.Where(x =>
                x.Name.Contains(term) ||
                x.Email.Value.Contains(term));
        }

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new CustomerListItemDto(x.Id, x.Name, x.Email.Value))
            .ToListAsync(ct);

        return new PagedResult<CustomerListItemDto>(items, total, page, pageSize);
    }
}
