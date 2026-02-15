namespace CleanArchitecture.Template.Application.Customers.UseCases.ReadCustomers;

public sealed record ReadCustomersByIdsQuery(IReadOnlyCollection<Guid> Ids);
