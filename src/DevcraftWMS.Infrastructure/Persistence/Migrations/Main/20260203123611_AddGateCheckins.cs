using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevcraftWMS.Infrastructure.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddGateCheckins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GateCheckins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    InboundOrderId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DocumentNumber = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    VehiclePlate = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DriverName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    CarrierName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    ArrivalAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
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
                    table.PrimaryKey("PK_GateCheckins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GateCheckins_InboundOrders_InboundOrderId",
                        column: x => x.InboundOrderId,
                        principalTable: "InboundOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GateCheckins_ArrivalAtUtc",
                table: "GateCheckins",
                column: "ArrivalAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_GateCheckins_CustomerId",
                table: "GateCheckins",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_GateCheckins_DocumentNumber",
                table: "GateCheckins",
                column: "DocumentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_GateCheckins_InboundOrderId",
                table: "GateCheckins",
                column: "InboundOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_GateCheckins_Status",
                table: "GateCheckins",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_GateCheckins_VehiclePlate",
                table: "GateCheckins",
                column: "VehiclePlate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GateCheckins");
        }
    }
}
