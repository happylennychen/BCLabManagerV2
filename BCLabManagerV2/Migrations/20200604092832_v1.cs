using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BCLabManager.Migrations
{
    public partial class v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_emulator_settings_projects_project_class_id",
                table: "emulator_settings");

            migrationBuilder.DropPrimaryKey(
                name: "pk_emulator_settings",
                table: "emulator_settings");

            migrationBuilder.DropColumn(
                name: "name",
                table: "emulator_results");

            migrationBuilder.RenameTable(
                name: "emulator_settings",
                newName: "project_settings");

            migrationBuilder.RenameIndex(
                name: "ix_emulator_settings_project_class_id",
                table: "project_settings",
                newName: "ix_project_settings_project_class_id");

            migrationBuilder.AddColumn<string>(
                name: "em_current",
                table: "emulator_results",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "em_temperature",
                table: "emulator_results",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "emulator_info",
                table: "emulator_results",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "error_rsoc",
                table: "emulator_results",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "error_tte",
                table: "emulator_results",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_valid",
                table: "emulator_results",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "log_file_path",
                table: "emulator_results",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "package_file_path",
                table: "emulator_results",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rsoc_png_file_path",
                table: "emulator_results",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tte_png_file_path",
                table: "emulator_results",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_project_settings",
                table: "project_settings",
                column: "id");

            migrationBuilder.CreateTable(
                name: "lib_fgs",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    libfg_model_code = table.Column<int>(nullable: false),
                    libfg_version = table.Column<int>(nullable: false),
                    libfg_sample_file_path = table.Column<string>(nullable: true),
                    libfg_dev_pack_file_path = table.Column<string>(nullable: true),
                    libfg_dll_path = table.Column<string>(nullable: true),
                    is_valid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lib_fgs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "release_packages",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    error_rsoc = table.Column<string>(nullable: true),
                    error_tte = table.Column<string>(nullable: true),
                    result_file_path = table.Column<string>(nullable: true),
                    package_file_path = table.Column<string>(nullable: true),
                    emulator_info = table.Column<string>(nullable: true),
                    is_valid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_release_packages", x => x.id);
                });

            migrationBuilder.AddForeignKey(
                name: "fk_project_settings_projects_project_class_id",
                table: "project_settings",
                column: "project_class_id",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_project_settings_projects_project_class_id",
                table: "project_settings");

            migrationBuilder.DropTable(
                name: "lib_fgs");

            migrationBuilder.DropTable(
                name: "release_packages");

            migrationBuilder.DropPrimaryKey(
                name: "pk_project_settings",
                table: "project_settings");

            migrationBuilder.DropColumn(
                name: "em_current",
                table: "emulator_results");

            migrationBuilder.DropColumn(
                name: "em_temperature",
                table: "emulator_results");

            migrationBuilder.DropColumn(
                name: "emulator_info",
                table: "emulator_results");

            migrationBuilder.DropColumn(
                name: "error_rsoc",
                table: "emulator_results");

            migrationBuilder.DropColumn(
                name: "error_tte",
                table: "emulator_results");

            migrationBuilder.DropColumn(
                name: "is_valid",
                table: "emulator_results");

            migrationBuilder.DropColumn(
                name: "log_file_path",
                table: "emulator_results");

            migrationBuilder.DropColumn(
                name: "package_file_path",
                table: "emulator_results");

            migrationBuilder.DropColumn(
                name: "rsoc_png_file_path",
                table: "emulator_results");

            migrationBuilder.DropColumn(
                name: "tte_png_file_path",
                table: "emulator_results");

            migrationBuilder.RenameTable(
                name: "project_settings",
                newName: "emulator_settings");

            migrationBuilder.RenameIndex(
                name: "ix_project_settings_project_class_id",
                table: "emulator_settings",
                newName: "ix_emulator_settings_project_class_id");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "emulator_results",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_emulator_settings",
                table: "emulator_settings",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_emulator_settings_projects_project_class_id",
                table: "emulator_settings",
                column: "project_class_id",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
