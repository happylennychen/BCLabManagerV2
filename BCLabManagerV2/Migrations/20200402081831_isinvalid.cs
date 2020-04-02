using Microsoft.EntityFrameworkCore.Migrations;

namespace BCLabManager.Migrations
{
    public partial class isinvalid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "Programs");

            migrationBuilder.AddColumn<bool>(
                name: "IsInvalid",
                table: "Programs",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInvalid",
                table: "Programs");

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "Programs",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
