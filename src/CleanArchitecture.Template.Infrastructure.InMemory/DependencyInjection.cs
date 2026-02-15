using CleanArchitecture.Template.Application.Abstractions.Persistence;
using CleanArchitecture.Template.Application.Abstractions.Queries;
using CleanArchitecture.Template.Domain.Aggregates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Template.Infrastructure.InMemory;

public static class DependencyInjection
{
    public static IServiceCollection AddInMemoryInfrastructure(this IServiceCollection services, IConfiguration _)
    {
        services.AddSingleton<InMemoryStore>();

        services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();

        services.AddSingleton<IReadRepository<Customer, Guid>, InMemoryCustomerRepository>();
        services.AddSingleton<IWriteRepository<Customer>, InMemoryCustomerRepository>();

        services.AddSingleton<ICustomerQueries, InMemoryCustomerQueries>();

        return services;
    }
}
