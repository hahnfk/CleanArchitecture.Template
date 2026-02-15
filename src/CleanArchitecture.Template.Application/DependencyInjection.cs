using CleanArchitecture.Template.Application.Customers.UseCases.CreateCustomer;
using CleanArchitecture.Template.Application.Customers.UseCases.DeleteCustomer;
using CleanArchitecture.Template.Application.Customers.UseCases.DeleteCustomers;
using CleanArchitecture.Template.Application.Customers.UseCases.GetCustomer;
using CleanArchitecture.Template.Application.Customers.UseCases.ReadCustomers;
using CleanArchitecture.Template.Application.Customers.UseCases.UpdateCustomer;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Template.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Use cases (vertical slices)
        services.AddScoped<CreateCustomerHandler>();
        services.AddScoped<DeleteCustomerHandler>();
        services.AddScoped<DeleteCustomersHandler>();
        services.AddScoped<GetCustomerHandler>();
        services.AddScoped<ReadCustomersByIdsHandler>();
        services.AddScoped<UpdateCustomerHandler>();

        return services;
    }
}
