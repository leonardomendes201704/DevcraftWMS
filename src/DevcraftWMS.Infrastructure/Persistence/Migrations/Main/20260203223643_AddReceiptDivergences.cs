using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevcraftWMS.Infrastructure.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddReceiptDivergences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReceiptDivergences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReceiptId = table.Column<Guid>(type: "TEXT", nullable: false),
                    InboundOrderId = table.Column<Guid>(type: "TEXT", nullable: true),
                    InboundOrderItemId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    RequiresEvidence = table.Column<bool>(type: "INTEGER", nullable: false),
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
                    table.PrimaryKey("PK_ReceiptDivergences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptDivergences_InboundOrderItems_InboundOrderItemId",
                        column: x => x.InboundOrderItemId,
                        principalTable: "InboundOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReceiptDivergences_Receipts_ReceiptId",
                        column: x => x.ReceiptId,
                        principalTable: "Receipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptDivergenceEvidence",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReceiptDivergenceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 260, nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    SizeBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: false),
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
                    table.PrimaryKey("PK_ReceiptDivergenceEvidence", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptDivergenceEvidence_ReceiptDivergences_ReceiptDivergenceId",
                        column: x => x.ReceiptDivergenceId,
                        principalTable: "ReceiptDivergences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptDivergenceEvidence_CreatedAtUtc",
                table: "ReceiptDivergenceEvidence",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptDivergenceEvidence_ReceiptDivergenceId",
                table: "ReceiptDivergenceEvidence",
                column: "ReceiptDivergenceId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptDivergences_CreatedAtUtc",
                table: "ReceiptDivergences",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptDivergences_InboundOrderId",
                table: "ReceiptDivergences",
                column: "InboundOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptDivergences_InboundOrderItemId",
                table: "ReceiptDivergences",
                column: "InboundOrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptDivergences_ReceiptId",
                table: "ReceiptDivergences",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptDivergences_Type",
                table: "ReceiptDivergences",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceiptDivergenceEvidence");

            migrationBuilder.DropTable(
                name: "ReceiptDivergences");
        }
    }
}
