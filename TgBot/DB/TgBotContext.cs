using TgBot.Base.Entities;
using Microsoft.EntityFrameworkCore;
using TelegramBot.Infrastructure.Database;
using TelegramBot.Infrastructure.Database.Entities;

namespace TgBot.DB
{
    public class TgBotContext : BotContext
    {
        public TgBotContext(DbContextOptions<TgBotContext> options) : base(options)
        {
        }

        public DbSet<PolicySettings> PolicySettings { get; set; }
        public DbSet<MessageEntry> MessageEntries { get; set; }
        public DbSet<UserWarnings> UserWarnings { get; set; }
        public DbSet<RulesMessage> Rules { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Stats> Stats { get; set; }
        public DbSet<Poll> Polls { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<PollAnswer> Answers { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }
        public DbSet<ChatMentionGroup> ChatMentionGroups { get; set; }
        public DbSet<MentionGroupUser> MentionGroupUsers { get; set; }
        public DbSet<UserDetails> UserDetails { get; set; }
        public DbSet<Reminder> Reminders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PolicySettings>().
                HasKey(settings => new { settings.ChatId, settings.UserId, settings.MessageType });
            modelBuilder.Entity<MessageEntry>().
                HasKey(entry => new { entry.ChatId, entry.MessageId });
            modelBuilder.Entity<Stats>().
                HasKey(entry => new { entry.ChatId, entry.UserId, entry.Date });
            modelBuilder.Entity<PollAnswer>().
                HasKey(entry => new { entry.PollId, entry.UserId });
            modelBuilder.Entity<ChatUser>().
                HasKey(entry => new {entry.ChatId, entry.UserId});
            modelBuilder.Entity<MentionGroupUser>().
                HasKey(entry => new {entry.UserId, entry.MentionGroupId});
            modelBuilder.Entity<UserDetails>().
                Property(entry => entry.Prefix).HasDefaultValue("😷");
            modelBuilder.Entity<Reminder>().
                HasKey(entry => new {entry.Name, entry.ChatId, entry.CreatorId});

        }
    }
}
