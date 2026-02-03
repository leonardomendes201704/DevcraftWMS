using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevcraftWMS.Infrastructure.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddReceiptInboundOrderSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InboundOrderId",
                table: "Receipts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAtUtc",
                table: "Receipts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_InboundOrderId",
                table: "Receipts",
                column: "InboundOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_StartedAtUtc",
                table: "Receipts",
                column: "StartedAtUtc");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_InboundOrders_InboundOrderId",
                table: "Receipts",
                column: "InboundOrderId",
                principalTable: "InboundOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_InboundOrders_InboundOrderId",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_InboundOrderId",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_StartedAtUtc",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "InboundOrderId",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "StartedAtUtc",
                table: "Receipts");
        }
    }
}
