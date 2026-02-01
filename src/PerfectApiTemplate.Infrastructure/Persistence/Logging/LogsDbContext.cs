using Microsoft.EntityFrameworkCore;
using PerfectApiTemplate.Infrastructure.Persistence.Logging.Entities;

namespace PerfectApiTemplate.Infrastructure.Persistence.Logging;

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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LogsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

