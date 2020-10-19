using Microsoft.EntityFrameworkCore.Migrations;

namespace BCLabManager.Migrations
{
    public partial class Protections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_protection_recipe_templates_recipe_template_id",
                table: "protection");

            migrationBuilder.DropForeignKey(
                name: "fk_recipes_recipe_templates_recipe_template_id",
                table: "recipes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_protection",
                table: "protection");

            migrationBuilder.RenameTable(
                name: "protection",
                newName: "protections");

            migrationBuilder.RenameIndex(
                name: "ix_protection_recipe_template_id",
                table: "protections",
                newName: "ix_protections_recipe_template_id");

            migrationBuilder.AlterColumn<int>(
                name: "recipe_template_id",
                table: "recipes",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_protections",
                table: "protections",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_protections_recipe_templates_recipe_template_id",
                table: "protections",
                column: "recipe_template_id",
                principalTable: "recipe_templates",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_recipes_recipe_templates_recipe_template_id",
                table: "recipes",
                column: "recipe_template_id",
                principalTable: "recipe_templates",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_protections_recipe_templates_recipe_template_id",
                table: "protections");

            migrationBuilder.DropForeignKey(
                name: "fk_recipes_recipe_templates_recipe_template_id",
                table: "recipes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_protections",
                table: "protections");

            migrationBuilder.RenameTable(
                name: "protections",
                newName: "protection");

            migrationBuilder.RenameIndex(
                name: "ix_protections_recipe_template_id",
                table: "protection",
                newName: "ix_protection_recipe_template_id");

            migrationBuilder.AlterColumn<int>(
                name: "recipe_template_id",
                table: "recipes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey(
                name: "pk_protection",
                table: "protection",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_protection_recipe_templates_recipe_template_id",
                table: "protection",
                column: "recipe_template_id",
                principalTable: "recipe_templates",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_recipes_recipe_templates_recipe_template_id",
                table: "recipes",
                column: "recipe_template_id",
                principalTable: "recipe_templates",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
