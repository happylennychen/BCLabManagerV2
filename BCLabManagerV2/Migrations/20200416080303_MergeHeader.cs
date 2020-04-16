using Microsoft.EntityFrameworkCore.Migrations;

namespace BCLabManager.Migrations
{
    public partial class MergeHeader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Current",
                table: "RecipeTemplates");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "RecipeTemplates");

            migrationBuilder.DropColumn(
                name: "Current",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Programs");

            migrationBuilder.AddColumn<double>(
                name: "Current",
                table: "TestRecords",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "ProjectStr",
                table: "TestRecords",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Temperature",
                table: "TestRecords",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Current",
                table: "TestRecords");

            migrationBuilder.DropColumn(
                name: "ProjectStr",
                table: "TestRecords");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "TestRecords");

            migrationBuilder.AddColumn<double>(
                name: "Current",
                table: "RecipeTemplates",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Temperature",
                table: "RecipeTemplates",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Current",
                table: "Recipes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Temperature",
                table: "Recipes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Programs",
                type: "text",
                nullable: true);
        }
    }
}
