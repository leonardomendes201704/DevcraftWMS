using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevcraftWMS.Infrastructure.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddInventoryMovements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryMovements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FromLocationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ToLocationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LotId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Quantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Reference = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PerformedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
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
                    table.PrimaryKey("PK_InventoryMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryMovements_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryMovements_Locations_FromLocationId",
                        column: x => x.FromLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryMovements_Locations_ToLocationId",
                        column: x => x.ToLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryMovements_Lots_LotId",
                        column: x => x.LotId,
                        principalTable: "Lots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryMovements_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_CustomerId",
                table: "InventoryMovements",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_FromLocationId",
                table: "InventoryMovements",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_LotId",
                table: "InventoryMovements",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_PerformedAtUtc",
                table: "InventoryMovements",
                column: "PerformedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_ProductId",
                table: "InventoryMovements",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_ToLocationId",
                table: "InventoryMovements",
                column: "ToLocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryMovements");
        }
    }
}
