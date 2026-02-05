using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevcraftWMS.Infrastructure.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddOutboundPackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutboundPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OutboundOrderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PackageNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    WeightKg = table.Column<decimal>(type: "TEXT", nullable: true),
                    LengthCm = table.Column<decimal>(type: "TEXT", nullable: true),
                    WidthCm = table.Column<decimal>(type: "TEXT", nullable: true),
                    HeightCm = table.Column<decimal>(type: "TEXT", nullable: true),
                    PackedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PackedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
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
                    table.PrimaryKey("PK_OutboundPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutboundPackages_OutboundOrders_OutboundOrderId",
                        column: x => x.OutboundOrderId,
                        principalTable: "OutboundOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutboundPackages_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OutboundPackageItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OutboundPackageId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OutboundOrderItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UomId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantity = table.Column<decimal>(type: "TEXT", nullable: false),
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
                    table.PrimaryKey("PK_OutboundPackageItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutboundPackageItems_OutboundOrderItems_OutboundOrderItemId",
                        column: x => x.OutboundOrderItemId,
                        principalTable: "OutboundOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutboundPackageItems_OutboundPackages_OutboundPackageId",
                        column: x => x.OutboundPackageId,
                        principalTable: "OutboundPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutboundPackageItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutboundPackageItems_Uoms_UomId",
                        column: x => x.UomId,
                        principalTable: "Uoms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutboundPackageItems_OutboundOrderItemId",
                table: "OutboundPackageItems",
                column: "OutboundOrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundPackageItems_OutboundPackageId",
                table: "OutboundPackageItems",
                column: "OutboundPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundPackageItems_ProductId",
                table: "OutboundPackageItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundPackageItems_UomId",
                table: "OutboundPackageItems",
                column: "UomId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundPackages_OutboundOrderId",
                table: "OutboundPackages",
                column: "OutboundOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundPackages_OutboundOrderId_PackageNumber",
                table: "OutboundPackages",
                columns: new[] { "OutboundOrderId", "PackageNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboundPackages_WarehouseId",
                table: "OutboundPackages",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboundPackageItems");

            migrationBuilder.DropTable(
                name: "OutboundPackages");
        }
    }
}
