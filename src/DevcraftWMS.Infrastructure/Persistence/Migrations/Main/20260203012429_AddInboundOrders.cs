using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevcraftWMS.Infrastructure.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddInboundOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InboundOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AsnId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrderNumber = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    SupplierName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    DocumentNumber = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    ExpectedArrivalDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    InspectionLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    SuggestedDock = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    CancelReason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CanceledAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CanceledByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
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
                    table.PrimaryKey("PK_InboundOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InboundOrders_Asns_AsnId",
                        column: x => x.AsnId,
                        principalTable: "Asns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InboundOrders_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InboundOrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    InboundOrderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UomId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 3, nullable: false),
                    LotCode = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    ExpirationDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
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
                    table.PrimaryKey("PK_InboundOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InboundOrderItems_InboundOrders_InboundOrderId",
                        column: x => x.InboundOrderId,
                        principalTable: "InboundOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InboundOrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InboundOrderItems_Uoms_UomId",
                        column: x => x.UomId,
                        principalTable: "Uoms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InboundOrderItems_InboundOrderId",
                table: "InboundOrderItems",
                column: "InboundOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundOrderItems_ProductId",
                table: "InboundOrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundOrderItems_UomId",
                table: "InboundOrderItems",
                column: "UomId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundOrders_AsnId",
                table: "InboundOrders",
                column: "AsnId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InboundOrders_CustomerId_OrderNumber",
                table: "InboundOrders",
                columns: new[] { "CustomerId", "OrderNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InboundOrders_Priority",
                table: "InboundOrders",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_InboundOrders_Status",
                table: "InboundOrders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InboundOrders_WarehouseId",
                table: "InboundOrders",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InboundOrderItems");

            migrationBuilder.DropTable(
                name: "InboundOrders");
        }
    }
}
