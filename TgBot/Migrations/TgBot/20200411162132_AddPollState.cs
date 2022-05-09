using Microsoft.EntityFrameworkCore.Migrations;

namespace TgBot.Migrations.TgBot
{
    public partial class AddPollState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Polls",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Polls");
        }
    }
}
