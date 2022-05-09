using Microsoft.EntityFrameworkCore.Migrations;

namespace TgBot.Migrations.TgBot
{
    public partial class MigrateFullNameAndPrefix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into \"UserDetails\" (\"UserId\" , \"FullName\" , \"Prefix\" ) (select u.\"Id\" , u.\"FullName\" , u.\"Prefix\" from \"Users\" u) on conflict do nothing");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
