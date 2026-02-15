namespace CleanArchitecture.Template.Application.Customers.UseCases.UpdateCustomer;

public sealed record UpdateCustomerCommand(Guid Id, string Name, string Email);
