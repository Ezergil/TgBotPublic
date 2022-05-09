using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TgBot.Base.DTO;
using TgBot.Base.Entities;
using TgBot.Base.Interfaces;
using TelegramBot.Infrastructure.Database.Entities;
using TelegramBot.Infrastructure.Database.Interfaces;

namespace TgBot.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<ChatUser> _chatUserRepository;
        private readonly IRepository<UserDetails> _userDetailsRepository;

        public UserService(IRepository<ChatUser> chatUserRepository, 
            IRepository<User> userRepository, 
            IRepository<UserDetails> userDetailsRepository)
        {
            _chatUserRepository = chatUserRepository;
            _userRepository = userRepository;
            _userDetailsRepository = userDetailsRepository;
        }

        public UserModel GetById(long userId)
        {
            var user = _userRepository.SingleOrDefault(u => u.Id == userId);
            var details = _userDetailsRepository.SingleOrDefault(u => u.UserId == userId);
            return new UserModel(user,details);
        }
        
        public UserModel GetByUserName(string userName)
        {
            var user = _userRepository.SingleOrDefault(u =>
                u.UserName.ToLower().Equals(userName.Replace("@", string.Empty).ToLower()));
            var details = _userDetailsRepository.SingleOrDefault(u => u.UserId == user.Id);
            return new UserModel(user,details);
        }

        public IEnumerable<ChatUser> GetChatUsers(long chatId)
        {
            return _chatUserRepository.Find(user => user.ChatId == chatId);
        }
        public IEnumerable<UserModel> GetAll()
        {
            return _userRepository.GetAll().Select(u =>
                new UserModel(u, _userDetailsRepository.SingleOrDefault(ud => ud.UserId == u.Id)));
        }

        public async Task AddOrUpdateAsync(UserModel model)
        {
            var user = _userRepository.SingleOrDefault(u => u.Id == model.Id);
            var details = _userDetailsRepository.SingleOrDefault(u => u.UserId == model.Id);
            
            UpdateFieldsIfChanged(model, user, details, out var userChanged, out var detailsChanged);

            if (userChanged)
                await _userRepository.AddOrUpdateAsync(user, u => u.Id == model.Id);
            if (detailsChanged)
                await _userDetailsRepository.AddOrUpdateAsync(details, d => d.UserId == model.Id);
            if (user == null)
                await _userRepository.AddOrUpdateAsync(new User
                {
                    Id = model.Id,
                    UserName = model.UserName
                });
            if (details == null)
                await _userDetailsRepository.AddOrUpdateAsync(new UserDetails
                {
                    UserId = model.Id,
                    FullName = model.FullName,
                    Prefix = model.Prefix
                });
        }

        private static void UpdateFieldsIfChanged(UserModel model, User user, UserDetails details, out bool userChanged,
            out bool detailsChanged)
        {
            userChanged = false;
            detailsChanged = false;
            if (user != null && !string.IsNullOrEmpty(model.UserName) && !model.UserName.Equals(user.UserName))
            {
                userChanged = true;
                user.UserName = model.UserName;
            }

            if (details != null)
            {
                if (!string.IsNullOrEmpty(model.Prefix) && !model.Prefix.Equals(details.Prefix))
                {
                    detailsChanged = true;
                    details.Prefix = model.Prefix;
                }

                if (!string.IsNullOrEmpty(model.FullName) && !model.FullName.Equals(details.FullName))
                {
                    detailsChanged = true;
                    details.FullName = model.FullName;
                }
            }
        }
    }
}