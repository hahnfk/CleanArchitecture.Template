using CleanArchitecture.Template.Application.Abstractions.Persistence;

namespace CleanArchitecture.Template.Infrastructure.EfCore.Persistence;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public EfUnitOfWork(AppDbContext db) => _db = db;

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
