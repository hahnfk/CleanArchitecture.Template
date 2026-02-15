using CleanArchitecture.Template.Application.Abstractions.Specifications;
using CleanArchitecture.Template.Domain.Aggregates;

namespace CleanArchitecture.Template.Application.Customers.Specifications;

public sealed class ActiveCustomersSearchSpec : Specification<Customer>
{
    public ActiveCustomersSearchSpec(string? term, int page, int pageSize)
    {
        term = term?.Trim();

        Criteria = string.IsNullOrWhiteSpace(term)
            ? x => x.IsActive
            : x => x.IsActive && (x.Name.Contains(term) || x.Email.Value.Contains(term));

        OrderBy = x => x.Name;

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);

        Skip = (page - 1) * pageSize;
        Take = pageSize;
    }
}
