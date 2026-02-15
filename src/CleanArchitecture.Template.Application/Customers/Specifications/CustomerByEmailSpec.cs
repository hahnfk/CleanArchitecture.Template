using CleanArchitecture.Template.Application.Abstractions.Specifications;
using CleanArchitecture.Template.Domain.Aggregates;

namespace CleanArchitecture.Template.Application.Customers.Specifications;

public sealed class CustomerByEmailSpec : Specification<Customer>
{
    public CustomerByEmailSpec(string email)
        => Criteria = x => x.Email.Value == email;
}
