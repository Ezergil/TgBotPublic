using Microsoft.EntityFrameworkCore.Migrations;

namespace TgBot.Migrations.PostgreBot
{
    public partial class RemovedUserFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Prefix",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prefix",
                table: "Users",
                type: "text",
                nullable: true,
                defaultValue: "😷");
        }
    }
}
