using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevcraftWMS.Infrastructure.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddOutboundShipments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutboundShipments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OutboundOrderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DockCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LoadingStartedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LoadingCompletedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ShippedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboundShipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutboundShipments_OutboundOrders_OutboundOrderId",
                        column: x => x.OutboundOrderId,
                        principalTable: "OutboundOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutboundShipments_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OutboundShipmentItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OutboundShipmentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OutboundPackageId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PackageNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    WeightKg = table.Column<decimal>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboundShipmentItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutboundShipmentItems_OutboundPackages_OutboundPackageId",
                        column: x => x.OutboundPackageId,
                        principalTable: "OutboundPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutboundShipmentItems_OutboundShipments_OutboundShipmentId",
                        column: x => x.OutboundShipmentId,
                        principalTable: "OutboundShipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutboundShipmentItems_OutboundPackageId",
                table: "OutboundShipmentItems",
                column: "OutboundPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundShipmentItems_OutboundShipmentId",
                table: "OutboundShipmentItems",
                column: "OutboundShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundShipments_OutboundOrderId",
                table: "OutboundShipments",
                column: "OutboundOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundShipments_WarehouseId",
                table: "OutboundShipments",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboundShipmentItems");

            migrationBuilder.DropTable(
                name: "OutboundShipments");
        }
    }
}
