using Microsoft.EntityFrameworkCore.Migrations;

namespace BCLabManager.Migrations
{
    public partial class ProjectStr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Programs");

            migrationBuilder.AddColumn<string>(
                name: "ProjectStr",
                table: "TestRecords",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectStr",
                table: "TestRecords");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Programs",
                type: "text",
                nullable: true);
        }
    }
}
