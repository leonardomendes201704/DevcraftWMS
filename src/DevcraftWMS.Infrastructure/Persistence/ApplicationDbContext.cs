using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Application.Abstractions.Auth;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService? currentUserService = null)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserProvider> UserProviders => Set<UserProvider>();
    public DbSet<EmailMessage> EmailMessages => Set<EmailMessage>();
    public DbSet<EmailDeliveryAttempt> EmailDeliveryAttempts => Set<EmailDeliveryAttempt>();
    public DbSet<EmailInboxMessage> EmailInboxMessages => Set<EmailInboxMessage>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<WarehouseAddress> WarehouseAddresses => Set<WarehouseAddress>();
    public DbSet<WarehouseContact> WarehouseContacts => Set<WarehouseContact>();
    public DbSet<WarehouseCapacity> WarehouseCapacities => Set<WarehouseCapacity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var userId = _currentUserService?.UserId;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAtUtc = now;
                    entry.Entity.CreatedByUserId = userId;
                    entry.Entity.IsActive = entry.Entity.IsActive;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAtUtc = now;
                    entry.Entity.UpdatedByUserId = userId;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsActive = false;
                    entry.Entity.DeletedAtUtc = now;
                    entry.Entity.DeletedByUserId = userId;
                    entry.Entity.UpdatedAtUtc = now;
                    entry.Entity.UpdatedByUserId = userId;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}


