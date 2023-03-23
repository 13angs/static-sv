using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace static_sv.Migrations
{
    public partial class initdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "folders",
                columns: table => new
                {
                    folder_id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: true),
                    path = table.Column<string>(type: "TEXT", nullable: false),
                    parent_folder_id = table.Column<long>(type: "INTEGER", nullable: true),
                    created_date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    modified_date = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_folders", x => x.folder_id);
                    table.ForeignKey(
                        name: "FK_folders_folders_parent_folder_id",
                        column: x => x.parent_folder_id,
                        principalTable: "folders",
                        principalColumn: "folder_id");
                });

            migrationBuilder.CreateTable(
                name: "staticfiles",
                columns: table => new
                {
                    staticfile_id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: true),
                    path = table.Column<string>(type: "TEXT", nullable: false),
                    type = table.Column<string>(type: "TEXT", nullable: false),
                    size = table.Column<long>(type: "INTEGER", nullable: false),
                    folder_id = table.Column<long>(type: "INTEGER", nullable: true),
                    created_date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    modified_date = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staticfiles", x => x.staticfile_id);
                    table.ForeignKey(
                        name: "FK_staticfiles_folders_folder_id",
                        column: x => x.folder_id,
                        principalTable: "folders",
                        principalColumn: "folder_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_folders_parent_folder_id",
                table: "folders",
                column: "parent_folder_id");

            migrationBuilder.CreateIndex(
                name: "IX_staticfiles_folder_id",
                table: "staticfiles",
                column: "folder_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "staticfiles");

            migrationBuilder.DropTable(
                name: "folders");
        }
    }
}
