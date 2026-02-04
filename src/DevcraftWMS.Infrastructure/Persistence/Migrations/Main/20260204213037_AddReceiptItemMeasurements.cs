using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevcraftWMS.Infrastructure.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddReceiptItemMeasurements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ActualVolumeCm3",
                table: "ReceiptItems",
                type: "TEXT",
                precision: 18,
                scale: 3,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualWeightKg",
                table: "ReceiptItems",
                type: "TEXT",
                precision: 18,
                scale: 3,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExpectedVolumeCm3",
                table: "ReceiptItems",
                type: "TEXT",
                precision: 18,
                scale: 3,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExpectedWeightKg",
                table: "ReceiptItems",
                type: "TEXT",
                precision: 18,
                scale: 3,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMeasurementOutOfRange",
                table: "ReceiptItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "VolumeDeviationPercent",
                table: "ReceiptItems",
                type: "TEXT",
                precision: 9,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WeightDeviationPercent",
                table: "ReceiptItems",
                type: "TEXT",
                precision: 9,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualVolumeCm3",
                table: "ReceiptItems");

            migrationBuilder.DropColumn(
                name: "ActualWeightKg",
                table: "ReceiptItems");

            migrationBuilder.DropColumn(
                name: "ExpectedVolumeCm3",
                table: "ReceiptItems");

            migrationBuilder.DropColumn(
                name: "ExpectedWeightKg",
                table: "ReceiptItems");

            migrationBuilder.DropColumn(
                name: "IsMeasurementOutOfRange",
                table: "ReceiptItems");

            migrationBuilder.DropColumn(
                name: "VolumeDeviationPercent",
                table: "ReceiptItems");

            migrationBuilder.DropColumn(
                name: "WeightDeviationPercent",
                table: "ReceiptItems");
        }
    }
}
