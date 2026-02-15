using CleanArchitecture.Template.Application.Abstractions.Persistence;
using CleanArchitecture.Template.Application.Abstractions.Specifications;
using CleanArchitecture.Template.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Template.Infrastructure.EfCore.Persistence;

public sealed class EfRepository<TAggregate, TId> : IRepository<TAggregate, TId>
    where TAggregate : class, IAggregateRoot
{
    private readonly AppDbContext _db;
    private readonly DbSet<TAggregate> _set;

    public EfRepository(AppDbContext db)
    {
        _db = db;
        _set = db.Set<TAggregate>();
    }

    public Task<TAggregate?> GetByIdAsync(TId id, CancellationToken ct = default)
        => _set.FindAsync([id!], ct).AsTask();

    public async Task<IReadOnlyList<TAggregate>> ListAsync(CancellationToken ct = default)
        => await _set.AsNoTracking().ToListAsync(ct);

    public Task AddAsync(TAggregate aggregate, CancellationToken ct = default)
        => _set.AddAsync(aggregate, ct).AsTask();

    public Task RemoveAsync(TAggregate aggregate, CancellationToken ct = default)
{
    if (aggregate is CleanArchitecture.Template.Domain.Abstractions.ISoftDeletable)
    {
        // Convention: soft delete aggregates that support it
        var method = aggregate.GetType().GetMethod("SoftDelete", new[] { typeof(DateTimeOffset) });
        method?.Invoke(aggregate, new object[] { DateTimeOffset.UtcNow });
        _set.Update(aggregate);
        return Task.CompletedTask;
    }

    _set.Remove(aggregate);
    return Task.CompletedTask;
}

    public Task<TAggregate?> FirstOrDefaultAsync(ISpecification<TAggregate> specification, CancellationToken ct = default)
        => SpecificationEvaluator.GetQuery(_set, specification).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<TAggregate>> ListAsync(ISpecification<TAggregate> specification, CancellationToken ct = default)
        => await SpecificationEvaluator.GetQuery(_set, specification).ToListAsync(ct);
}
