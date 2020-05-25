using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Manufactor = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Material = table.Column<string>(nullable: true),
                    TypicalCapacity = table.Column<int>(nullable: false),
                    LimitedChargeVoltage = table.Column<int>(nullable: false),
                    RatedCapacity = table.Column<int>(nullable: false),
                    NominalVoltage = table.Column<int>(nullable: false),
                    CutoffDischargeVoltage = table.Column<int>(nullable: false),
                    FullyChargedEndCurrent = table.Column<int>(nullable: false),
                    FullyChargedEndingTimeout = table.Column<int>(nullable: false)
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                name: "CoefficientClass",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Temperature = table.Column<double>(nullable: false),
                    Slope = table.Column<double>(nullable: false),
                    Offset = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoefficientClass", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgramTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecipeTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Testers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Customer = table.Column<string>(nullable: true),
                    BatteryTypeId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    AbsoluteMaxCapacity = table.Column<int>(nullable: false),
                    LimitedChargeVoltage = table.Column<int>(nullable: false),
                    CutoffDischargeVoltage = table.Column<int>(nullable: false),
                    VoltagePoints = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_BatteryTypes_BatteryTypeId",
                        column: x => x.BatteryTypeId,
                        principalTable: "BatteryTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StepTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CoefficientId = table.Column<int>(nullable: true),
                    CurrentInput = table.Column<double>(nullable: false),
                    CurrentUnit = table.Column<int>(nullable: false),
                    CutOffConditionValue = table.Column<double>(nullable: false),
                    CutOffConditionType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepTemplates_CoefficientClass_CoefficientId",
                        column: x => x.CoefficientId,
                        principalTable: "CoefficientClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AssetUseCount = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TesterId = table.Column<int>(nullable: true)
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
                name: "EvSettingClass",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DesignCapacity = table.Column<int>(nullable: false),
                    FullyChargedEndCurrent = table.Column<int>(nullable: false),
                    FullyChargedEndingTimeout = table.Column<int>(nullable: false),
                    DischargeEndVoltage = table.Column<int>(nullable: false),
                    LimitedChargeVoltage = table.Column<int>(nullable: false),
                    ProjectClassId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvSettingClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvSettingClass_Projects_ProjectClassId",
                        column: x => x.ProjectClassId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Programs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Order = table.Column<decimal>(nullable: false),
                    Requester = table.Column<string>(nullable: true),
                    RequestTime = table.Column<DateTime>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    EST = table.Column<DateTime>(nullable: false),
                    EET = table.Column<DateTime>(nullable: false),
                    ED = table.Column<TimeSpan>(nullable: false),
                    TypeId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsInvalid = table.Column<bool>(nullable: false),
                    TableFilePath = table.Column<string>(nullable: true),
                    ProjectId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Programs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Programs_ProgramTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "ProgramTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectProductClass",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FilePath = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    ProjectClassId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectProductClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectProductClass_Projects_ProjectClassId",
                        column: x => x.ProjectClassId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Steps",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StepTemplateId = table.Column<int>(nullable: true),
                    LoopLabel = table.Column<string>(nullable: true),
                    LoopTarget = table.Column<string>(nullable: true),
                    LoopCount = table.Column<int>(nullable: false),
                    CompareMark = table.Column<int>(nullable: false),
                    CRate = table.Column<double>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    RecipeTemplateId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Steps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Steps_RecipeTemplates_RecipeTemplateId",
                        column: x => x.RecipeTemplateId,
                        principalTable: "RecipeTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Steps_StepTemplates_StepTemplateId",
                        column: x => x.StepTemplateId,
                        principalTable: "StepTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetUsageRecordClass",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    AssetUseCount = table.Column<int>(nullable: false),
                    ProgramName = table.Column<string>(nullable: true),
                    RecipeName = table.Column<string>(nullable: true),
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
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsAbandoned = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Temperature = table.Column<double>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    EST = table.Column<DateTime>(nullable: false),
                    EET = table.Column<DateTime>(nullable: false),
                    ED = table.Column<TimeSpan>(nullable: false),
                    ProgramClassId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recipes_Programs_ProgramClassId",
                        column: x => x.ProgramClassId,
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EvResultClass",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    RecipeClassId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvResultClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvResultClass_Recipes_RecipeClassId",
                        column: x => x.RecipeClassId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StepRuntimes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StepTemplateId = table.Column<int>(nullable: true),
                    DesignCapacityInmAH = table.Column<double>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    Duration = table.Column<TimeSpan>(nullable: false),
                    EST = table.Column<DateTime>(nullable: false),
                    EET = table.Column<DateTime>(nullable: false),
                    ED = table.Column<TimeSpan>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    RecipeClassId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepRuntimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepRuntimes_Recipes_RecipeClassId",
                        column: x => x.RecipeClassId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StepRuntimes_StepTemplates_StepTemplateId",
                        column: x => x.StepTemplateId,
                        principalTable: "StepTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<int>(nullable: false),
                    BatteryTypeStr = table.Column<string>(nullable: true),
                    BatteryStr = table.Column<string>(nullable: true),
                    TesterStr = table.Column<string>(nullable: true),
                    ChannelStr = table.Column<string>(nullable: true),
                    ChamberStr = table.Column<string>(nullable: true),
                    RecipeStr = table.Column<string>(nullable: true),
                    ProgramStr = table.Column<string>(nullable: true),
                    ProjectStr = table.Column<string>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    NewCycle = table.Column<double>(nullable: false),
                    MeasurementGain = table.Column<double>(nullable: false),
                    MeasurementOffset = table.Column<double>(nullable: false),
                    TraceResistance = table.Column<double>(nullable: false),
                    CapacityDifference = table.Column<double>(nullable: false),
                    TestFilePath = table.Column<string>(nullable: true),
                    Operator = table.Column<string>(nullable: true),
                    Current = table.Column<double>(nullable: false),
                    Temperature = table.Column<double>(nullable: false),
                    AssignedBatteryId = table.Column<int>(nullable: true),
                    AssignedChamberId = table.Column<int>(nullable: true),
                    AssignedChannelId = table.Column<int>(nullable: true),
                    RecipeClassId = table.Column<int>(nullable: true)
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
                        name: "FK_TestRecords_Recipes_RecipeClassId",
                        column: x => x.RecipeClassId,
                        principalTable: "Recipes",
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
                name: "IX_EvResultClass_RecipeClassId",
                table: "EvResultClass",
                column: "RecipeClassId");

            migrationBuilder.CreateIndex(
                name: "IX_EvSettingClass_ProjectClassId",
                table: "EvSettingClass",
                column: "ProjectClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_ProjectId",
                table: "Programs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_TypeId",
                table: "Programs",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProductClass_ProjectClassId",
                table: "ProjectProductClass",
                column: "ProjectClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_BatteryTypeId",
                table: "Projects",
                column: "BatteryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_ProgramClassId",
                table: "Recipes",
                column: "ProgramClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StepRuntimes_RecipeClassId",
                table: "StepRuntimes",
                column: "RecipeClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StepRuntimes_StepTemplateId",
                table: "StepRuntimes",
                column: "StepTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Steps_RecipeTemplateId",
                table: "Steps",
                column: "RecipeTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Steps_StepTemplateId",
                table: "Steps",
                column: "StepTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_StepTemplates_CoefficientId",
                table: "StepTemplates",
                column: "CoefficientId");

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
                name: "IX_TestRecords_RecipeClassId",
                table: "TestRecords",
                column: "RecipeClassId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetUsageRecordClass");

            migrationBuilder.DropTable(
                name: "EvResultClass");

            migrationBuilder.DropTable(
                name: "EvSettingClass");

            migrationBuilder.DropTable(
                name: "ProjectProductClass");

            migrationBuilder.DropTable(
                name: "StepRuntimes");

            migrationBuilder.DropTable(
                name: "Steps");

            migrationBuilder.DropTable(
                name: "TestRecords");

            migrationBuilder.DropTable(
                name: "RecipeTemplates");

            migrationBuilder.DropTable(
                name: "StepTemplates");

            migrationBuilder.DropTable(
                name: "Batteries");

            migrationBuilder.DropTable(
                name: "Chambers");

            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropTable(
                name: "Recipes");

            migrationBuilder.DropTable(
                name: "CoefficientClass");

            migrationBuilder.DropTable(
                name: "Testers");

            migrationBuilder.DropTable(
                name: "Programs");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "ProgramTypes");

            migrationBuilder.DropTable(
                name: "BatteryTypes");
        }
    }
}
