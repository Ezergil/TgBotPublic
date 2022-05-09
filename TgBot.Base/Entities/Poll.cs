using System;
using System.ComponentModel.DataAnnotations;
using TelegramBot.Infrastructure.Database.Entities;

namespace TgBot.Base.Entities
{
    public class Poll : Entity
    {
        [Key]
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public long ChatId { get; set; }
        public int MessageId { get; set; }
        public PollState State { get; set; }
        public bool IsVote { get; set; }
        public long CreatedById { get; set; }

        public void Stop()
        {
            State = PollState.Stopped;
        }
    }

    public enum PollState
    {
        Active,
        Stopped
    }
}
