using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevcraftWMS.Infrastructure.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddPutawayTaskAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedToUserEmail",
                table: "PutawayTasks",
                type: "TEXT",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedToUserId",
                table: "PutawayTasks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PutawayTaskAssignmentEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PutawayTaskId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FromUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    FromUserEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ToUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ToUserEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    AssignedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
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
                    table.PrimaryKey("PK_PutawayTaskAssignmentEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PutawayTaskAssignmentEvents_PutawayTasks_PutawayTaskId",
                        column: x => x.PutawayTaskId,
                        principalTable: "PutawayTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PutawayTaskAssignmentEvents_AssignedAtUtc",
                table: "PutawayTaskAssignmentEvents",
                column: "AssignedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_PutawayTaskAssignmentEvents_PutawayTaskId",
                table: "PutawayTaskAssignmentEvents",
                column: "PutawayTaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PutawayTaskAssignmentEvents");

            migrationBuilder.DropColumn(
                name: "AssignedToUserEmail",
                table: "PutawayTasks");

            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "PutawayTasks");
        }
    }
}
