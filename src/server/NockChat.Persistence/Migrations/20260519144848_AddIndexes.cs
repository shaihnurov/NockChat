using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NockChat.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Rooms_AccessCode",
                table: "Rooms",
                column: "AccessCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rooms_AccessCode",
                table: "Rooms");
        }
    }
}
