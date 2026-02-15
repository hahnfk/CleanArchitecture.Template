using CleanArchitecture.Template.Application.Abstractions.Specifications;
using CleanArchitecture.Template.Domain.Aggregates;

namespace CleanArchitecture.Template.Application.Customers.Specifications;

public sealed class CustomerByIdSpec : Specification<Customer>
{
    public CustomerByIdSpec(Guid id) => Criteria = x => x.Id == id;
}
