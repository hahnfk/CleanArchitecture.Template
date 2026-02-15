using CleanArchitecture.Template.Application.Abstractions.Persistence;
using CleanArchitecture.Template.Application.Abstractions.Queries;
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
        var connectionString = config.GetConnectionString("AppDb") ?? "Data Source=app.db";

        services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseSqlite(connectionString);
        });

        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        // Open-generic repository registration (primary)
        services.AddScoped(typeof(IRepository<,>), typeof(EfRepository<,>));

        // NOTE:
        // IWriteRepository<TAggregate> cannot be mapped to IRepository<TAggregate, TId> as open generic because TId is unknown.
        // For the template (Customer uses Guid) we register the bridge explicitly.
        services.AddScoped<IRepository<Customer, Guid>, EfRepository<Customer, Guid>>();
        services.AddScoped<IReadRepository<Customer, Guid>>(sp => sp.GetRequiredService<IRepository<Customer, Guid>>());
        services.AddScoped<IWriteRepository<Customer>>(sp => sp.GetRequiredService<IRepository<Customer, Guid>>());

        services.AddScoped<ICustomerQueries, CustomerQueries>();

        return services;
    }
}
