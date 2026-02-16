using CleanArchitecture.Template.Application.Abstractions.Persistence;

namespace CleanArchitecture.Template.Infrastructure.InMemory;

public sealed class InMemoryUnitOfWork : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return Task.FromResult(0);
    }
}
