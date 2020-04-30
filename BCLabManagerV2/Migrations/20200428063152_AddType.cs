using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BCLabManager.Migrations
{
    public partial class AddType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "Programs",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Programs_TypeId",
                table: "Programs",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Programs_ProgramTypes_TypeId",
                table: "Programs",
                column: "TypeId",
                principalTable: "ProgramTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Programs_ProgramTypes_TypeId",
                table: "Programs");

            migrationBuilder.DropTable(
                name: "ProgramTypes");

            migrationBuilder.DropIndex(
                name: "IX_Programs_TypeId",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Programs");
        }
    }
}
