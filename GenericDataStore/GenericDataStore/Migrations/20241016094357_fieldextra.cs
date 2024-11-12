using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenericDataStore.Migrations
{
    /// <inheritdoc />
    public partial class fieldextra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DefaultOrder",
                table: "Field",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LabelColorMethod",
                table: "Field",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SizeMethod",
                table: "Field",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                table: "Field",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultOrder",
                table: "Field");

            migrationBuilder.DropColumn(
                name: "LabelColorMethod",
                table: "Field");

            migrationBuilder.DropColumn(
                name: "SizeMethod",
                table: "Field");

            migrationBuilder.DropColumn(
                name: "Visible",
                table: "Field");
        }
    }
}
