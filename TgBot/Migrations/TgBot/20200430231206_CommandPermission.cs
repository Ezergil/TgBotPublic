using Microsoft.EntityFrameworkCore.Migrations;

namespace TgBot.Migrations.TgBot
{
    public partial class CommandPermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommandPermissions",
                columns: table => new
                {
                    ChatId = table.Column<long>(nullable: false),
                    CommandHandlerName = table.Column<string>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    HasAccess = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandPermissions", x => new { x.ChatId, x.CommandHandlerName, x.UserId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommandPermissions");
        }
    }
}
