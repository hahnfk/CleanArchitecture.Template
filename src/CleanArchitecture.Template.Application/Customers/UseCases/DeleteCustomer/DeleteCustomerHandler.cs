using CleanArchitecture.Template.Application.Abstractions.Persistence;
using CleanArchitecture.Template.Application.Primitives;
using CleanArchitecture.Template.Domain.Aggregates;

namespace CleanArchitecture.Template.Application.Customers.UseCases.DeleteCustomer;

public sealed class DeleteCustomerHandler
{
    private readonly IReadRepository<Customer, Guid> _readRepo;
    private readonly IWriteRepository<Customer> _writeRepo;
    private readonly IUnitOfWork _uow;

    public DeleteCustomerHandler(
        IReadRepository<Customer, Guid> readRepo,
        IWriteRepository<Customer> writeRepo,
        IUnitOfWork uow)
    {
        _readRepo = readRepo;
        _writeRepo = writeRepo;
        _uow = uow;
    }

    public async Task<Result<bool>> HandleAsync(Guid id, CancellationToken ct = default)
    {
        var customer = await _readRepo.GetByIdAsync(id, ct);
        if (customer is null)
            return Result<bool>.Fail("customer.not_found", "Customer not found.");

        await _writeRepo.RemoveAsync(customer, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<bool>.Ok(true);
    }
}
