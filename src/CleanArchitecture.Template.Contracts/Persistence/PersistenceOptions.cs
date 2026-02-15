namespace CleanArchitecture.Template.Contracts.Persistence;

public sealed class PersistenceOptions
{
    public const string SectionName = "Persistence";

    public PersistenceProvider Provider { get; set; } = PersistenceProvider.Ef;

    /// <summary>
    /// Used by EF Core and ADO providers (SQLite).
    /// For InMemory this can be null/empty.
    /// </summary>
    public string? ConnectionString { get; set; } = "Data Source=app.db";
}
