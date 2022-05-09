using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TelegramBot.Infrastructure.Database.Entities;

namespace TelegramBot.Infrastructure.Database
{
    public class BotContext : DbContext
    {
        public BotContext(DbContextOptions options) : base(options)
        {
            
        }
        
        public DbSet<ListenerState> ListenerStates { get; set; }
        public DbSet<CommandPermission> CommandPermissions { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ListenerState>().
                HasKey(entry => new { entry.ChatId, entry.ListenerType });
            modelBuilder.Entity<CommandPermission>().
                HasKey(entry => new { entry.ChatId, entry.CommandHandlerName, entry.UserId });
        }

        public int Commit()
        {
            try
            {
                var saveChanges = SaveChanges();

                return saveChanges;
            }
            catch (DbUpdateException)
            {
                return 0;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task<int> CommitAsync()
        {
            try
            {
                var saveChangesAsync = await SaveChangesAsync().ConfigureAwait(false);
                return saveChangesAsync;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}