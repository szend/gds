using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenericDataStore.Migrations
{
    /// <inheritdoc />
    public partial class fieldconnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjectTypeObjectType");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "ObjectTypeObjectType",
                columns: table => new
                {
                    ChildObjectTypesObjectTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentObjectTypesObjectTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectTypeObjectType", x => new { x.ChildObjectTypesObjectTypeId, x.ParentObjectTypesObjectTypeId });
                    table.ForeignKey(
                        name: "FK_ObjectTypeObjectType_ObjectType_ChildObjectTypesObjectTypeId",
                        column: x => x.ChildObjectTypesObjectTypeId,
                        principalTable: "ObjectType",
                        principalColumn: "ObjectTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObjectTypeObjectType_ObjectType_ParentObjectTypesObjectTypeId",
                        column: x => x.ParentObjectTypesObjectTypeId,
                        principalTable: "ObjectType",
                        principalColumn: "ObjectTypeId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectTypeObjectType_ParentObjectTypesObjectTypeId",
                table: "ObjectTypeObjectType",
                column: "ParentObjectTypesObjectTypeId");
        }
    }
}
