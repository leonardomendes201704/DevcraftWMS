using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Infrastructure.Persistence.Logging.Entities;

namespace DevcraftWMS.Infrastructure.Persistence.Logging;

public sealed class LogsDbContext : DbContext
{
    public LogsDbContext(DbContextOptions<LogsDbContext> options)
        : base(options)
    {
    }

    public DbSet<RequestLog> RequestLogs => Set<RequestLog>();
    public DbSet<ErrorLog> ErrorLogs => Set<ErrorLog>();
    public DbSet<TransactionLog> TransactionLogs => Set<TransactionLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(LogsDbContext).Assembly,
            type => type.Namespace is not null && type.Namespace.Contains("Persistence.Logging.Configurations"));
        base.OnModelCreating(modelBuilder);
    }
}


