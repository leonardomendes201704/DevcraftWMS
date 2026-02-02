using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevcraftWMS.Infrastructure.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddPhysicalVisibilityMappings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Structures_CustomerId",
                table: "Structures");

            migrationBuilder.DropIndex(
                name: "IX_Sectors_CustomerId",
                table: "Sectors");

            migrationBuilder.DropIndex(
                name: "IX_Sectors_CustomerId_WarehouseId_Code",
                table: "Sectors");

            migrationBuilder.DropIndex(
                name: "IX_Sectors_WarehouseId",
                table: "Sectors");

            migrationBuilder.DropIndex(
                name: "IX_Sections_CustomerId",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_Locations_CustomerId",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Aisles_CustomerId",
                table: "Aisles");

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
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Aisles");

            migrationBuilder.CreateTable(
                name: "AisleCustomers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AisleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
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
                    table.PrimaryKey("PK_AisleCustomers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AisleCustomers_Aisles_AisleId",
                        column: x => x.AisleId,
                        principalTable: "Aisles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AisleCustomers_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LocationCustomers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LocationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
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
                    table.PrimaryKey("PK_LocationCustomers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationCustomers_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LocationCustomers_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SectionCustomers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SectionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
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
                    table.PrimaryKey("PK_SectionCustomers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SectionCustomers_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SectionCustomers_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SectorCustomers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SectorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
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
                    table.PrimaryKey("PK_SectorCustomers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SectorCustomers_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SectorCustomers_Sectors_SectorId",
                        column: x => x.SectorId,
                        principalTable: "Sectors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StructureCustomers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    StructureId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
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
                    table.PrimaryKey("PK_StructureCustomers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StructureCustomers_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StructureCustomers_Structures_StructureId",
                        column: x => x.StructureId,
                        principalTable: "Structures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_WarehouseId_Code",
                table: "Sectors",
                columns: new[] { "WarehouseId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AisleCustomers_AisleId_CustomerId",
                table: "AisleCustomers",
                columns: new[] { "AisleId", "CustomerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AisleCustomers_CustomerId",
                table: "AisleCustomers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationCustomers_CustomerId",
                table: "LocationCustomers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationCustomers_LocationId_CustomerId",
                table: "LocationCustomers",
                columns: new[] { "LocationId", "CustomerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SectionCustomers_CustomerId",
                table: "SectionCustomers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionCustomers_SectionId_CustomerId",
                table: "SectionCustomers",
                columns: new[] { "SectionId", "CustomerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SectorCustomers_CustomerId",
                table: "SectorCustomers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SectorCustomers_SectorId_CustomerId",
                table: "SectorCustomers",
                columns: new[] { "SectorId", "CustomerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StructureCustomers_CustomerId",
                table: "StructureCustomers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_StructureCustomers_StructureId_CustomerId",
                table: "StructureCustomers",
                columns: new[] { "StructureId", "CustomerId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AisleCustomers");

            migrationBuilder.DropTable(
                name: "LocationCustomers");

            migrationBuilder.DropTable(
                name: "SectionCustomers");

            migrationBuilder.DropTable(
                name: "SectorCustomers");

            migrationBuilder.DropTable(
                name: "StructureCustomers");

            migrationBuilder.DropIndex(
                name: "IX_Sectors_WarehouseId_Code",
                table: "Sectors");

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
                name: "IX_Structures_CustomerId",
                table: "Structures",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_CustomerId",
                table: "Sectors",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_CustomerId_WarehouseId_Code",
                table: "Sectors",
                columns: new[] { "CustomerId", "WarehouseId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_WarehouseId",
                table: "Sectors",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_CustomerId",
                table: "Sections",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_CustomerId",
                table: "Locations",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Aisles_CustomerId",
                table: "Aisles",
                column: "CustomerId");
        }
    }
}
