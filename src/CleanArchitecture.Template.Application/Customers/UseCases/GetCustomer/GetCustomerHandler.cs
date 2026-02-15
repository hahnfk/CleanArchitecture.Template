using CleanArchitecture.Template.Application.Abstractions.Queries;
using CleanArchitecture.Template.Application.Customers.Dtos;
using CleanArchitecture.Template.Application.Primitives;

namespace CleanArchitecture.Template.Application.Customers.UseCases.GetCustomer;

public sealed class GetCustomerHandler
{
    private readonly ICustomerQueries _queries;

    public GetCustomerHandler(ICustomerQueries queries) => _queries = queries;

    public async Task<Result<CustomerDetailsDto>> HandleAsync(Guid id, CancellationToken ct = default)
    {
        var dto = await _queries.GetDetailsAsync(id, ct);
        return dto is null
            ? Result<CustomerDetailsDto>.Fail("customer.not_found", "Customer not found.")
            : Result<CustomerDetailsDto>.Ok(dto);
    }
}
