using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenericDataStore.Migrations
{
    /// <inheritdoc />
    public partial class usermessageid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NoVisibleReceiver",
                table: "UserMessage",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NoVisibleSender",
                table: "UserMessage",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ObjectTypeId",
                table: "UserMessage",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoVisibleReceiver",
                table: "UserMessage");

            migrationBuilder.DropColumn(
                name: "NoVisibleSender",
                table: "UserMessage");

            migrationBuilder.DropColumn(
                name: "ObjectTypeId",
                table: "UserMessage");
        }
    }
}
