using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCLabManager.Migrations
{
    public partial class AddBatteries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Batteries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Status = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    BatteryTypeId = table.Column<int>(nullable: true),
                    CycleCount = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Batteries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Batteries_BatteryTypes_BatteryTypeId",
                        column: x => x.BatteryTypeId,
                        principalTable: "BatteryTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetUsageRecordClass",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    ProgramName = table.Column<string>(nullable: true),
                    SubProgramName = table.Column<string>(nullable: true),
                    BatteryClassId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetUsageRecordClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetUsageRecordClass_Batteries_BatteryClassId",
                        column: x => x.BatteryClassId,
                        principalTable: "Batteries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetUsageRecordClass_BatteryClassId",
                table: "AssetUsageRecordClass",
                column: "BatteryClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Batteries_BatteryTypeId",
                table: "Batteries",
                column: "BatteryTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetUsageRecordClass");

            migrationBuilder.DropTable(
                name: "Batteries");
        }
    }
}
