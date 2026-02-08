using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using DevcraftWMS.Application.Common.Behaviors;
using DevcraftWMS.Application.Features.Customers;
using DevcraftWMS.Application.Features.CostCenters;
using DevcraftWMS.Application.Features.Notifications;
using DevcraftWMS.Application.Abstractions.Notifications;
using DevcraftWMS.Application.Abstractions.Auth;
using DevcraftWMS.Application.Features.Auth;
using DevcraftWMS.Application.Features.Emails;
using DevcraftWMS.Application.Features.Warehouses;
using DevcraftWMS.Application.Features.Sectors;
using DevcraftWMS.Application.Features.Sections;
using DevcraftWMS.Application.Features.Structures;
using DevcraftWMS.Application.Features.Locations;
using DevcraftWMS.Application.Features.Aisles;
using DevcraftWMS.Application.Features.Products;
using DevcraftWMS.Application.Features.Lots;
using DevcraftWMS.Application.Features.Uoms;
using DevcraftWMS.Application.Features.ProductUoms;
using DevcraftWMS.Application.Features.InventoryBalances;
using DevcraftWMS.Application.Features.Receipts;
using DevcraftWMS.Application.Features.InventoryMovements;
using DevcraftWMS.Application.Features.Zones;
using DevcraftWMS.Application.Features.Asns;
using DevcraftWMS.Application.Features.InboundOrders;
using DevcraftWMS.Application.Features.OutboundOrders;
using DevcraftWMS.Application.Features.OutboundChecks;
using DevcraftWMS.Application.Features.OutboundPacking;
using DevcraftWMS.Application.Features.OutboundShipping;
using DevcraftWMS.Application.Features.OutboundOrderNotifications;
using DevcraftWMS.Application.Features.InboundOrderNotifications;
using DevcraftWMS.Application.Features.GateCheckins;
using DevcraftWMS.Application.Features.UnitLoads;
using DevcraftWMS.Application.Features.ReceiptCounts;
using DevcraftWMS.Application.Features.ReceiptDivergences;
using DevcraftWMS.Application.Features.QualityInspections;
using DevcraftWMS.Application.Features.Dashboard;
using DevcraftWMS.Application.Features.Rbac;
using DevcraftWMS.Application.Features.Users;
using DevcraftWMS.Application.Features.PickingReplenishments;
using DevcraftWMS.Application.Features.Returns;
using DevcraftWMS.Application.Features.InventoryCounts;
using DevcraftWMS.Application.Features.DockSchedules;
using DevcraftWMS.Application.Features.InventoryVisibility;

namespace DevcraftWMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IOutboxEnqueuer, OutboxEnqueuer>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IWarehouseService, WarehouseService>();
        services.AddScoped<ISectorService, SectorService>();
        services.AddScoped<ISectionService, SectionService>();
        services.AddScoped<IStructureService, StructureService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IAisleService, AisleService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ILotService, LotService>();
        services.AddScoped<IUomService, UomService>();
        services.AddScoped<IProductUomService, ProductUomService>();
        services.AddScoped<ICostCenterService, CostCenterService>();
        services.AddScoped<IInventoryBalanceService, InventoryBalanceService>();
        services.AddScoped<IInventoryMovementService, InventoryMovementService>();
        services.AddScoped<IReceiptService, ReceiptService>();
        services.AddScoped<IQualityInspectionService, QualityInspectionService>();
        services.AddScoped<IZoneService, ZoneService>();
        services.AddScoped<IAsnService, AsnService>();
        services.AddScoped<IInboundOrderService, InboundOrderService>();
        services.AddScoped<IOutboundOrderService, OutboundOrderService>();
        services.AddScoped<IOutboundOrderReportService, OutboundOrderReportService>();
        services.AddScoped<IOutboundCheckService, OutboundCheckService>();
        services.AddScoped<IOutboundPackingService, OutboundPackingService>();
        services.AddScoped<IOutboundShippingService, OutboundShippingService>();
        services.AddScoped<IOutboundOrderNotificationService, OutboundOrderNotificationService>();
        services.AddScoped<IInboundOrderReportService, InboundOrderReportService>();
        services.AddScoped<IInboundOrderNotificationService, InboundOrderNotificationService>();
        services.AddScoped<IGateCheckinService, GateCheckinService>();
        services.AddScoped<IUnitLoadService, UnitLoadService>();
        services.AddScoped<IReceiptCountService, ReceiptCountService>();
        services.AddScoped<IReceiptDivergenceService, ReceiptDivergenceService>();
        services.AddScoped<IReturnService, ReturnService>();
        services.AddScoped<IInventoryCountService, InventoryCountService>();
        services.AddScoped<IDockScheduleService, DockScheduleService>();
        services.AddScoped<IInventoryVisibilityService, InventoryVisibilityService>();
        services.AddScoped<PickingReplenishmentService>();
        services.AddScoped<RoleService>();
        services.AddScoped<PermissionService>();
        services.AddScoped<UserManagementService>();

        return services;
    }
}


