using CleanArchitecture.Template.Application.Abstractions.Persistence;
using CleanArchitecture.Template.Application.Customers.Dtos;
using CleanArchitecture.Template.Application.Primitives;
using CleanArchitecture.Template.Domain.Aggregates;

namespace CleanArchitecture.Template.Application.Customers.UseCases.ReadCustomers;

public sealed class ReadCustomersByIdsHandler
{
    private readonly IReadRepository<Customer, Guid> _readRepo;

    public ReadCustomersByIdsHandler(IReadRepository<Customer, Guid> readRepo) => _readRepo = readRepo;

    public async Task<Result<IReadOnlyList<CustomerDetailsDto>>> HandleAsync(ReadCustomersByIdsQuery query, CancellationToken ct = default)
    {
        if (query.Ids.Count == 0)
            return Result<IReadOnlyList<CustomerDetailsDto>>.Ok(Array.Empty<CustomerDetailsDto>());

        // Simple approach: fetch individually (works for all providers incl. ADO/InMemory)
        // For real-world usage you may prefer a Specification with an "IN" predicate for EF Core.
        var list = new List<CustomerDetailsDto>(query.Ids.Count);

        foreach (var id in query.Ids.Distinct())
        {
            var customer = await _readRepo.GetByIdAsync(id, ct);
            if (customer is null) continue;

            list.Add(new CustomerDetailsDto(customer.Id, customer.Name, customer.Email.Value, customer.IsActive));
        }

        return Result<IReadOnlyList<CustomerDetailsDto>>.Ok(list);
    }
}
