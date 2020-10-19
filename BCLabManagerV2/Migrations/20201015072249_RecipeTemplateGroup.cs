using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BCLabManager.Migrations
{
    public partial class RecipeTemplateGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "editable",
                table: "recipe_templates",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "group_id",
                table: "recipe_templates",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "recipe_template_groups",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recipe_template_groups", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_recipe_templates_group_id",
                table: "recipe_templates",
                column: "group_id");

            migrationBuilder.AddForeignKey(
                name: "fk_recipe_templates_recipe_template_groups_group_id",
                table: "recipe_templates",
                column: "group_id",
                principalTable: "recipe_template_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_recipe_templates_recipe_template_groups_group_id",
                table: "recipe_templates");

            migrationBuilder.DropTable(
                name: "recipe_template_groups");

            migrationBuilder.DropIndex(
                name: "ix_recipe_templates_group_id",
                table: "recipe_templates");

            migrationBuilder.DropColumn(
                name: "editable",
                table: "recipe_templates");

            migrationBuilder.DropColumn(
                name: "group_id",
                table: "recipe_templates");
        }
    }
}
