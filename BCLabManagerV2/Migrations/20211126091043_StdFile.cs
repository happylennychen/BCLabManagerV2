using Microsoft.EntityFrameworkCore.Migrations;

namespace BCLabManager.Migrations
{
    public partial class StdFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "std_file_path",
                table: "test_records",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "std_md5",
                table: "test_records",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "std_file_path",
                table: "test_records");

            migrationBuilder.DropColumn(
                name: "std_md5",
                table: "test_records");
        }
    }
}
