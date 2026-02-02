using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevcraftWMS.Infrastructure.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddLocationCapacityAndRestrictions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowExpiryTracking",
                table: "Locations",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowLotTracking",
                table: "Locations",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxVolumeM3",
                table: "Locations",
                type: "TEXT",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxWeightKg",
                table: "Locations",
                type: "TEXT",
                precision: 18,
                scale: 3,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowExpiryTracking",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "AllowLotTracking",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "MaxVolumeM3",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "MaxWeightKg",
                table: "Locations");
        }
    }
}
