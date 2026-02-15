using Microsoft.OpenApi;

namespace CleanArchitecture.Template.Presentation.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CleanArchitecture.Template API",
                Version = "v1"
            });
        });

        return services;
    }
}
