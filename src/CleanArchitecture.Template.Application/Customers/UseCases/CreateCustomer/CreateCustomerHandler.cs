using CleanArchitecture.Template.Application.Abstractions.Persistence;
using CleanArchitecture.Template.Application.Customers.Specifications;
using CleanArchitecture.Template.Application.Primitives;
using CleanArchitecture.Template.Domain.Aggregates;
using CleanArchitecture.Template.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Template.Application.Customers.UseCases.CreateCustomer;

public sealed class CreateCustomerHandler
{
    private readonly IReadRepository<Customer, Guid> _readRepo;
    private readonly IWriteRepository<Customer> _writeRepo;
    private readonly IUnitOfWork _uow;

    public CreateCustomerHandler(
        IReadRepository<Customer, Guid> readRepo,
        IWriteRepository<Customer> writeRepo,
        IUnitOfWork uow)
    {
        _readRepo = readRepo;
        _writeRepo = writeRepo;
        _uow = uow;
    }

    public async Task<Result<Guid>> HandleAsync(CreateCustomerCommand command, CancellationToken ct = default)
    {
        try
        {
            var email = Email.Create(command.Email);

            // Prevent DB-level unique constraint exceptions and return a friendly error.
            var existing = await _readRepo.FirstOrDefaultAsync(new CustomerByEmailSpec(email.Value), ct);
            if (existing is not null)
                return Result<Guid>.Fail("customer.email_already_exists", "A customer with this email already exists.");

            var customer = new Customer(Guid.NewGuid(), command.Name, email);

            await _writeRepo.AddAsync(customer, ct);
            await _uow.SaveChangesAsync(ct);

            return Result<Guid>.Ok(customer.Id);
        }
        catch (DbUpdateException ex)
        {
            var details = ex.InnerException?.Message ?? ex.Message;
            return Result<Guid>.Fail("customer.create_failed", details);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Fail("customer.create_failed", ex.Message);
        }
    }
}
