using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevcraftWMS.Infrastructure.Persistence.Migrations.Main
{
    /// <inheritdoc />
    public partial class UpdateAsnAttachmentsStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "AsnAttachments");

            migrationBuilder.AddColumn<string>(
                name: "ContentBase64",
                table: "AsnAttachments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentHash",
                table: "AsnAttachments",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StorageKey",
                table: "AsnAttachments",
                type: "TEXT",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StorageProvider",
                table: "AsnAttachments",
                type: "TEXT",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StorageUrl",
                table: "AsnAttachments",
                type: "TEXT",
                maxLength: 1024,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentBase64",
                table: "AsnAttachments");

            migrationBuilder.DropColumn(
                name: "ContentHash",
                table: "AsnAttachments");

            migrationBuilder.DropColumn(
                name: "StorageKey",
                table: "AsnAttachments");

            migrationBuilder.DropColumn(
                name: "StorageProvider",
                table: "AsnAttachments");

            migrationBuilder.DropColumn(
                name: "StorageUrl",
                table: "AsnAttachments");

            migrationBuilder.AddColumn<byte[]>(
                name: "Content",
                table: "AsnAttachments",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
