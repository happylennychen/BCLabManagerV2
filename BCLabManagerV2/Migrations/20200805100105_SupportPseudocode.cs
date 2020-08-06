using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BCLabManager.Migrations
{
    public partial class SupportPseudocode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "ed1",
                table: "recipes",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ed2",
                table: "recipes",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "recipe_template_id",
                table: "recipes",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "protection",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    parameter = table.Column<int>(nullable: false),
                    mark = table.Column<int>(nullable: false),
                    value = table.Column<int>(nullable: false),
                    recipe_template_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_protection", x => x.id);
                    table.ForeignKey(
                        name: "fk_protection_recipe_templates_recipe_template_id",
                        column: x => x.recipe_template_id,
                        principalTable: "recipe_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tester_action",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mode = table.Column<int>(nullable: false),
                    voltage = table.Column<int>(nullable: false),
                    current = table.Column<int>(nullable: false),
                    power = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tester_action", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "step_v2",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    index = table.Column<int>(nullable: false),
                    rest = table.Column<int>(nullable: false),
                    prerest = table.Column<int>(nullable: false),
                    loop1label = table.Column<string>(nullable: true),
                    loop2label = table.Column<string>(nullable: true),
                    action_id = table.Column<int>(nullable: true),
                    recipe_template_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_step_v2", x => x.id);
                    table.ForeignKey(
                        name: "fk_step_v2_tester_action_action_id",
                        column: x => x.action_id,
                        principalTable: "tester_action",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_step_v2_recipe_templates_recipe_template_id",
                        column: x => x.recipe_template_id,
                        principalTable: "recipe_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cut_off_condition",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    parameter = table.Column<int>(nullable: false),
                    mark = table.Column<int>(nullable: false),
                    value = table.Column<int>(nullable: false),
                    jump_type = table.Column<int>(nullable: false),
                    index = table.Column<int>(nullable: false),
                    loop1target = table.Column<string>(nullable: true),
                    loop1count = table.Column<int>(nullable: false),
                    loop2target = table.Column<string>(nullable: true),
                    loop2count = table.Column<int>(nullable: false),
                    step_v2id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cut_off_condition", x => x.id);
                    table.ForeignKey(
                        name: "fk_cut_off_condition_step_v2_step_v2id",
                        column: x => x.step_v2id,
                        principalTable: "step_v2",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_recipes_recipe_template_id",
                table: "recipes",
                column: "recipe_template_id");

            migrationBuilder.CreateIndex(
                name: "ix_cut_off_condition_step_v2id",
                table: "cut_off_condition",
                column: "step_v2id");

            migrationBuilder.CreateIndex(
                name: "ix_protection_recipe_template_id",
                table: "protection",
                column: "recipe_template_id");

            migrationBuilder.CreateIndex(
                name: "ix_step_v2_action_id",
                table: "step_v2",
                column: "action_id");

            migrationBuilder.CreateIndex(
                name: "ix_step_v2_recipe_template_id",
                table: "step_v2",
                column: "recipe_template_id");

            migrationBuilder.AddForeignKey(
                name: "fk_recipes_recipe_templates_recipe_template_id",
                table: "recipes",
                column: "recipe_template_id",
                principalTable: "recipe_templates",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_recipes_recipe_templates_recipe_template_id",
                table: "recipes");

            migrationBuilder.DropTable(
                name: "cut_off_condition");

            migrationBuilder.DropTable(
                name: "protection");

            migrationBuilder.DropTable(
                name: "step_v2");

            migrationBuilder.DropTable(
                name: "tester_action");

            migrationBuilder.DropIndex(
                name: "ix_recipes_recipe_template_id",
                table: "recipes");

            migrationBuilder.DropColumn(
                name: "ed1",
                table: "recipes");

            migrationBuilder.DropColumn(
                name: "ed2",
                table: "recipes");

            migrationBuilder.DropColumn(
                name: "recipe_template_id",
                table: "recipes");
        }
    }
}
