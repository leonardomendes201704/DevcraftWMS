using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevcraftWMS.Infrastructure.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddUnitLoads : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UnitLoads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReceiptId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SsccInternal = table.Column<string>(type: "TEXT", maxLength: 18, nullable: false),
                    SsccExternal = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PrintedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_UnitLoads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitLoads_Receipts_ReceiptId",
                        column: x => x.ReceiptId,
                        principalTable: "Receipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnitLoads_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnitLoads_CustomerId",
                table: "UnitLoads",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitLoads_PrintedAtUtc",
                table: "UnitLoads",
                column: "PrintedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_UnitLoads_ReceiptId",
                table: "UnitLoads",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitLoads_SsccInternal",
                table: "UnitLoads",
                column: "SsccInternal",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitLoads_Status",
                table: "UnitLoads",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_UnitLoads_WarehouseId",
                table: "UnitLoads",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnitLoads");
        }
    }
}
