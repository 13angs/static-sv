using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace static_sv.Migrations
{
    public partial class addrelatedfiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "file_data",
                table: "staticfiles",
                type: "BLOB",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "parent_file_id",
                table: "staticfiles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_staticfiles_parent_file_id",
                table: "staticfiles",
                column: "parent_file_id");

            migrationBuilder.AddForeignKey(
                name: "FK_staticfiles_staticfiles_parent_file_id",
                table: "staticfiles",
                column: "parent_file_id",
                principalTable: "staticfiles",
                principalColumn: "staticfile_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_staticfiles_staticfiles_parent_file_id",
                table: "staticfiles");

            migrationBuilder.DropIndex(
                name: "IX_staticfiles_parent_file_id",
                table: "staticfiles");

            migrationBuilder.DropColumn(
                name: "file_data",
                table: "staticfiles");

            migrationBuilder.DropColumn(
                name: "parent_file_id",
                table: "staticfiles");
        }
    }
}
