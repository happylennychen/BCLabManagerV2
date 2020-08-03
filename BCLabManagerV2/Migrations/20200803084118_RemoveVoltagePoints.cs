using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCLabManager.Migrations
{
    public partial class RemoveVoltagePoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "voltage_points",
                table: "projects");

            migrationBuilder.AddColumn<List<string>>(
                name: "recipe_templates",
                table: "programs",
                nullable: true);

            migrationBuilder.AddColumn<List<int>>(
                name: "temperatures",
                table: "programs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "recipe_templates",
                table: "programs");

            migrationBuilder.DropColumn(
                name: "temperatures",
                table: "programs");

            migrationBuilder.AddColumn<string>(
                name: "voltage_points",
                table: "projects",
                type: "text",
                nullable: true);
        }
    }
}
