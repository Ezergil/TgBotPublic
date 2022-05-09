using Microsoft.EntityFrameworkCore.Migrations;

namespace TgBot.Migrations.TgBot
{
    public partial class ReactionInsteadOfByte : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Bites",
                newName:"Reactions");

            migrationBuilder.AddColumn<int>(
                table: "Reactions",
                name: "ReactionType",
                defaultValue: 0,
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                table: "Reactions", 
                name: "ReactionType");

            migrationBuilder.RenameTable(
                name: "Reactions",
                newName: "Bites");
        }
    }
}
