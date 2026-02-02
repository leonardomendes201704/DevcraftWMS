using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevcraftWMS.Infrastructure.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class AdjustWarehouseOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Warehouses_CustomerId",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_Warehouses_CustomerId_Code",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_Sectors_WarehouseId_Code",
                table: "Sectors");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Warehouses");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_Code",
                table: "Warehouses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_CustomerId_WarehouseId_Code",
                table: "Sectors",
                columns: new[] { "CustomerId", "WarehouseId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_WarehouseId",
                table: "Sectors",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Warehouses_Code",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_Sectors_CustomerId_WarehouseId_Code",
                table: "Sectors");

            migrationBuilder.DropIndex(
                name: "IX_Sectors_WarehouseId",
                table: "Sectors");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "Warehouses",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_CustomerId",
                table: "Warehouses",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_CustomerId_Code",
                table: "Warehouses",
                columns: new[] { "CustomerId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_WarehouseId_Code",
                table: "Sectors",
                columns: new[] { "WarehouseId", "Code" },
                unique: true);
        }
    }
}
