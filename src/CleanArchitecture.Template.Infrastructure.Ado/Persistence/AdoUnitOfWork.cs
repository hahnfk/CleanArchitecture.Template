using CleanArchitecture.Template.Application.Abstractions.Persistence;
using Microsoft.Data.Sqlite;

namespace CleanArchitecture.Template.Infrastructure.Ado.Persistence;

public sealed class AdoUnitOfWork : IUnitOfWork, IDisposable
{
    private readonly SqliteConnection _connection;
    private SqliteTransaction? _tx;

    public AdoUnitOfWork(string connectionString)
    {
        _connection = new SqliteConnection(connectionString);
        _connection.Open();
        _tx = _connection.BeginTransaction();
    }

    public SqliteConnection Connection => _connection;
    public SqliteTransaction Transaction => _tx ?? throw new InvalidOperationException("Transaction not available.");

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        _tx?.Commit();
        _tx?.Dispose();
        _tx = null;
        return Task.FromResult(0);
    }

    public void Dispose()
    {
        try { _tx?.Dispose(); } catch { }
        _connection.Dispose();
    }
}
