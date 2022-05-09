using System.Collections.Generic;
using System.Threading.Tasks;
using TgBot.Base.DTO;
using TgBot.Base.Entities;

namespace TgBot.Base.Interfaces
{
    public interface IUserService
    {
        UserModel GetById(long userId);
        IEnumerable<ChatUser> GetChatUsers(long chatId);
        IEnumerable<UserModel> GetAll();
        Task AddOrUpdateAsync(UserModel model);
        UserModel GetByUserName(string userName);
    }
}