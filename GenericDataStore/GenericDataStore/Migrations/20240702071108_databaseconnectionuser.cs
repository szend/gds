using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenericDataStore.Migrations
{
    /// <inheritdoc />
    public partial class databaseconnectionuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AppUserId",
                table: "DatabaseConnectionProperty",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DatabaseConnectionProperty_AppUserId",
                table: "DatabaseConnectionProperty",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DatabaseConnectionProperty_AspNetUsers_AppUserId",
                table: "DatabaseConnectionProperty",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatabaseConnectionProperty_AspNetUsers_AppUserId",
                table: "DatabaseConnectionProperty");

            migrationBuilder.DropIndex(
                name: "IX_DatabaseConnectionProperty_AppUserId",
                table: "DatabaseConnectionProperty");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "DatabaseConnectionProperty");
        }
    }
}
