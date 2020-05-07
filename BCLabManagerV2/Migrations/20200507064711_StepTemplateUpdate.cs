using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BCLabManager.Migrations
{
    public partial class StepTemplateUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Offset",
                table: "StepTemplates");

            migrationBuilder.DropColumn(
                name: "Slope",
                table: "StepTemplates");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "StepTemplates");

            migrationBuilder.AddColumn<int>(
                name: "CoefficientId",
                table: "StepTemplates",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_StepTemplates_CoefficientId",
                table: "StepTemplates",
                column: "CoefficientId");

            migrationBuilder.AddForeignKey(
                name: "FK_StepTemplates_CoefficientClass_CoefficientId",
                table: "StepTemplates",
                column: "CoefficientId",
                principalTable: "CoefficientClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StepTemplates_CoefficientClass_CoefficientId",
                table: "StepTemplates");

            migrationBuilder.DropTable(
                name: "CoefficientClass");

            migrationBuilder.DropIndex(
                name: "IX_StepTemplates_CoefficientId",
                table: "StepTemplates");

            migrationBuilder.DropColumn(
                name: "CoefficientId",
                table: "StepTemplates");

            migrationBuilder.AddColumn<double>(
                name: "Offset",
                table: "StepTemplates",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Slope",
                table: "StepTemplates",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Temperature",
                table: "StepTemplates",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
