using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevcraftWMS.Infrastructure.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddOutboundCheckQueueFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CheckedAtUtc",
                table: "OutboundChecks",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "OutboundChecks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAtUtc",
                table: "OutboundChecks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StartedByUserId",
                table: "OutboundChecks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "OutboundChecks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OutboundChecks_Priority",
                table: "OutboundChecks",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundChecks_Status",
                table: "OutboundChecks",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutboundChecks_Priority",
                table: "OutboundChecks");

            migrationBuilder.DropIndex(
                name: "IX_OutboundChecks_Status",
                table: "OutboundChecks");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "OutboundChecks");

            migrationBuilder.DropColumn(
                name: "StartedAtUtc",
                table: "OutboundChecks");

            migrationBuilder.DropColumn(
                name: "StartedByUserId",
                table: "OutboundChecks");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OutboundChecks");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CheckedAtUtc",
                table: "OutboundChecks",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
