namespace CleanArchitecture.Template.Application.Customers.UseCases.DeleteCustomers;

public sealed record DeleteCustomersCommand(IReadOnlyCollection<Guid> Ids);
