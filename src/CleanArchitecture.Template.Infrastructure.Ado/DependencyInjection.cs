using CleanArchitecture.Template.Application.Abstractions.Persistence;
using CleanArchitecture.Template.Application.Abstractions.Queries;
using CleanArchitecture.Template.Domain.Aggregates;
using CleanArchitecture.Template.Infrastructure.Ado.Persistence;
using CleanArchitecture.Template.Infrastructure.Ado.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Template.Infrastructure.Ado;

public static class DependencyInjection
{
    public static IServiceCollection AddAdoInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("AppDb") ?? "Data Source=app.db";

        services.AddScoped(_ => new AdoUnitOfWork(connectionString));
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AdoUnitOfWork>());

        services.AddScoped<IReadRepository<Customer, Guid>, AdoCustomerRepository>();
        services.AddScoped<IWriteRepository<Customer>, AdoCustomerRepository>();

        services.AddScoped<ICustomerQueries>(_ => new AdoCustomerQueries(connectionString));

        return services;
    }
}
