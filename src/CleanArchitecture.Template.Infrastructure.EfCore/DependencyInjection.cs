using CleanArchitecture.Template.Application.Abstractions.Persistence;
using CleanArchitecture.Template.Application.Abstractions.Queries;
using CleanArchitecture.Template.Contracts.Persistence;
using CleanArchitecture.Template.Domain.Aggregates;
using CleanArchitecture.Template.Infrastructure.EfCore.Persistence;
using CleanArchitecture.Template.Infrastructure.EfCore.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Template.Infrastructure.EfCore;

public static class DependencyInjection
{
    public static IServiceCollection AddEfCoreInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // Prefer PersistenceOptions.ConnectionString (enum-based provider config),
        // fall back to ConnectionStrings:AppDb for compatibility.
        var persistence = config.GetPersistenceOptions();

        var connectionString =
            persistence.ConnectionString
            ?? config.GetConnectionString("AppDb")
            ?? "Data Source=app.db";

        services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(connectionString));

        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        // Open-generic repository registration (primary)
        services.AddScoped(typeof(IRepository<,>), typeof(EfRepository<,>));

        // Template bridge registrations for Customer aggregate (Customer uses Guid)
        services.AddScoped<IRepository<Customer, Guid>, EfRepository<Customer, Guid>>();
        services.AddScoped<IReadRepository<Customer, Guid>>(sp => sp.GetRequiredService<IRepository<Customer, Guid>>());
        services.AddScoped<IWriteRepository<Customer>>(sp => sp.GetRequiredService<IRepository<Customer, Guid>>());

        services.AddScoped<ICustomerQueries, CustomerQueries>();

        return services;
    }
}
