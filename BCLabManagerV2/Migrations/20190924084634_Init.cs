using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCLabManager.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbsoluteCurrents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbsoluteCurrents", x => x.Id);
                });

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
                name: "Chambers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AssetUseCount = table.Column<int>(nullable: false),
                    Manufactor = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    LowestTemperature = table.Column<double>(nullable: false),
                    HighestTemperature = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chambers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DynamicCurrents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicCurrents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PercentageCurrents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PercentageCurrents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubProgramTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChargeTemperature = table.Column<double>(nullable: false),
                    ChargeCurrent = table.Column<double>(nullable: false),
                    ChargeCurrentType = table.Column<int>(nullable: false),
                    DischargeTemperature = table.Column<double>(nullable: false),
                    DischargeCurrent = table.Column<double>(nullable: false),
                    DischargeCurrentType = table.Column<int>(nullable: false),
                    TestCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubProgramTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Temperatures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Temperatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Testers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Manufactor = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Testers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Batteries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AssetUseCount = table.Column<int>(nullable: false),
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
                name: "Programs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    BatteryTypeId = table.Column<int>(nullable: true),
                    Requester = table.Column<string>(nullable: true),
                    RequestDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CompleteDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Programs_BatteryTypes_BatteryTypeId",
                        column: x => x.BatteryTypeId,
                        principalTable: "BatteryTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EstimateTimeRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BatteryTypeId = table.Column<int>(nullable: true),
                    SubTemplateId = table.Column<int>(nullable: true),
                    TestCount = table.Column<int>(nullable: false),
                    ExecutedCount = table.Column<int>(nullable: false),
                    AverageTime = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimateTimeRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstimateTimeRecords_BatteryTypes_BatteryTypeId",
                        column: x => x.BatteryTypeId,
                        principalTable: "BatteryTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EstimateTimeRecords_SubProgramTemplates_SubTemplateId",
                        column: x => x.SubTemplateId,
                        principalTable: "SubProgramTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AssetUseCount = table.Column<int>(nullable: false),
                    TesterId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Channels_Testers_TesterId",
                        column: x => x.TesterId,
                        principalTable: "Testers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubPrograms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsAbandoned = table.Column<bool>(nullable: false),
                    ChargeTemperature = table.Column<double>(nullable: false),
                    DischargeTemperature = table.Column<double>(nullable: false),
                    ChargeCurrent = table.Column<double>(nullable: false),
                    DischargeCurrent = table.Column<double>(nullable: false),
                    TestCount = table.Column<int>(nullable: false),
                    Loop = table.Column<int>(nullable: false),
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
                    AssetUseCount = table.Column<int>(nullable: false),
                    ProgramName = table.Column<string>(nullable: true),
                    SubProgramName = table.Column<string>(nullable: true),
                    BatteryClassId = table.Column<int>(nullable: true),
                    ChamberClassId = table.Column<int>(nullable: true),
                    ChannelClassId = table.Column<int>(nullable: true)
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
                    table.ForeignKey(
                        name: "FK_AssetUsageRecordClass_Chambers_ChamberClassId",
                        column: x => x.ChamberClassId,
                        principalTable: "Chambers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetUsageRecordClass_Channels_ChannelClassId",
                        column: x => x.ChannelClassId,
                        principalTable: "Channels",
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
                    NewCycle = table.Column<double>(nullable: false),
                    AssignedBatteryId = table.Column<int>(nullable: true),
                    AssignedChamberId = table.Column<int>(nullable: true),
                    AssignedChannelId = table.Column<int>(nullable: true),
                    SubProgramClassId = table.Column<int>(nullable: true),
                    SubProgramClassId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestRecords_Batteries_AssignedBatteryId",
                        column: x => x.AssignedBatteryId,
                        principalTable: "Batteries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestRecords_Chambers_AssignedChamberId",
                        column: x => x.AssignedChamberId,
                        principalTable: "Chambers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestRecords_Channels_AssignedChannelId",
                        column: x => x.AssignedChannelId,
                        principalTable: "Channels",
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

            migrationBuilder.CreateTable(
                name: "RawDataClass",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileName = table.Column<string>(nullable: true),
                    MD5 = table.Column<string>(nullable: true),
                    TestRecordClassId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RawDataClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RawDataClass_TestRecords_TestRecordClassId",
                        column: x => x.TestRecordClassId,
                        principalTable: "TestRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetUsageRecordClass_BatteryClassId",
                table: "AssetUsageRecordClass",
                column: "BatteryClassId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetUsageRecordClass_ChamberClassId",
                table: "AssetUsageRecordClass",
                column: "ChamberClassId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetUsageRecordClass_ChannelClassId",
                table: "AssetUsageRecordClass",
                column: "ChannelClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Batteries_BatteryTypeId",
                table: "Batteries",
                column: "BatteryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_TesterId",
                table: "Channels",
                column: "TesterId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimateTimeRecords_BatteryTypeId",
                table: "EstimateTimeRecords",
                column: "BatteryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimateTimeRecords_SubTemplateId",
                table: "EstimateTimeRecords",
                column: "SubTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_BatteryTypeId",
                table: "Programs",
                column: "BatteryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RawDataClass_TestRecordClassId",
                table: "RawDataClass",
                column: "TestRecordClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SubPrograms_ProgramClassId",
                table: "SubPrograms",
                column: "ProgramClassId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRecords_AssignedBatteryId",
                table: "TestRecords",
                column: "AssignedBatteryId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRecords_AssignedChamberId",
                table: "TestRecords",
                column: "AssignedChamberId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRecords_AssignedChannelId",
                table: "TestRecords",
                column: "AssignedChannelId");

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
                name: "AbsoluteCurrents");

            migrationBuilder.DropTable(
                name: "AssetUsageRecordClass");

            migrationBuilder.DropTable(
                name: "DynamicCurrents");

            migrationBuilder.DropTable(
                name: "EstimateTimeRecords");

            migrationBuilder.DropTable(
                name: "PercentageCurrents");

            migrationBuilder.DropTable(
                name: "RawDataClass");

            migrationBuilder.DropTable(
                name: "Temperatures");

            migrationBuilder.DropTable(
                name: "SubProgramTemplates");

            migrationBuilder.DropTable(
                name: "TestRecords");

            migrationBuilder.DropTable(
                name: "Batteries");

            migrationBuilder.DropTable(
                name: "Chambers");

            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropTable(
                name: "SubPrograms");

            migrationBuilder.DropTable(
                name: "Testers");

            migrationBuilder.DropTable(
                name: "Programs");

            migrationBuilder.DropTable(
                name: "BatteryTypes");
        }
    }
}
