using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenericDataStore.Migrations
{
    /// <inheritdoc />
    public partial class dashboard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Field",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Chart",
                columns: table => new
                {
                    ChartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ObjectTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RootFilter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Size = table.Column<int>(type: "int", nullable: true),
                    Position = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupOption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Xcalculation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ycalculation = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chart", x => x.ChartId);
                });

            migrationBuilder.CreateTable(
                name: "DashboardTable",
                columns: table => new
                {
                    DashboardTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ObjectTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RootFilter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Size = table.Column<int>(type: "int", nullable: true),
                    Position = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardTable", x => x.DashboardTableId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chart");

            migrationBuilder.DropTable(
                name: "DashboardTable");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Field");
        }
    }
}
