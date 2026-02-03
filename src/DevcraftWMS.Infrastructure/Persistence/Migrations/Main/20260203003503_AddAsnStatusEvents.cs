using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevcraftWMS.Infrastructure.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddAsnStatusEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AsnStatusEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AsnId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FromStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    ToStatus = table.Column<int>(type: "INTEGER", nullable: false),
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
                    table.PrimaryKey("PK_AsnStatusEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AsnStatusEvents_Asns_AsnId",
                        column: x => x.AsnId,
                        principalTable: "Asns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsnStatusEvents_AsnId",
                table: "AsnStatusEvents",
                column: "AsnId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsnStatusEvents");
        }
    }
}
