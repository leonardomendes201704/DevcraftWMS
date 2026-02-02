using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevcraftWMS.Infrastructure.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddCustomerOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Warehouses_Code",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_ProductUoms_ProductId_UomId",
                table: "ProductUoms");

            migrationBuilder.DropIndex(
                name: "IX_Products_Code",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_Ean",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ErpCode",
                table: "Products");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "Warehouses",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "Structures",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "Sectors",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "Sections",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "ProductUoms",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "Products",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "Locations",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "Aisles",
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
                name: "IX_Structures_CustomerId",
                table: "Structures",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_CustomerId",
                table: "Sectors",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_CustomerId",
                table: "Sections",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductUoms_CustomerId",
                table: "ProductUoms",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductUoms_CustomerId_ProductId_UomId",
                table: "ProductUoms",
                columns: new[] { "CustomerId", "ProductId", "UomId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CustomerId",
                table: "Products",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CustomerId_Code",
                table: "Products",
                columns: new[] { "CustomerId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CustomerId_Ean",
                table: "Products",
                columns: new[] { "CustomerId", "Ean" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CustomerId_ErpCode",
                table: "Products",
                columns: new[] { "CustomerId", "ErpCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_CustomerId",
                table: "Locations",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Aisles_CustomerId",
                table: "Aisles",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Warehouses_CustomerId",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_Warehouses_CustomerId_Code",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_Structures_CustomerId",
                table: "Structures");

            migrationBuilder.DropIndex(
                name: "IX_Sectors_CustomerId",
                table: "Sectors");

            migrationBuilder.DropIndex(
                name: "IX_Sections_CustomerId",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_ProductUoms_CustomerId",
                table: "ProductUoms");

            migrationBuilder.DropIndex(
                name: "IX_ProductUoms_CustomerId_ProductId_UomId",
                table: "ProductUoms");

            migrationBuilder.DropIndex(
                name: "IX_Products_CustomerId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CustomerId_Code",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CustomerId_Ean",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CustomerId_ErpCode",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Locations_CustomerId",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Aisles_CustomerId",
                table: "Aisles");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Structures");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Sectors");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "ProductUoms");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Aisles");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_Code",
                table: "Warehouses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductUoms_ProductId_UomId",
                table: "ProductUoms",
                columns: new[] { "ProductId", "UomId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Code",
                table: "Products",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Ean",
                table: "Products",
                column: "Ean",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_ErpCode",
                table: "Products",
                column: "ErpCode",
                unique: true);
        }
    }
}
