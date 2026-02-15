using CleanArchitecture.Template.Application.Abstractions.Persistence;
using CleanArchitecture.Template.Application.Abstractions.Queries;
using CleanArchitecture.Template.Domain.Aggregates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Template.Infrastructure.EfCore.Persistence;
using CleanArchitecture.Template.Infrastructure.EfCore.Queries;

namespace CleanArchitecture.Template.Infrastructure.Ado;

public static class DependencyInjection
{
    public static IServiceCollection AddAdoInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("AppDb") ?? "Data Source=app.db";

        services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseSqlite(connectionString);
        });

        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        services.AddScoped(typeof(IRepository<,>), typeof(EfRepository<,>));
        services.AddScoped(typeof(IReadRepository<,>), typeof(EfRepository<,>));
        // IWriteRepository<> has fewer type params than EfRepository<,>, so register closed types:
        services.AddScoped<IWriteRepository<Customer>>(sp => sp.GetRequiredService<IRepository<Customer, Guid>>());

        services.AddScoped<ICustomerQueries, CustomerQueries>();

        return services;
    }
}
