using Microsoft.EntityFrameworkCore.Migrations;

namespace BCLabManager.Migrations
{
    public partial class RenameCRate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Steps");

            migrationBuilder.AddColumn<double>(
                name: "CRate",
                table: "Steps",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CRate",
                table: "Steps");

            migrationBuilder.AddColumn<double>(
                name: "Capacity",
                table: "Steps",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
