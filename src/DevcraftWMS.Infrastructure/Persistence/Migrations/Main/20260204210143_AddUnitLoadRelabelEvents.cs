using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevcraftWMS.Infrastructure.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddUnitLoadRelabelEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UnitLoadRelabelEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UnitLoadId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OldSsccInternal = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    NewSsccInternal = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    RelabeledAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
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
                    table.PrimaryKey("PK_UnitLoadRelabelEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitLoadRelabelEvents_UnitLoads_UnitLoadId",
                        column: x => x.UnitLoadId,
                        principalTable: "UnitLoads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnitLoadRelabelEvents_RelabeledAtUtc",
                table: "UnitLoadRelabelEvents",
                column: "RelabeledAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_UnitLoadRelabelEvents_UnitLoadId",
                table: "UnitLoadRelabelEvents",
                column: "UnitLoadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnitLoadRelabelEvents");
        }
    }
}
