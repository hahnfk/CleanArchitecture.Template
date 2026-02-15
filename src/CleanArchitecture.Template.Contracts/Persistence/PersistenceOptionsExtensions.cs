using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.Template.Contracts.Persistence;

public static class PersistenceOptionsExtensions
{
    public static PersistenceOptions GetPersistenceOptions(this IConfiguration configuration)
    {
        var options = new PersistenceOptions();
        ConfigurationBinder.Bind(configuration.GetSection(PersistenceOptions.SectionName), options);
        return options;
    }
}
