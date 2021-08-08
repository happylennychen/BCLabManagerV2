using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BCLabManager.Migrations
{
    public partial class TMRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "table_maker_record_id",
                table: "table_maker_products",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "table_maker_records",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    table_maker_version = table.Column<string>(nullable: true),
                    project_id = table.Column<int>(nullable: true),
                    eod = table.Column<long>(nullable: false),
                    voltage_points = table.Column<List<int>>(nullable: true),
                    is_valid = table.Column<bool>(nullable: false),
                    ocv_sources = table.Column<List<string>>(nullable: true),
                    rc_sources = table.Column<List<string>>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_table_maker_records", x => x.id);
                    table.ForeignKey(
                        name: "fk_table_maker_records_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_table_maker_products_table_maker_record_id",
                table: "table_maker_products",
                column: "table_maker_record_id");

            migrationBuilder.CreateIndex(
                name: "ix_table_maker_records_project_id",
                table: "table_maker_records",
                column: "project_id");

            migrationBuilder.AddForeignKey(
                name: "fk_table_maker_products_table_maker_records_table_maker_record",
                table: "table_maker_products",
                column: "table_maker_record_id",
                principalTable: "table_maker_records",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_table_maker_products_table_maker_records_table_maker_record",
                table: "table_maker_products");

            migrationBuilder.DropTable(
                name: "table_maker_records");

            migrationBuilder.DropIndex(
                name: "ix_table_maker_products_table_maker_record_id",
                table: "table_maker_products");

            migrationBuilder.DropColumn(
                name: "table_maker_record_id",
                table: "table_maker_products");
        }
    }
}
