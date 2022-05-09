using Microsoft.EntityFrameworkCore.Migrations;

namespace TgBot.Migrations.TgBot
{
    public partial class IsVoteField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVote",
                table: "Polls",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVote",
                table: "Polls");
        }
    }
}
