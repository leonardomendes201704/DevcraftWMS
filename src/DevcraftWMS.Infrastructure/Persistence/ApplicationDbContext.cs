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
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserRoleAssignment> UserRoles => Set<UserRoleAssignment>();
    public DbSet<EmailMessage> EmailMessages => Set<EmailMessage>();
    public DbSet<EmailDeliveryAttempt> EmailDeliveryAttempts => Set<EmailDeliveryAttempt>();
    public DbSet<EmailInboxMessage> EmailInboxMessages => Set<EmailInboxMessage>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<WarehouseAddress> WarehouseAddresses => Set<WarehouseAddress>();
    public DbSet<WarehouseContact> WarehouseContacts => Set<WarehouseContact>();
    public DbSet<WarehouseCapacity> WarehouseCapacities => Set<WarehouseCapacity>();
    public DbSet<CostCenter> CostCenters => Set<CostCenter>();
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
    public DbSet<InboundOrderStatusEvent> InboundOrderStatusEvents => Set<InboundOrderStatusEvent>();
    public DbSet<InboundOrderNotification> InboundOrderNotifications => Set<InboundOrderNotification>();
    public DbSet<OutboundOrder> OutboundOrders => Set<OutboundOrder>();
    public DbSet<OutboundOrderItem> OutboundOrderItems => Set<OutboundOrderItem>();
    public DbSet<OutboundOrderReservation> OutboundOrderReservations => Set<OutboundOrderReservation>();
    public DbSet<PickingTask> PickingTasks => Set<PickingTask>();
    public DbSet<PickingTaskItem> PickingTaskItems => Set<PickingTaskItem>();
    public DbSet<PickingReplenishmentTask> PickingReplenishmentTasks => Set<PickingReplenishmentTask>();
    public DbSet<OutboundCheck> OutboundChecks => Set<OutboundCheck>();
    public DbSet<OutboundCheckItem> OutboundCheckItems => Set<OutboundCheckItem>();
    public DbSet<OutboundCheckEvidence> OutboundCheckEvidence => Set<OutboundCheckEvidence>();
    public DbSet<OutboundPackage> OutboundPackages => Set<OutboundPackage>();
    public DbSet<OutboundPackageItem> OutboundPackageItems => Set<OutboundPackageItem>();
    public DbSet<OutboundShipment> OutboundShipments => Set<OutboundShipment>();
    public DbSet<OutboundShipmentItem> OutboundShipmentItems => Set<OutboundShipmentItem>();
    public DbSet<OutboundOrderNotification> OutboundOrderNotifications => Set<OutboundOrderNotification>();
    public DbSet<ReturnOrder> ReturnOrders => Set<ReturnOrder>();
    public DbSet<ReturnItem> ReturnItems => Set<ReturnItem>();
    public DbSet<InventoryCount> InventoryCounts => Set<InventoryCount>();
    public DbSet<InventoryCountItem> InventoryCountItems => Set<InventoryCountItem>();
    public DbSet<DockSchedule> DockSchedules => Set<DockSchedule>();
    public DbSet<GateCheckin> GateCheckins => Set<GateCheckin>();
    public DbSet<Receipt> Receipts => Set<Receipt>();
    public DbSet<ReceiptItem> ReceiptItems => Set<ReceiptItem>();
    public DbSet<ReceiptCount> ReceiptCounts => Set<ReceiptCount>();
    public DbSet<ReceiptDivergence> ReceiptDivergences => Set<ReceiptDivergence>();
    public DbSet<ReceiptDivergenceEvidence> ReceiptDivergenceEvidence => Set<ReceiptDivergenceEvidence>();
    public DbSet<QualityInspection> QualityInspections => Set<QualityInspection>();
    public DbSet<QualityInspectionEvidence> QualityInspectionEvidence => Set<QualityInspectionEvidence>();
    public DbSet<UnitLoad> UnitLoads => Set<UnitLoad>();
    public DbSet<UnitLoadRelabelEvent> UnitLoadRelabelEvents => Set<UnitLoadRelabelEvent>();
    public DbSet<PutawayTask> PutawayTasks => Set<PutawayTask>();
    public DbSet<PutawayTaskAssignmentEvent> PutawayTaskAssignmentEvents => Set<PutawayTaskAssignmentEvent>();
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
            if (entry.State == EntityState.Deleted &&
                entry.Entity is RolePermission or UserRoleAssignment)
            {
                continue;
            }

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


