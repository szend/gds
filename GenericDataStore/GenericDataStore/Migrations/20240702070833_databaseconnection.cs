using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenericDataStore.Migrations
{
    /// <inheritdoc />
    public partial class databaseconnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DatabaseConnectionPropertyId",
                table: "ObjectType",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DatabaseConnectionProperty",
                columns: table => new
                {
                    DatabaseConnectionPropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConnectionString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatabaseType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatabaseName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatabaseConnectionProperty", x => x.DatabaseConnectionPropertyId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectType_DatabaseConnectionPropertyId",
                table: "ObjectType",
                column: "DatabaseConnectionPropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectType_DatabaseConnectionProperty_DatabaseConnectionPropertyId",
                table: "ObjectType",
                column: "DatabaseConnectionPropertyId",
                principalTable: "DatabaseConnectionProperty",
                principalColumn: "DatabaseConnectionPropertyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObjectType_DatabaseConnectionProperty_DatabaseConnectionPropertyId",
                table: "ObjectType");

            migrationBuilder.DropTable(
                name: "DatabaseConnectionProperty");

            migrationBuilder.DropIndex(
                name: "IX_ObjectType_DatabaseConnectionPropertyId",
                table: "ObjectType");

            migrationBuilder.DropColumn(
                name: "DatabaseConnectionPropertyId",
                table: "ObjectType");
        }
    }
}
