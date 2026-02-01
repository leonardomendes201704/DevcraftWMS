using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PerfectApiTemplate.Infrastructure.Persistence.Logging;

public sealed class LogsDbContextFactory : IDesignTimeDbContextFactory<LogsDbContext>
{
    public LogsDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__LogsDb")
            ?? "Data Source=logs.db";
        var optionsBuilder = new DbContextOptionsBuilder<LogsDbContext>();
        optionsBuilder.UseSqlite(connectionString);

        return new LogsDbContext(optionsBuilder.Options);
    }
}

