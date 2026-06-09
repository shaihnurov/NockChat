using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NockChat.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceTextWithEncryptedPayload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Text",
                table: "Messages");

            migrationBuilder.AddColumn<string>(
                name: "EncryptedPayload",
                table: "Messages",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncryptedPayload",
                table: "Messages");

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Messages",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");
        }
    }
}
