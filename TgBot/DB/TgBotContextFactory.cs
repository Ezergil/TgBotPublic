using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TgBot.DB
{
    public class TgBotContextFactory : IDesignTimeDbContextFactory<TgBotContext>
    {
        public TgBotContext CreateDbContext(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder().
                SetBasePath(Directory.GetCurrentDirectory()).
                AddJsonFile("appSettings.json", optional: false).
                AddJsonFile("appSettings.Local.json", optional: false).
                Build();

            var optionsBuilder = new DbContextOptionsBuilder<TgBotContext>();
            var connectionString = config.GetConnectionString("PostgreConnectionString");
            optionsBuilder.UseNpgsql(connectionString);
            return new TgBotContext(optionsBuilder.Options);
        }
    }
}