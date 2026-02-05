using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevcraftWMS.Infrastructure.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddOutboundOrderReleaseFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PickingMethod",
                table: "OutboundOrders",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ShippingWindowEndUtc",
                table: "OutboundOrders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ShippingWindowStartUtc",
                table: "OutboundOrders",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PickingMethod",
                table: "OutboundOrders");

            migrationBuilder.DropColumn(
                name: "ShippingWindowEndUtc",
                table: "OutboundOrders");

            migrationBuilder.DropColumn(
                name: "ShippingWindowStartUtc",
                table: "OutboundOrders");
        }
    }
}
