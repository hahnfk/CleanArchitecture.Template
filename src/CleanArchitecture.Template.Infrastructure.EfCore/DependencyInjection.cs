using CleanArchitecture.Template.Application.Abstractions.Persistence;
using CleanArchitecture.Template.Application.Abstractions.Queries;
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

        services.AddScoped(typeof(IRepository<,>), typeof(EfRepository<,>));
        services.AddScoped(typeof(IReadRepository<,>), sp => sp.GetRequiredService(typeof(IRepository<,>)));
        services.AddScoped(typeof(IWriteRepository<>), sp => sp.GetRequiredService(typeof(IRepository<,>)));

        services.AddScoped<ICustomerQueries, CustomerQueries>();

        return services;
    }
}
