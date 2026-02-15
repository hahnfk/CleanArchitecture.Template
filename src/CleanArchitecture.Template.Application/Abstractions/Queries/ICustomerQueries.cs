using CleanArchitecture.Template.Application.Customers.Dtos;

namespace CleanArchitecture.Template.Application.Abstractions.Queries;

public interface ICustomerQueries
{
    Task<CustomerDetailsDto?> GetDetailsAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<CustomerListItemDto>> SearchAsync(CustomerSearchQuery query, CancellationToken ct = default);
}
