using TelegramBot.Infrastructure.Database.Entities;
using TgBot.Base.Entities;

namespace TgBot.Base.DTO
{
    public class UserModel
    { 
        public long Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Prefix { get; set; }

        public UserModel()
        {
        }
        public UserModel(User user, UserDetails details)
        {
            Id = user.Id;
            UserName = user.UserName;
            FullName = details?.FullName;
            Prefix = details?.Prefix;
        }
    }
}