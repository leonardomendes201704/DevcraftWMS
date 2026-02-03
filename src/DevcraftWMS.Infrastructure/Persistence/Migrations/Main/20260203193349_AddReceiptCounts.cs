using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevcraftWMS.Infrastructure.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddReceiptCounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReceiptCounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReceiptId = table.Column<Guid>(type: "TEXT", nullable: false),
                    InboundOrderItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExpectedQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    CountedQuantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Variance = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Mode = table.Column<int>(type: "INTEGER", nullable: false),
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
                    table.PrimaryKey("PK_ReceiptCounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptCounts_InboundOrderItems_InboundOrderItemId",
                        column: x => x.InboundOrderItemId,
                        principalTable: "InboundOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReceiptCounts_Receipts_ReceiptId",
                        column: x => x.ReceiptId,
                        principalTable: "Receipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptCounts_InboundOrderItemId",
                table: "ReceiptCounts",
                column: "InboundOrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptCounts_Mode",
                table: "ReceiptCounts",
                column: "Mode");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptCounts_ReceiptId",
                table: "ReceiptCounts",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptCounts_ReceiptId_InboundOrderItemId",
                table: "ReceiptCounts",
                columns: new[] { "ReceiptId", "InboundOrderItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptCounts_Variance",
                table: "ReceiptCounts",
                column: "Variance");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceiptCounts");
        }
    }
}
