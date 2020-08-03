using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCLabManager.Migrations
{
    public partial class AddVoltagePoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<int>>(
                name: "voltage_points",
                table: "projects",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "voltage_points",
                table: "projects");
        }
    }
}
