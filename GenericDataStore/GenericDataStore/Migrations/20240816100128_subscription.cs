using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenericDataStore.Migrations
{
    /// <inheritdoc />
    public partial class subscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasSub",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxDataCountInMonth",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxExternalDataCountInMonth",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextPay",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubStart",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubType",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasSub",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MaxDataCountInMonth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MaxExternalDataCountInMonth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NextPay",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SubStart",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SubType",
                table: "AspNetUsers");
        }
    }
}
