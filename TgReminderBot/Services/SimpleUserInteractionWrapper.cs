using System;
using System.Threading.Tasks;
using IRO.Storage;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.AspNetPipeline.Exceptions;
using Telegram.Bot.AspNetPipeline.Extensions.ReadWithoutContext;
using TgReminderBot.Data;

namespace TgReminderBot.Services
{
    public class SimpleUserInteractionWrapper : ISimpleUserInteractionWrapper
    {
        readonly UserService _userService;

        public DateTime LastConversation
        {
            get
            {
                var res = _userService.GetLastConversationTime(Username).Result;
                return res;
            }
            set
            {
                _userService.SetLastConversationTime(Username, value).Wait();
            }
        }

        public MailingSchedule Schedule
        {
            get
            {
                var res = _userService.GetSchedule(Username).Result;
                return res;
            }
            set
            {
                _userService.SetSchedule(Username, value).Wait();
            }
        }

        public ITelegramBotClient Bot { get; }
        public IKeyValueStorage UserStorage { get; }
        public string Username { get; }

        public SimpleUserInteractionWrapper(ITelegramBotClient bot, string username, UserService userService, UserScopedStorageProvider userScopedStorageProvider)
        {
            Username = username;
            UserStorage = userScopedStorageProvider.GetUserStorage(Username);
            _userService = userService;
            Bot = bot;
        }
        public async Task SendMessage(string msg)
        {
            var chatId=await _userService.GetUserChatId(Username);
            await Bot.SendTextMessageAsync(chatId, msg);
        }

        public async Task<string> ReadMessage()
        {
            var chatId = await _userService.GetUserChatId(Username);
            var msg = await Bot.ReadMessageAsync(chatId);
            return msg.Text;
        }
    }


}