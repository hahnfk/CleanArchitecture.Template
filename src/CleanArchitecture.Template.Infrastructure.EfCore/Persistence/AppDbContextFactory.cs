using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CleanArchitecture.Template.Infrastructure.EfCore.Persistence;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var cs = Environment.GetEnvironmentVariable("APPDB_CONNECTIONSTRING") ?? "Data Source=app.db";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(cs)
            .Options;

        return new AppDbContext(options);
    }
}
