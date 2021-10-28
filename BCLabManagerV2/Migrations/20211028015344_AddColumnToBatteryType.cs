using Microsoft.EntityFrameworkCore.Migrations;

namespace BCLabManager.Migrations
{
    public partial class AddColumnToBatteryType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "charge_current",
                table: "battery_types",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "charge_high_temp",
                table: "battery_types",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "charge_low_temp",
                table: "battery_types",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "discharge_high_temp",
                table: "battery_types",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "discharge_low_temp",
                table: "battery_types",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "charge_current",
                table: "battery_types");

            migrationBuilder.DropColumn(
                name: "charge_high_temp",
                table: "battery_types");

            migrationBuilder.DropColumn(
                name: "charge_low_temp",
                table: "battery_types");

            migrationBuilder.DropColumn(
                name: "discharge_high_temp",
                table: "battery_types");

            migrationBuilder.DropColumn(
                name: "discharge_low_temp",
                table: "battery_types");
        }
    }
}
