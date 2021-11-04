using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCLabManager.Migrations
{
    public partial class AddStartStopTimesToProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "planed_days",
                table: "projects",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<List<DateTime>>(
                name: "start_times",
                table: "projects",
                nullable: true);

            migrationBuilder.AddColumn<List<DateTime>>(
                name: "stop_times",
                table: "projects",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "planed_days",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "start_times",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "stop_times",
                table: "projects");
        }
    }
}
