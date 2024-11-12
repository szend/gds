using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenericDataStore.Migrations
{
    /// <inheritdoc />
    public partial class externalcount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AllowedExternalDataCount",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedExternalDataCount",
                table: "AspNetUsers");
        }
    }
}
