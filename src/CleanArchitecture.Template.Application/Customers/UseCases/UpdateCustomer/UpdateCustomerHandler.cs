using CleanArchitecture.Template.Application.Abstractions.Persistence;
using CleanArchitecture.Template.Application.Primitives;
using CleanArchitecture.Template.Domain.Aggregates;
using CleanArchitecture.Template.Domain.ValueObjects;

namespace CleanArchitecture.Template.Application.Customers.UseCases.UpdateCustomer;

public sealed class UpdateCustomerHandler
{
    private readonly IReadRepository<Customer, Guid> _readRepo;
    private readonly IUnitOfWork _uow;

    public UpdateCustomerHandler(IReadRepository<Customer, Guid> readRepo, IUnitOfWork uow)
    {
        _readRepo = readRepo;
        _uow = uow;
    }

    public async Task<Result<Unit>> HandleAsync(UpdateCustomerCommand command, CancellationToken ct = default)
    {
        try
        {
            var customer = await _readRepo.GetByIdAsync(command.Id, ct);
            if (customer is null)
                return Result<Unit>.Fail("customer.not_found", "Customer not found.");

            customer.Rename(command.Name);
            customer.ChangeEmail(Email.Create(command.Email));

            await _uow.SaveChangesAsync(ct);
            return Result<Unit>.Ok(new Unit());
        }
        catch (Exception ex)
        {
            return Result<Unit>.Fail("customer.update_failed", ex.Message);
        }
    }
}
