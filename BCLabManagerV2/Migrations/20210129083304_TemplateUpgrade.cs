using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BCLabManager.Migrations
{
    public partial class TemplateUpgrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "step_v2id",
                table: "protections",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "condition",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    parameter = table.Column<int>(nullable: false),
                    mark = table.Column<int>(nullable: false),
                    value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_condition", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cut_off_behavior",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    condition_id = table.Column<int>(nullable: true),
                    loop1target = table.Column<string>(nullable: true),
                    loop1count = table.Column<int>(nullable: false),
                    loop2target = table.Column<string>(nullable: true),
                    loop2count = table.Column<int>(nullable: false),
                    step_v2id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cut_off_behavior", x => x.id);
                    table.ForeignKey(
                        name: "fk_cut_off_behavior_condition_condition_id",
                        column: x => x.condition_id,
                        principalTable: "condition",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cut_off_behavior_step_v2_step_v2id",
                        column: x => x.step_v2id,
                        principalTable: "step_v2",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "jump_behavior",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    condition_id = table.Column<int>(nullable: true),
                    jump_type = table.Column<int>(nullable: false),
                    index = table.Column<int>(nullable: false),
                    cut_off_behavior_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_jump_behavior", x => x.id);
                    table.ForeignKey(
                        name: "fk_jump_behavior_condition_condition_id",
                        column: x => x.condition_id,
                        principalTable: "condition",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_jump_behavior_cut_off_behavior_cut_off_behavior_id",
                        column: x => x.cut_off_behavior_id,
                        principalTable: "cut_off_behavior",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_protections_step_v2id",
                table: "protections",
                column: "step_v2id");

            migrationBuilder.CreateIndex(
                name: "ix_cut_off_behavior_condition_id",
                table: "cut_off_behavior",
                column: "condition_id");

            migrationBuilder.CreateIndex(
                name: "ix_cut_off_behavior_step_v2id",
                table: "cut_off_behavior",
                column: "step_v2id");

            migrationBuilder.CreateIndex(
                name: "ix_jump_behavior_condition_id",
                table: "jump_behavior",
                column: "condition_id");

            migrationBuilder.CreateIndex(
                name: "ix_jump_behavior_cut_off_behavior_id",
                table: "jump_behavior",
                column: "cut_off_behavior_id");

            migrationBuilder.AddForeignKey(
                name: "fk_protections_step_v2_step_v2id",
                table: "protections",
                column: "step_v2id",
                principalTable: "step_v2",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_protections_step_v2_step_v2id",
                table: "protections");

            migrationBuilder.DropTable(
                name: "jump_behavior");

            migrationBuilder.DropTable(
                name: "cut_off_behavior");

            migrationBuilder.DropTable(
                name: "condition");

            migrationBuilder.DropIndex(
                name: "ix_protections_step_v2id",
                table: "protections");

            migrationBuilder.DropColumn(
                name: "step_v2id",
                table: "protections");
        }
    }
}
