using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenericDataStore.Migrations
{
    /// <inheritdoc />
    public partial class nouniquename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ObjectType_Name",
                table: "ObjectType");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ObjectType",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ObjectType",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectType_Name",
                table: "ObjectType",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");
        }
    }
}
