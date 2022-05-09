using Microsoft.EntityFrameworkCore.Migrations;

namespace TgBot.Migrations.TgBot
{
    public partial class PollAnswerOptionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Option",
                table: "Answers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Option",
                table: "Answers");
        }
    }
}
