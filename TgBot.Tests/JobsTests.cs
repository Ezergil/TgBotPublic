using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using TgBot.Base.DTO;
using TgBot.Base.Entities;
using TgBot.Base.Interfaces;
using TgBot.Jobs;
using TgBot.Services;
using Moq;
using NUnit.Framework;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Infrastructure.Database.Interfaces;
using TelegramBot.Infrastructure.Extensions.DependencyInjection;
using TelegramBot.Infrastructure.Interfaces;
using Chat = TgBot.Base.Entities.Chat;
using Poll = TgBot.Base.Entities.Poll;
using PollAnswer = TgBot.Base.Entities.PollAnswer;

namespace TgBot.Tests
{
    [TestFixture]
    public class JobsTests
    {
        private PollNotifierJob _job;
        private Mock<ITelegramBotClientAdapter> _clientMock;
        private Mock<IRepository<Chat>> _chatRepositoryMock;
        private Mock<IPollService> _pollServiceMock;
        private Mock<IUserService> _userServiceMock;

        private static long chatId = 1;
        private Chat chat = new Chat { Id = chatId };
        private long user1Id = 1;
        private long user2Id = 2;
        private string pollId = "1";
        private int messageId = 69;
        [SetUp]
        public void Setup()
        {
            var builder = new ContainerBuilder();
            builder.RegisterTelegramBot();
            _chatRepositoryMock = new Mock<IRepository<Chat>>();
            _pollServiceMock = new Mock<IPollService>();
            _userServiceMock = new Mock<IUserService>();
            _clientMock = new Mock<ITelegramBotClientAdapter>();
            builder.Register(c => _chatRepositoryMock.Object).
                As<IRepository<Chat>>();
            builder.Register(c => _userServiceMock.Object).
                As<IUserService>();
            builder.Register(c => _pollServiceMock.Object).
                As<IPollService>();
            builder.Register(c => _clientMock.Object).
                As<ITelegramBotClientAdapter>();
            builder.RegisterType<PollNotifierJob>();
            builder.RegisterType<PollService>();

            _job = builder.Build().Resolve<PollNotifierJob>();
        }

        private void SuccessFlowSetup()
        {
            _chatRepositoryMock.
                Setup(m => m.GetAll()).
                Returns(new List<Chat> {chat});
            _clientMock.
                Setup(m => m.GetChatAsync(chatId)).
                Returns(Task.FromResult(new Telegram.Bot.Types.Chat
                {
                    Type = ChatType.Supergroup,
                    Permissions = new ChatPermissions
                    {
                        CanSendMessages = true
                    }
                }));
            _userServiceMock.
                Setup(m => m.GetChatUsers(chatId)).
                Returns(new List<ChatUser>
                {
                    new ChatUser {ChatId = chatId, UserId = user1Id},
                    new ChatUser {ChatId = chatId, UserId = user2Id}
                });
            _pollServiceMock.
                Setup(m => m.GetActivePolls(chatId))
                .Returns(new List<Poll> {new Poll {CreatedById = user1Id, Id = pollId, MessageId = messageId}});
            _pollServiceMock.
                Setup(m => m.GetPollAnswers(pollId)).
                Returns(new List<PollAnswer>
                {
                    new PollAnswer {PollId = pollId, Option = 0, UserId = user1Id},
                    new PollAnswer {PollId = pollId, Option = 1, UserId = user2Id}
                });
            _userServiceMock.
                Setup(m => m.GetById(user1Id)).
                Returns(new UserModel{UserName = "admin", Id = user1Id, FullName = "admin"});
        }

        [Test]
        public async Task TestSuccessFlow()
        {
            SuccessFlowSetup();
            await _job.Invoke();
            _clientMock.Verify(c => 
                c.SendTextMessageAsync(chatId, It.IsAny<string>(),
                    It.IsAny<ParseMode>(), messageId, It.IsAny<IReplyMarkup>()), Times.Exactly(1));
        }

        [Test]
        public async Task TestMessageIsNotSentWhenNotEnoughPollAnswers()
        {
            SuccessFlowSetup();
            _pollServiceMock.
                Setup(m => m.GetPollAnswers(pollId)).
                Returns(new List<PollAnswer>
                {
                    new PollAnswer {PollId = pollId, Option = 0, UserId = user1Id},
                });
            await _job.Invoke();
            _clientMock.Verify(c =>
                c.SendTextMessageAsync(It.IsAny<long>(), It.IsAny<string>(),
                    It.IsAny<ParseMode>(), It.IsAny<int>(), It.IsAny<IReplyMarkup>()), Times.Exactly(0));
        }

        [Test]
        public async Task TestJobSkipsInvocationWhenChatIsPrivate()
        {
            SuccessFlowSetup();
            _clientMock.Setup(m => m.GetChatAsync(chatId)).
                Returns(Task.FromResult(new Telegram.Bot.Types.Chat
                {
                    Type = ChatType.Private
                }));
            await _job.Invoke();
            _userServiceMock.Verify(service => service.GetChatUsers(It.IsAny<long>()), Times.Exactly(0));
        }

        [Test]
        public async Task TestBrokenChatsAreDeleted()
        {
            SuccessFlowSetup();
            _clientMock.
                Setup(m => m.GetChatAsync(chatId)).
                Returns(Task.FromResult(new Telegram.Bot.Types.Chat
                {
                    Type = ChatType.Supergroup,
                    Permissions = new ChatPermissions
                    {
                        CanSendMessages = false
                    }
                }));
            await _job.Invoke();
            _chatRepositoryMock.Verify(repository => repository.Delete(chat), Times.Exactly(1));
        }
    }
}