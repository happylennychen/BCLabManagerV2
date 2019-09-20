using Microsoft.EntityFrameworkCore.Migrations;

namespace BCLabManager.Migrations
{
    public partial class ReplaceStatusWithUseCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Channels",
                newName: "AssetUseCount");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Chambers",
                newName: "AssetUseCount");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Batteries",
                newName: "AssetUseCount");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "AssetUsageRecordClass",
                newName: "AssetUseCount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssetUseCount",
                table: "Channels",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "AssetUseCount",
                table: "Chambers",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "AssetUseCount",
                table: "Batteries",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "AssetUseCount",
                table: "AssetUsageRecordClass",
                newName: "Status");
        }
    }
}
