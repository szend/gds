using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenericDataStore.Migrations
{
    /// <inheritdoc />
    public partial class moreparent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObjectType_ObjectType_ParentObjectTypeId",
                table: "ObjectType");

            migrationBuilder.DropIndex(
                name: "IX_ObjectType_ParentObjectTypeId",
                table: "ObjectType");

            migrationBuilder.DropColumn(
                name: "ParentObjectTypeId",
                table: "ObjectType");

            migrationBuilder.AlterColumn<string>(
                name: "PropertyName",
                table: "Field",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjectTypeObjectType");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentObjectTypeId",
                table: "ObjectType",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PropertyName",
                table: "Field",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectType_ParentObjectTypeId",
                table: "ObjectType",
                column: "ParentObjectTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectType_ObjectType_ParentObjectTypeId",
                table: "ObjectType",
                column: "ParentObjectTypeId",
                principalTable: "ObjectType",
                principalColumn: "ObjectTypeId");
        }
    }
}
