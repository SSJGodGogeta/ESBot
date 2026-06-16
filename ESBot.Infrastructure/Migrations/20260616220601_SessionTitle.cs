using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESBot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SessionTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "UserSessions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "UserSessions");
        }
    }
}
