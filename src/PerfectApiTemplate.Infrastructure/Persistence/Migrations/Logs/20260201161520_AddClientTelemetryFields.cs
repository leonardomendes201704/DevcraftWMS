using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerfectApiTemplate.Infrastructure.Persistence.Migrations.Logs
{
    /// <inheritdoc />
    public partial class AddClientTelemetryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiMethod",
                table: "ErrorLogs",
                type: "TEXT",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApiPath",
                table: "ErrorLogs",
                type: "TEXT",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApiRequestId",
                table: "ErrorLogs",
                type: "TEXT",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApiStatusCode",
                table: "ErrorLogs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientApp",
                table: "ErrorLogs",
                type: "TEXT",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientEnv",
                table: "ErrorLogs",
                type: "TEXT",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientIp",
                table: "ErrorLogs",
                type: "TEXT",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientRoute",
                table: "ErrorLogs",
                type: "TEXT",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientUrl",
                table: "ErrorLogs",
                type: "TEXT",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DetailsJson",
                table: "ErrorLogs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DurationMs",
                table: "ErrorLogs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventType",
                table: "ErrorLogs",
                type: "TEXT",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Severity",
                table: "ErrorLogs",
                type: "TEXT",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "ErrorLogs",
                type: "TEXT",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "ErrorLogs",
                type: "TEXT",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "ErrorLogs",
                type: "TEXT",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserIdText",
                table: "ErrorLogs",
                type: "TEXT",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_ApiStatusCode",
                table: "ErrorLogs",
                column: "ApiStatusCode");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_EventType",
                table: "ErrorLogs",
                column: "EventType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ErrorLogs_ApiStatusCode",
                table: "ErrorLogs");

            migrationBuilder.DropIndex(
                name: "IX_ErrorLogs_EventType",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "ApiMethod",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "ApiPath",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "ApiRequestId",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "ApiStatusCode",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "ClientApp",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "ClientEnv",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "ClientIp",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "ClientRoute",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "ClientUrl",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "DetailsJson",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "DurationMs",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "EventType",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "Severity",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "UserIdText",
                table: "ErrorLogs");
        }
    }
}
