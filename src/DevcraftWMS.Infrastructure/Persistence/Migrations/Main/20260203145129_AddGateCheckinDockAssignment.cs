using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevcraftWMS.Infrastructure.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddGateCheckinDockAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DockAssignedAtUtc",
                table: "GateCheckins",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DockCode",
                table: "GateCheckins",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GateCheckins_DockCode",
                table: "GateCheckins",
                column: "DockCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GateCheckins_DockCode",
                table: "GateCheckins");

            migrationBuilder.DropColumn(
                name: "DockAssignedAtUtc",
                table: "GateCheckins");

            migrationBuilder.DropColumn(
                name: "DockCode",
                table: "GateCheckins");
        }
    }
}
