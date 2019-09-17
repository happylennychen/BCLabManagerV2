using System;
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
                name: "Chambers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Status = table.Column<int>(nullable: false),
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
                name: "ChargeCurrents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeCurrents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChargeTemperatures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeTemperatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DischargeCurrents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DischargeCurrents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DischargeTemperatures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DischargeTemperatures", x => x.Id);
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
                name: "SubProgramTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChargeTemperatureId = table.Column<int>(nullable: true),
                    ChargeCurrentId = table.Column<int>(nullable: true),
                    DischargeTemperatureId = table.Column<int>(nullable: true),
                    DischargeCurrentId = table.Column<int>(nullable: true),
                    TestCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubProgramTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubProgramTemplates_ChargeCurrents_ChargeCurrentId",
                        column: x => x.ChargeCurrentId,
                        principalTable: "ChargeCurrents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubProgramTemplates_ChargeTemperatures_ChargeTemperatureId",
                        column: x => x.ChargeTemperatureId,
                        principalTable: "ChargeTemperatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubProgramTemplates_DischargeCurrents_DischargeCurrentId",
                        column: x => x.DischargeCurrentId,
                        principalTable: "DischargeCurrents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubProgramTemplates_DischargeTemperatures_DischargeTemperatureId",
                        column: x => x.DischargeTemperatureId,
                        principalTable: "DischargeTemperatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubPrograms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChargeTemperatureId = table.Column<int>(nullable: true),
                    ChargeCurrentId = table.Column<int>(nullable: true),
                    DischargeTemperatureId = table.Column<int>(nullable: true),
                    DischargeCurrentId = table.Column<int>(nullable: true),
                    TestCount = table.Column<int>(nullable: false),
                    ProgramClassId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubPrograms_ChargeCurrents_ChargeCurrentId",
                        column: x => x.ChargeCurrentId,
                        principalTable: "ChargeCurrents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubPrograms_ChargeTemperatures_ChargeTemperatureId",
                        column: x => x.ChargeTemperatureId,
                        principalTable: "ChargeTemperatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubPrograms_DischargeCurrents_DischargeCurrentId",
                        column: x => x.DischargeCurrentId,
                        principalTable: "DischargeCurrents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubPrograms_DischargeTemperatures_DischargeTemperatureId",
                        column: x => x.DischargeTemperatureId,
                        principalTable: "DischargeTemperatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubPrograms_Programs_ProgramClassId",
                        column: x => x.ProgramClassId,
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Status = table.Column<int>(nullable: false),
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
                name: "AssetUsageRecordClass",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
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
                name: "IX_RawDataClass_TestRecordClassId",
                table: "RawDataClass",
                column: "TestRecordClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SubPrograms_ChargeCurrentId",
                table: "SubPrograms",
                column: "ChargeCurrentId");

            migrationBuilder.CreateIndex(
                name: "IX_SubPrograms_ChargeTemperatureId",
                table: "SubPrograms",
                column: "ChargeTemperatureId");

            migrationBuilder.CreateIndex(
                name: "IX_SubPrograms_DischargeCurrentId",
                table: "SubPrograms",
                column: "DischargeCurrentId");

            migrationBuilder.CreateIndex(
                name: "IX_SubPrograms_DischargeTemperatureId",
                table: "SubPrograms",
                column: "DischargeTemperatureId");

            migrationBuilder.CreateIndex(
                name: "IX_SubPrograms_ProgramClassId",
                table: "SubPrograms",
                column: "ProgramClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SubProgramTemplates_ChargeCurrentId",
                table: "SubProgramTemplates",
                column: "ChargeCurrentId");

            migrationBuilder.CreateIndex(
                name: "IX_SubProgramTemplates_ChargeTemperatureId",
                table: "SubProgramTemplates",
                column: "ChargeTemperatureId");

            migrationBuilder.CreateIndex(
                name: "IX_SubProgramTemplates_DischargeCurrentId",
                table: "SubProgramTemplates",
                column: "DischargeCurrentId");

            migrationBuilder.CreateIndex(
                name: "IX_SubProgramTemplates_DischargeTemperatureId",
                table: "SubProgramTemplates",
                column: "DischargeTemperatureId");

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
                name: "AssetUsageRecordClass");

            migrationBuilder.DropTable(
                name: "RawDataClass");

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
                name: "BatteryTypes");

            migrationBuilder.DropTable(
                name: "Testers");

            migrationBuilder.DropTable(
                name: "ChargeCurrents");

            migrationBuilder.DropTable(
                name: "ChargeTemperatures");

            migrationBuilder.DropTable(
                name: "DischargeCurrents");

            migrationBuilder.DropTable(
                name: "DischargeTemperatures");

            migrationBuilder.DropTable(
                name: "Programs");
        }
    }
}
