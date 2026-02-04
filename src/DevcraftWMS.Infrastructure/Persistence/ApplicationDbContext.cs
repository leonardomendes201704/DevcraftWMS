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
    public DbSet<Sector> Sectors => Set<Sector>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<Structure> Structures => Set<Structure>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Aisle> Aisles => Set<Aisle>();
    public DbSet<Zone> Zones => Set<Zone>();
    public DbSet<SectorCustomer> SectorCustomers => Set<SectorCustomer>();
    public DbSet<SectionCustomer> SectionCustomers => Set<SectionCustomer>();
    public DbSet<StructureCustomer> StructureCustomers => Set<StructureCustomer>();
    public DbSet<LocationCustomer> LocationCustomers => Set<LocationCustomer>();
    public DbSet<AisleCustomer> AisleCustomers => Set<AisleCustomer>();
    public DbSet<ZoneCustomer> ZoneCustomers => Set<ZoneCustomer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Lot> Lots => Set<Lot>();
    public DbSet<InventoryBalance> InventoryBalances => Set<InventoryBalance>();
    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();
    public DbSet<Asn> Asns => Set<Asn>();
    public DbSet<AsnItem> AsnItems => Set<AsnItem>();
    public DbSet<AsnAttachment> AsnAttachments => Set<AsnAttachment>();
    public DbSet<AsnStatusEvent> AsnStatusEvents => Set<AsnStatusEvent>();
    public DbSet<InboundOrder> InboundOrders => Set<InboundOrder>();
    public DbSet<InboundOrderItem> InboundOrderItems => Set<InboundOrderItem>();
    public DbSet<GateCheckin> GateCheckins => Set<GateCheckin>();
    public DbSet<Receipt> Receipts => Set<Receipt>();
    public DbSet<ReceiptItem> ReceiptItems => Set<ReceiptItem>();
    public DbSet<ReceiptCount> ReceiptCounts => Set<ReceiptCount>();
    public DbSet<ReceiptDivergence> ReceiptDivergences => Set<ReceiptDivergence>();
    public DbSet<ReceiptDivergenceEvidence> ReceiptDivergenceEvidence => Set<ReceiptDivergenceEvidence>();
    public DbSet<QualityInspection> QualityInspections => Set<QualityInspection>();
    public DbSet<QualityInspectionEvidence> QualityInspectionEvidence => Set<QualityInspectionEvidence>();
    public DbSet<UnitLoad> UnitLoads => Set<UnitLoad>();
    public DbSet<Uom> Uoms => Set<Uom>();
    public DbSet<ProductUom> ProductUoms => Set<ProductUom>();

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


