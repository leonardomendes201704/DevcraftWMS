using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerfectApiTemplate.Infrastructure.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class SyncModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ErrorLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExceptionType = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    StackTrace = table.Column<string>(type: "TEXT", nullable: true),
                    InnerExceptions = table.Column<string>(type: "TEXT", nullable: true),
                    Method = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    Path = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    QueryString = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                    RequestHeaders = table.Column<string>(type: "TEXT", nullable: true),
                    RequestBody = table.Column<string>(type: "TEXT", nullable: true),
                    RequestBodyTruncated = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequestBodyOriginalLength = table.Column<long>(type: "INTEGER", nullable: true),
                    StatusCode = table.Column<int>(type: "INTEGER", nullable: true),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    CorrelationId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    RequestId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    TraceId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    SpanId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    EnvironmentName = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    MachineName = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    AssemblyVersion = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FinishedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DurationMs = table.Column<long>(type: "INTEGER", nullable: false),
                    Method = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    Scheme = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    Host = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Path = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    QueryString = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                    StatusCode = table.Column<int>(type: "INTEGER", nullable: false),
                    RequestContentType = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    RequestContentLength = table.Column<long>(type: "INTEGER", nullable: true),
                    ResponseContentType = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ResponseContentLength = table.Column<long>(type: "INTEGER", nullable: true),
                    RequestHeaders = table.Column<string>(type: "TEXT", nullable: true),
                    ResponseHeaders = table.Column<string>(type: "TEXT", nullable: true),
                    RequestBody = table.Column<string>(type: "TEXT", nullable: true),
                    ResponseBody = table.Column<string>(type: "TEXT", nullable: true),
                    RequestBodyTruncated = table.Column<bool>(type: "INTEGER", nullable: false),
                    ResponseBodyTruncated = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequestBodyOriginalLength = table.Column<long>(type: "INTEGER", nullable: true),
                    ResponseBodyOriginalLength = table.Column<long>(type: "INTEGER", nullable: true),
                    ClientIp = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    CorrelationId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    RequestId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    TraceId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    SpanId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EntityName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    EntityId = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Operation = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    BeforeJson = table.Column<string>(type: "TEXT", nullable: true),
                    AfterJson = table.Column<string>(type: "TEXT", nullable: true),
                    ChangedProperties = table.Column<string>(type: "TEXT", nullable: true),
                    DurationMs = table.Column<long>(type: "INTEGER", nullable: false),
                    OperationId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    CorrelationId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    RequestId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    TraceId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    SpanId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_CorrelationId",
                table: "ErrorLogs",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_CreatedAtUtc",
                table: "ErrorLogs",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_ExceptionType",
                table: "ErrorLogs",
                column: "ExceptionType");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_RequestId",
                table: "ErrorLogs",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestLogs_CorrelationId",
                table: "RequestLogs",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestLogs_Path",
                table: "RequestLogs",
                column: "Path");

            migrationBuilder.CreateIndex(
                name: "IX_RequestLogs_RequestId",
                table: "RequestLogs",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestLogs_StartedAtUtc",
                table: "RequestLogs",
                column: "StartedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_RequestLogs_StatusCode",
                table: "RequestLogs",
                column: "StatusCode");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_CorrelationId",
                table: "TransactionLogs",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_CreatedAtUtc",
                table: "TransactionLogs",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_EntityId",
                table: "TransactionLogs",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_EntityName",
                table: "TransactionLogs",
                column: "EntityName");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_RequestId",
                table: "TransactionLogs",
                column: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ErrorLogs");

            migrationBuilder.DropTable(
                name: "RequestLogs");

            migrationBuilder.DropTable(
                name: "TransactionLogs");
        }
    }
}
