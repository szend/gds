using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenericDataStore.Migrations
{
    /// <inheritdoc />
    public partial class usermessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataObjectId",
                table: "UserMessage");

            migrationBuilder.DropColumn(
                name: "NoVisibleReceiver",
                table: "UserMessage");

            migrationBuilder.DropColumn(
                name: "NoVisibleSender",
                table: "UserMessage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DataObjectId",
                table: "UserMessage",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NoVisibleReceiver",
                table: "UserMessage",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NoVisibleSender",
                table: "UserMessage",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
