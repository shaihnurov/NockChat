using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NockChat.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_RoomId",
                table: "Messages");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RoomId_SentAt",
                table: "Messages",
                columns: new[] { "RoomId", "SentAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_RoomId_SentAt",
                table: "Messages");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RoomId",
                table: "Messages",
                column: "RoomId");
        }
    }
}
