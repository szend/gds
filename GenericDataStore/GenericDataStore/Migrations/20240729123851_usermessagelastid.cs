using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenericDataStore.Migrations
{
    /// <inheritdoc />
    public partial class usermessagelastid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LastMessageId",
                table: "UserMessage",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastMessageId",
                table: "UserMessage");
        }
    }
}
