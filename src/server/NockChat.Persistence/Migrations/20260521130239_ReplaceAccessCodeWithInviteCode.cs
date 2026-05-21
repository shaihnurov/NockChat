using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NockChat.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceAccessCodeWithInviteCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rooms_AccessCode",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "AccessCode",
                table: "Rooms");

            migrationBuilder.AddColumn<string>(
                name: "InviteCode",
                table: "Rooms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InviteCodeExpiresAt",
                table: "Rooms",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InviteCode",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "InviteCodeExpiresAt",
                table: "Rooms");

            migrationBuilder.AddColumn<string>(
                name: "AccessCode",
                table: "Rooms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_AccessCode",
                table: "Rooms",
                column: "AccessCode",
                unique: true);
        }
    }
}
