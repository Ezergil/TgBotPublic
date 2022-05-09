using Microsoft.EntityFrameworkCore.Migrations;

namespace TgBot.Migrations.TgBot
{
    public partial class MentionGroupUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MentionGroupUsers",
                columns: table => new
                {
                    MentionGroupId = table.Column<int>(nullable: false),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MentionGroupUsers", x => new { x.UserId, x.MentionGroupId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MentionGroupUsers");
        }
    }
}
