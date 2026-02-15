using CleanArchitecture.Template.Application.Abstractions.Persistence;
using CleanArchitecture.Template.Application.Primitives;
using CleanArchitecture.Template.Domain.Aggregates;
using CleanArchitecture.Template.Domain.ValueObjects;

namespace CleanArchitecture.Template.Application.Customers.UseCases.CreateCustomer;

public sealed class CreateCustomerHandler
{
    private readonly IWriteRepository<Customer> _repo;
    private readonly IUnitOfWork _uow;

    public CreateCustomerHandler(IWriteRepository<Customer> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<Guid>> HandleAsync(CreateCustomerCommand command, CancellationToken ct = default)
    {
        try
        {
            var customer = new Customer(Guid.NewGuid(), command.Name, Email.Create(command.Email));
            await _repo.AddAsync(customer, ct);
            await _uow.SaveChangesAsync(ct);
            return Result<Guid>.Ok(customer.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Fail("customer.create_failed", ex.Message);
        }
    }
}
