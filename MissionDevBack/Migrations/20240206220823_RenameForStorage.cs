using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MissionDevBack.Migrations
{
    /// <inheritdoc />
    public partial class RenameForStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TempFilename",
                table: "UserFiles",
                newName: "StorageFilename");

            migrationBuilder.RenameColumn(
                name: "TempFilename",
                table: "ProjectFiles",
                newName: "StorageFilename");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StorageFilename",
                table: "UserFiles",
                newName: "TempFilename");

            migrationBuilder.RenameColumn(
                name: "StorageFilename",
                table: "ProjectFiles",
                newName: "TempFilename");
        }
    }
}
