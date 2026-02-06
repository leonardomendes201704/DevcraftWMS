using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevcraftWMS.Infrastructure.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddOutboundOrderReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCrossDock",
                table: "OutboundOrders",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "OutboundOrderReservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OutboundOrderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OutboundOrderItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    InventoryBalanceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LotId = table.Column<Guid>(type: "TEXT", nullable: true),
                    QuantityReserved = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
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
                    table.PrimaryKey("PK_OutboundOrderReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutboundOrderReservations_InventoryBalances_InventoryBalanceId",
                        column: x => x.InventoryBalanceId,
                        principalTable: "InventoryBalances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutboundOrderReservations_OutboundOrderItems_OutboundOrderItemId",
                        column: x => x.OutboundOrderItemId,
                        principalTable: "OutboundOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutboundOrderReservations_OutboundOrders_OutboundOrderId",
                        column: x => x.OutboundOrderId,
                        principalTable: "OutboundOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutboundOrderReservations_InventoryBalanceId",
                table: "OutboundOrderReservations",
                column: "InventoryBalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundOrderReservations_OutboundOrderId_OutboundOrderItemId",
                table: "OutboundOrderReservations",
                columns: new[] { "OutboundOrderId", "OutboundOrderItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_OutboundOrderReservations_OutboundOrderItemId",
                table: "OutboundOrderReservations",
                column: "OutboundOrderItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboundOrderReservations");

            migrationBuilder.DropColumn(
                name: "IsCrossDock",
                table: "OutboundOrders");
        }
    }
}
