using CleanArchitecture.Template.Application.Abstractions.Persistence;
using CleanArchitecture.Template.Application.Primitives;
using CleanArchitecture.Template.Domain.Aggregates;

namespace CleanArchitecture.Template.Application.Customers.UseCases.DeleteCustomers;

public sealed class DeleteCustomersHandler
{
    private readonly IReadRepository<Customer, Guid> _readRepo;
    private readonly IWriteRepository<Customer> _writeRepo;
    private readonly IUnitOfWork _uow;

    public DeleteCustomersHandler(
        IReadRepository<Customer, Guid> readRepo,
        IWriteRepository<Customer> writeRepo,
        IUnitOfWork uow)
    {
        _readRepo = readRepo;
        _writeRepo = writeRepo;
        _uow = uow;
    }

    public async Task<Result<int>> HandleAsync(DeleteCustomersCommand command, CancellationToken ct = default)
    {
        if (command.Ids.Count == 0)
            return Result<int>.Ok(0);

        var deleted = 0;

        foreach (var id in command.Ids.Distinct())
        {
            var customer = await _readRepo.GetByIdAsync(id, ct);
            if (customer is null)
                continue;

            await _writeRepo.RemoveAsync(customer, ct);
            deleted++;
        }

        await _uow.SaveChangesAsync(ct);
        return Result<int>.Ok(deleted);
    }
}
