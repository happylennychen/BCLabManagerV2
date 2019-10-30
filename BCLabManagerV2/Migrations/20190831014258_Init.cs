﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCLabManager.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BatteryTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Manufactor = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Material = table.Column<string>(nullable: true),
                    LimitedChargeVoltage = table.Column<int>(nullable: false),
                    RatedCapacity = table.Column<int>(nullable: false),
                    NominalVoltage = table.Column<int>(nullable: false),
                    TypicalCapacity = table.Column<int>(nullable: false),
                    CutoffDischargeVoltage = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatteryTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Programs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Requester = table.Column<string>(nullable: true),
                    RequestDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CompleteDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RawDataClass",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RawDataClass", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubProgramTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    TestCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubProgramTemplates", x => x.Id);
                });

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
                name: "SubPrograms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    TestCount = table.Column<int>(nullable: false),
                    ProgramClassId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubPrograms_Programs_ProgramClassId",
                        column: x => x.ProgramClassId,
                        principalTable: "Programs",
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

            migrationBuilder.CreateTable(
                name: "TestRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Status = table.Column<int>(nullable: false),
                    BatteryTypeStr = table.Column<string>(nullable: true),
                    BatteryStr = table.Column<string>(nullable: true),
                    TesterStr = table.Column<string>(nullable: true),
                    ChannelStr = table.Column<string>(nullable: true),
                    ChamberStr = table.Column<string>(nullable: true),
                    SubProgramStr = table.Column<string>(nullable: true),
                    ProgramStr = table.Column<string>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    Steps = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    RawDataId = table.Column<int>(nullable: true),
                    NewCycle = table.Column<double>(nullable: false),
                    SubProgramClassId = table.Column<int>(nullable: true),
                    SubProgramClassId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestRecords_RawDataClass_RawDataId",
                        column: x => x.RawDataId,
                        principalTable: "RawDataClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestRecords_SubPrograms_SubProgramClassId",
                        column: x => x.SubProgramClassId,
                        principalTable: "SubPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestRecords_SubPrograms_SubProgramClassId1",
                        column: x => x.SubProgramClassId1,
                        principalTable: "SubPrograms",
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

            migrationBuilder.CreateIndex(
                name: "IX_SubPrograms_ProgramClassId",
                table: "SubPrograms",
                column: "ProgramClassId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRecords_RawDataId",
                table: "TestRecords",
                column: "RawDataId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRecords_SubProgramClassId",
                table: "TestRecords",
                column: "SubProgramClassId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRecords_SubProgramClassId1",
                table: "TestRecords",
                column: "SubProgramClassId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetUsageRecordClass");

            migrationBuilder.DropTable(
                name: "SubProgramTemplates");

            migrationBuilder.DropTable(
                name: "TestRecords");

            migrationBuilder.DropTable(
                name: "Batteries");

            migrationBuilder.DropTable(
                name: "RawDataClass");

            migrationBuilder.DropTable(
                name: "SubPrograms");

            migrationBuilder.DropTable(
                name: "BatteryTypes");

            migrationBuilder.DropTable(
                name: "Programs");
        }
    }
}