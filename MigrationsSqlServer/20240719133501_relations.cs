using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenericDataStore.Migrations
{
    /// <inheritdoc />
    public partial class relations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConnectedTable",
                table: "Field");

            migrationBuilder.DropColumn(
                name: "IsKey",
                table: "Field");

            migrationBuilder.DropColumn(
                name: "Parent",
                table: "Field");

            migrationBuilder.CreateTable(
                name: "DatabaseTableRelations",
                columns: table => new
                {
                    DatabaseTableRelationsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FKName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentTable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentPropertyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentColumnId = table.Column<int>(type: "int", nullable: false),
                    ChildTable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChildTableId = table.Column<int>(type: "int", nullable: false),
                    ChildPropertyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Virtual = table.Column<bool>(type: "bit", nullable: false),
                    ParentObjecttypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChildObjecttypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatabaseTableRelations", x => x.DatabaseTableRelationsId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatabaseTableRelations");

            migrationBuilder.AddColumn<string>(
                name: "ConnectedTable",
                table: "Field",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsKey",
                table: "Field",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Parent",
                table: "Field",
                type: "bit",
                nullable: true);
        }
    }
}
