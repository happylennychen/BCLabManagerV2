using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCLabManager.Migrations
{
    public partial class ProjectProgress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "create_day",
                table: "projects",
                nullable: true,
                defaultValue: null);

            migrationBuilder.AddColumn<DateTime>(
                name: "release_day",
                table: "projects",
                nullable: true,
                defaultValue: null);

            migrationBuilder.AddColumn<DateTime>(
                name: "stage1complete_day",
                table: "projects",
                nullable: true,
                defaultValue: null);

            migrationBuilder.AddColumn<DateTime>(
                name: "stage2complete_day",
                table: "projects",
                nullable: true,
                defaultValue: null);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "create_day",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "release_day",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "stage1complete_day",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "stage2complete_day",
                table: "projects");
        }
    }
}
