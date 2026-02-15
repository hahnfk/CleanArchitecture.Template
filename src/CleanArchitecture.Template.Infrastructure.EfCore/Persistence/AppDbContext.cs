using System.Linq.Expressions;
using CleanArchitecture.Template.Domain.Abstractions;
using CleanArchitecture.Template.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Template.Infrastructure.EfCore.Persistence;

public sealed class AppDbContext : DbContext
{
    public DbSet<Customer> Customers => Set<Customer>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        ApplySoftDeleteQueryFilter(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private static void ApplySoftDeleteQueryFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                continue;

            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var cast = Expression.Convert(parameter, typeof(ISoftDeletable));
            var prop = Expression.Property(cast, nameof(ISoftDeletable.IsDeleted));
            var notDeleted = Expression.Not(prop);
            var lambda = Expression.Lambda(notDeleted, parameter);

            entityType.SetQueryFilter(lambda);
        }
    }
}
