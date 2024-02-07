using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MissionDevBack.Migrations
{
    /// <inheritdoc />
    public partial class AddedMimeTypeToFilesModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MimeType",
                table: "UserFiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MimeType",
                table: "ProjectFiles",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MimeType",
                table: "UserFiles");

            migrationBuilder.DropColumn(
                name: "MimeType",
                table: "ProjectFiles");
        }
    }
}
